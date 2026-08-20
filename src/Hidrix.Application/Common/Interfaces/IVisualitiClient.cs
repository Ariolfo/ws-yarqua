namespace Hidrix.Application.Common.Interfaces;

/// <summary>
/// Lectura de humedad obtenida desde Visualiti.
/// </summary>
public class VisualitiReading
{
    /// <summary>Marca de tiempo UTC de la lectura.</summary>
    public DateTimeOffset FechaHora { get; set; }

    /// <summary>Valores en porcentaje por canal lógico (1 = Cont Vol1, 2 = Cont Vol2).</summary>
    public Dictionary<int, double> Valores { get; set; } = new();

    /// <summary>Valor del canal 1 (Cont Vol1 / sensor_1).</summary>
    public double? Volumetrico1 => Valores.TryGetValue(1, out var v) ? v : null;

    /// <summary>Valor del canal 2 (Cont Vol2 / sensor_2).</summary>
    public double? Volumetrico2 => Valores.TryGetValue(2, out var v) ? v : null;

    /// <summary>
    /// Obtiene el valor de un canal.
    /// </summary>
    /// <param name="channel">Número de canal.</param>
    /// <returns>Porcentaje o null.</returns>
    public double? ValueForChannel(int channel) =>
        Valores.TryGetValue(channel, out var v) ? v : null;

    /// <summary>Canales presentes ordenados.</summary>
    public IReadOnlyList<int> Channels => Valores.Keys.OrderBy(k => k).ToList();
}

/// <summary>
/// Cliente HTTP hacia la API Visualiti (appgricultor).
/// </summary>
public interface IVisualitiClient
{
    /// <summary>
    /// Obtiene la última lectura.
    /// Si hay serie histórica en caché, Latest = último punto (sin llamada Visualiti).
    /// Si no, carga rango 7d (queda en caché) y toma el último punto.
    /// </summary>
    /// <param name="sensorSerial">Serial M### o lógico M###-n.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lectura o null.</returns>
    Task<VisualitiReading?> GetLatestReadingCachedAsync(
        string sensorSerial,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene lecturas de humedad desde una fecha (sin clave de rango; no usa caché por rango).
    /// Preferir <see cref="FetchMoistureReadingsForRangeAsync"/> cuando se conozca el rango.
    /// </summary>
    /// <param name="sensorSerial">Serial M### o lógico.</param>
    /// <param name="since">Inicio del rango (UTC).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de lecturas ordenadas.</returns>
    Task<IReadOnlyList<VisualitiReading>> FetchMoistureReadingsAsync(
        string sensorSerial,
        DateTimeOffset since,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene lecturas para un rango tipado (today|7d|30d|6m) con caché en memoria.
    /// </summary>
    /// <param name="sensorSerial">Serial M### o lógico.</param>
    /// <param name="rangeKey">Clave de rango.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de lecturas ordenadas.</returns>
    Task<IReadOnlyList<VisualitiReading>> FetchMoistureReadingsForRangeAsync(
        string sensorSerial,
        string rangeKey,
        CancellationToken cancellationToken = default);
}
