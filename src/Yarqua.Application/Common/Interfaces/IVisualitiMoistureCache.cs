namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Caché en memoria compartida (singleton) de sesión Visualiti: token, última lectura e histórico.
/// Evita que el cliente HTTP tipado Transient pierda estado entre peticiones HTTP.
/// </summary>
public interface IVisualitiMoistureCache
{
    /// <summary>
    /// Intenta obtener el token Bearer vigente.
    /// </summary>
    /// <param name="token">Token si es válido.</param>
    /// <returns>True si hay token usable.</returns>
    bool TryGetToken(out string token);

    /// <summary>
    /// Guarda el token con su expiración absoluta.
    /// </summary>
    /// <param name="token">Access token.</param>
    /// <param name="expiresAt">Expiración UTC.</param>
    void SetToken(string token, DateTimeOffset expiresAt);

    /// <summary>
    /// Ejecuta <paramref name="factory"/> bajo un candado de proceso único
    /// (singleton) para evitar stampede de login cuando hay muchos HttpClient Transient.
    /// </summary>
    /// <param name="factory">Operación asíncrona (p. ej. login Visualiti).</param>
    /// <param name="cancellationToken">Cancelación.</param>
    /// <typeparam name="T">Tipo de resultado.</typeparam>
    /// <returns>Resultado de la factory.</returns>
    Task<T> RunExclusiveAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Intenta obtener la última lectura cacheada del serial físico.
    /// </summary>
    /// <param name="physicalSerial">Serial M###.</param>
    /// <param name="reading">Lectura o null cacheado.</param>
    /// <returns>True si la entrada no ha expirado.</returns>
    bool TryGetLatest(string physicalSerial, out VisualitiReading? reading);

    /// <summary>
    /// Guarda la última lectura con TTL.
    /// </summary>
    /// <param name="physicalSerial">Serial M###.</param>
    /// <param name="reading">Lectura o null.</param>
    /// <param name="ttl">Tiempo de vida.</param>
    void SetLatest(string physicalSerial, VisualitiReading? reading, TimeSpan ttl);

    /// <summary>
    /// Intenta obtener el histórico cacheado por serial y rango.
    /// </summary>
    /// <param name="physicalSerial">Serial M###.</param>
    /// <param name="rangeKey">Clave de rango (today|7d|30d|6m).</param>
    /// <param name="readings">Serie ordenada.</param>
    /// <returns>True si la entrada no ha expirado.</returns>
    bool TryGetHistory(
        string physicalSerial,
        string rangeKey,
        out IReadOnlyList<VisualitiReading> readings);

    /// <summary>
    /// Guarda el histórico con TTL.
    /// </summary>
    /// <param name="physicalSerial">Serial M###.</param>
    /// <param name="rangeKey">Clave de rango.</param>
    /// <param name="readings">Serie.</param>
    /// <param name="ttl">Tiempo de vida.</param>
    void SetHistory(
        string physicalSerial,
        string rangeKey,
        IReadOnlyList<VisualitiReading> readings,
        TimeSpan ttl);

    /// <summary>
    /// Busca en históricos cacheados del serial el punto más reciente (para alimentar «latest»).
    /// </summary>
    /// <param name="physicalSerial">Serial M###.</param>
    /// <returns>Última lectura encontrada o null.</returns>
    VisualitiReading? FindLatestFromHistory(string physicalSerial);
}
