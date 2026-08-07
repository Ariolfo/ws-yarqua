using System.Text.RegularExpressions;

namespace Yarqua.Application.Services;

/// <summary>
/// Sensor físico del catálogo estático (estación Visualiti M###).
/// </summary>
public sealed class PhysicalSensor
{
    /// <summary>
    /// Crea un sensor físico del catálogo.
    /// </summary>
    public PhysicalSensor(
        string serial,
        string red,
        string? cultivo = null,
        string? finca = null,
        string? pais = null,
        double? latitud = null,
        double? longitud = null,
        int canales = SensorCatalog.DefaultChannels)
    {
        Serial = serial;
        Red = red;
        Cultivo = cultivo;
        Finca = finca;
        Pais = pais;
        Latitud = latitud;
        Longitud = longitud;
        Canales = canales;
    }

    /// <summary>Serial Visualiti (M###).</summary>
    public string Serial { get; }

    /// <summary>Red de sensores.</summary>
    public string Red { get; }

    /// <summary>Cultivo asociado.</summary>
    public string? Cultivo { get; }

    /// <summary>Nombre de finca.</summary>
    public string? Finca { get; }

    /// <summary>País.</summary>
    public string? Pais { get; }

    /// <summary>Latitud WGS84.</summary>
    public double? Latitud { get; }

    /// <summary>Longitud WGS84.</summary>
    public double? Longitud { get; }

    /// <summary>Canales lógicos expuestos.</summary>
    public int Canales { get; }
}

/// <summary>
/// Catálogo estático de sensores Yarqua.
/// </summary>
public static partial class SensorCatalog
{
    /// <summary>Canales lógicos por defecto.</summary>
    public const int DefaultChannels = 2;

    private static readonly Regex LogicalIdRegex = MyLogicalIdRegex();

    private static readonly PhysicalSensor[] Sensors =
    [
        // Colombia — RED ASORUT
        new("M312", "RED ASORUT", "Lima ácida Tahiti", "Finca El Vergel", "COLOMBIA", 4.53281, -76.0704),
        new("M313", "RED ASORUT", "Lima ácida Tahiti", "Finca El Vergel", "COLOMBIA", 4.53297, -76.0704),
        new("M314", "RED ASORUT", "Lima ácida Tahiti", "Finca El Vergel", "COLOMBIA", 4.53314, -76.07039),
        new("M315", "RED ASORUT", "Aguacate", null, "COLOMBIA"),
        new("M316", "RED ASORUT", "Aguacate", "Finca San Antonio", "COLOMBIA", 4.52198, -76.07732),
        new("M317", "RED ASORUT", "Aguacate", "Finca San Antonio", "COLOMBIA", 4.52191, -76.0774),
        new("M318", "RED ASORUT", "Cacao", null, "COLOMBIA"),
        new("M319", "RED ASORUT", "Cacao", "Finca San Antonio", "COLOMBIA", 4.52369, -76.07819),
        new("M320", "RED ASORUT", "Cacao", null, "COLOMBIA"),
        new("M321", "RED ASORUT", "Papaya", null, "COLOMBIA"),
        new("M322", "RED ASORUT", "Papaya", "Finca La Floresta", "COLOMBIA", 4.47195, -76.08945),
        new("M323", "RED ASORUT", "Papaya", null, "COLOMBIA"),
        new("M336", "RED ASORUT", null, null, "COLOMBIA"),
        // Ecuador
        new("M333", "RED ECUADOR", "Cacao UTM", null, "ECUADOR"),
        new("M334", "RED ECUADOR", "Cacao UTM", null, "ECUADOR"),
        new("M335", "RED ECUADOR", "Cacao Productor", null, "ECUADOR"),
        // Honduras
        new("M324", "RED HONDURA", "Lima ácida Tahiti", null, "HONDURAS"),
        new("M325", "RED HONDURA", "Lima ácida Tahiti", null, "HONDURAS"),
        new("M326", "RED HONDURA", "Lima ácida Tahiti", null, "HONDURAS"),
        new("M327", "RED HONDURA", "Papaya", null, "HONDURAS"),
        new("M328", "RED HONDURA", "Papaya", null, "HONDURAS"),
        new("M329", "RED HONDURA", "Papaya", null, "HONDURAS"),
        new("M330", "RED HONDURA", "Cacao", null, "HONDURAS"),
        new("M331", "RED HONDURA", "Cacao", null, "HONDURAS"),
        new("M332", "RED HONDURA", "Cacao", null, "HONDURAS"),
    ];

    private static readonly Dictionary<string, PhysicalSensor> BySerial =
        Sensors.ToDictionary(s => s.Serial, StringComparer.Ordinal);

    /// <summary>
    /// Separa un ID lógico M316-1 en (M316, 1). Sin sufijo → canal null.
    /// </summary>
    /// <param name="sensorId">Id físico o lógico.</param>
    /// <returns>Serial físico y canal opcional.</returns>
    public static (string Physical, int? Channel) SplitLogicalId(string sensorId)
    {
        var trimmed = sensorId.Trim();
        var match = LogicalIdRegex.Match(trimmed);
        if (match.Success)
        {
            return (match.Groups["physical"].Value, int.Parse(match.Groups["channel"].Value));
        }

        return (trimmed, null);
    }

    /// <summary>
    /// Cont Vol{n} → sensor lógico M###-n (sensor_n).
    /// </summary>
    /// <param name="physicalSerial">Serial físico.</param>
    /// <param name="channel">Canal.</param>
    /// <returns>Id lógico.</returns>
    public static string LogicalSensorId(string physicalSerial, int channel) =>
        $"{physicalSerial}-{channel}";

    /// <summary>
    /// Lista todos los sensores físicos.
    /// </summary>
    /// <returns>Copia de la lista.</returns>
    public static IReadOnlyList<PhysicalSensor> ListSensors() => Sensors;

    /// <summary>
    /// Lista sensores con coordenadas.
    /// </summary>
    /// <returns>Sensores geolocalizados.</returns>
    public static IReadOnlyList<PhysicalSensor> ListGeolocatedSensors() =>
        Sensors.Where(s => s.Latitud is not null && s.Longitud is not null).ToList();

    /// <summary>
    /// Busca por serial físico o ID lógico.
    /// </summary>
    /// <param name="sensorId">M316 o M316-1.</param>
    /// <returns>Sensor o null.</returns>
    public static PhysicalSensor? GetSensor(string sensorId)
    {
        var (physical, _) = SplitLogicalId(sensorId);
        return BySerial.TryGetValue(physical, out var sensor) ? sensor : null;
    }

    /// <summary>
    /// Distancia en metros entre dos puntos WGS84 (Haversine).
    /// </summary>
    /// <param name="lat1">Latitud origen.</param>
    /// <param name="lng1">Longitud origen.</param>
    /// <param name="lat2">Latitud destino.</param>
    /// <param name="lng2">Longitud destino.</param>
    /// <returns>Distancia en metros.</returns>
    public static double HaversineM(double lat1, double lng1, double lat2, double lng2)
    {
        const double radius = 6371000.0;
        var phi1 = DegreesToRadians(lat1);
        var phi2 = DegreesToRadians(lat2);
        var dphi = DegreesToRadians(lat2 - lat1);
        var dlambda = DegreesToRadians(lng2 - lng1);
        var a = Math.Sin(dphi / 2) * Math.Sin(dphi / 2)
                + Math.Cos(phi1) * Math.Cos(phi2) * Math.Sin(dlambda / 2) * Math.Sin(dlambda / 2);
        return 2 * radius * Math.Asin(Math.Sqrt(a));
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

    [GeneratedRegex(@"^(?<physical>.+?)-(?<channel>\d+)$")]
    private static partial Regex MyLogicalIdRegex();
}
