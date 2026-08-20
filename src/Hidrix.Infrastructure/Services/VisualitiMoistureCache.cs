using System.Collections.Concurrent;
using Hidrix.Application.Common.Interfaces;

namespace Hidrix.Infrastructure.Services;

/// <summary>
/// Caché en memoria thread-safe de token, latest e histórico Visualiti (singleton DI).
/// </summary>
public sealed class VisualitiMoistureCache : IVisualitiMoistureCache
{
    private readonly object _tokenLock = new();
    private readonly SemaphoreSlim _exclusiveGate = new(1, 1);
    private string? _token;
    private DateTimeOffset _tokenExpiresAt = DateTimeOffset.MinValue;

    private readonly ConcurrentDictionary<string, CacheEntry<VisualitiReading?>> _latest = new(
        StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentDictionary<string, CacheEntry<IReadOnlyList<VisualitiReading>>> _history = new(
        StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool TryGetToken(out string token)
    {
        lock (_tokenLock)
        {
            if (_token is not null && DateTimeOffset.UtcNow < _tokenExpiresAt.AddSeconds(-60))
            {
                token = _token;
                return true;
            }
        }

        token = string.Empty;
        return false;
    }

    /// <inheritdoc />
    public void SetToken(string token, DateTimeOffset expiresAt)
    {
        lock (_tokenLock)
        {
            _token = token;
            _tokenExpiresAt = expiresAt;
        }
    }

    /// <inheritdoc />
    public async Task<T> RunExclusiveAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        await _exclusiveGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await factory(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _exclusiveGate.Release();
        }
    }

    /// <inheritdoc />
    public bool TryGetLatest(string physicalSerial, out VisualitiReading? reading)
    {
        if (_latest.TryGetValue(physicalSerial, out var entry) && entry.ExpiresAt > DateTimeOffset.UtcNow)
        {
            reading = entry.Value;
            return true;
        }

        reading = null;
        return false;
    }

    /// <inheritdoc />
    public void SetLatest(string physicalSerial, VisualitiReading? reading, TimeSpan ttl)
    {
        _latest[physicalSerial] = new CacheEntry<VisualitiReading?>(
            reading,
            DateTimeOffset.UtcNow.Add(ttl));
    }

    /// <inheritdoc />
    public bool TryGetHistory(
        string physicalSerial,
        string rangeKey,
        out IReadOnlyList<VisualitiReading> readings)
    {
        var key = HistoryKey(physicalSerial, rangeKey);
        if (_history.TryGetValue(key, out var entry) && entry.ExpiresAt > DateTimeOffset.UtcNow)
        {
            readings = entry.Value;
            return true;
        }

        readings = Array.Empty<VisualitiReading>();
        return false;
    }

    /// <inheritdoc />
    public void SetHistory(
        string physicalSerial,
        string rangeKey,
        IReadOnlyList<VisualitiReading> readings,
        TimeSpan ttl)
    {
        var key = HistoryKey(physicalSerial, rangeKey);
        _history[key] = new CacheEntry<IReadOnlyList<VisualitiReading>>(
            readings,
            DateTimeOffset.UtcNow.Add(ttl));
    }

    /// <inheritdoc />
    public VisualitiReading? FindLatestFromHistory(string physicalSerial)
    {
        VisualitiReading? best = null;
        foreach (var pair in _history)
        {
            if (!pair.Key.StartsWith(physicalSerial + "|", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (pair.Value.ExpiresAt <= DateTimeOffset.UtcNow || pair.Value.Value.Count == 0)
            {
                continue;
            }

            var last = pair.Value.Value[^1];
            if (best is null || last.FechaHora > best.FechaHora)
            {
                best = last;
            }
        }

        return best;
    }

    private static string HistoryKey(string physicalSerial, string rangeKey) =>
        $"{physicalSerial}|{rangeKey.Trim().ToLowerInvariant()}";

    private sealed record CacheEntry<T>(T Value, DateTimeOffset ExpiresAt);
}
