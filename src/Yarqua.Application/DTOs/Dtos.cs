namespace Yarqua.Application.DTOs;

/// <summary>Usuario expuesto en autenticación.</summary>
public class UserDto
{
    /// <summary>Identificador (GUID de Identity).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Nombre para mostrar.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Correo electrónico.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Roles asignados.</summary>
    public string[] Roles { get; set; } = [];

    /// <summary>País canónico (al registrar).</summary>
    public string? Country { get; set; }

    /// <summary>Departamento canónico (al registrar).</summary>
    public string? Department { get; set; }

    /// <summary>Ciudad canónica (al registrar).</summary>
    public string? City { get; set; }
}

/// <summary>Respuesta de registro / login.</summary>
public class AuthDto
{
    /// <summary>JWT de acceso.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>JWT de refresco.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Datos del usuario.</summary>
    public UserDto User { get; set; } = new();
}

/// <summary>Respuesta de refresh de tokens.</summary>
public class RefreshDto
{
    /// <summary>Nuevo JWT de acceso.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Nuevo JWT de refresco.</summary>
    public string? RefreshToken { get; set; }
}

/// <summary>País del catálogo geo.</summary>
public class GeoCountryDto
{
    /// <summary>Identificador.</summary>
    public int Id { get; set; }

    /// <summary>Nombre.</summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>Departamento del catálogo geo.</summary>
public class GeoDepartmentDto
{
    /// <summary>Identificador.</summary>
    public int Id { get; set; }

    /// <summary>Código.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Nombre.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Id del país.</summary>
    public int PaisId { get; set; }
}

/// <summary>Ciudad del catálogo geo.</summary>
public class GeoCityDto
{
    /// <summary>Identificador.</summary>
    public int Id { get; set; }

    /// <summary>Código.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Nombre.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Id del departamento.</summary>
    public int DepoId { get; set; }
}

/// <summary>Lectura de humedad por canal (sensor_1 / sensor_2).</summary>
public class ReadingDto
{
    /// <summary>Canal lógico como código de profundidad (10 = sensor_1, 30 = sensor_2).</summary>
    public int DepthCm { get; set; }

    /// <summary>Valor en porcentaje.</summary>
    public double Value { get; set; }

    /// <summary>Marca de tiempo.</summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>Unidad (siempre %).</summary>
    public string Unit { get; set; } = "%";
}

/// <summary>Sensor físico o lógico.</summary>
public class SensorDto
{
    /// <summary>Id (M316 o M316-1).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Id de estación (fin-... o sn-...).</summary>
    public string StationId { get; set; } = string.Empty;

    /// <summary>Nombre descriptivo.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Ubicación textual.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Estado: excess, attention_high, irrigate, attention_low, deficit, no_data.</summary>
    public string Status { get; set; } = "no_data";

    /// <summary>Última lectura.</summary>
    public DateTimeOffset LastReadingAt { get; set; }

    /// <summary>Lecturas por canal (sensor_1 / sensor_2).</summary>
    public List<ReadingDto> Readings { get; set; } = new();

    /// <summary>Mensaje de alerta opcional.</summary>
    public string? AlertMessage { get; set; }

    /// <summary>Latitud.</summary>
    public double? Latitude { get; set; }

    /// <summary>Longitud.</summary>
    public double? Longitude { get; set; }
}

/// <summary>Estación (finca o sensor suelto).</summary>
public class StationDto
{
    /// <summary>Id de estación.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Nombre.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Latitud media.</summary>
    public double Latitude { get; set; }

    /// <summary>Longitud media.</summary>
    public double Longitude { get; set; }

    /// <summary>Cantidad de sensores lógicos.</summary>
    public int SensorCount { get; set; }

    /// <summary>Distancia en km al punto de consulta.</summary>
    public double? DistanceKm { get; set; }

    /// <summary>Sensores anidados (opcional).</summary>
    public List<SensorDto> Sensors { get; set; } = new();
}

/// <summary>Punto de histórico de humedad.</summary>
public class HistoryPointDto
{
    /// <summary>Marca de tiempo.</summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>Humedad Cont Vol1 (sensor_1).</summary>
    public double Depth10cm { get; set; }

    /// <summary>Humedad Cont Vol2 (sensor_2).</summary>
    public double Depth30cm { get; set; }
}

/// <summary>
/// Detalle de sensor más histórico en una sola respuesta (optimización de carga de gráfica).
/// </summary>
public class SensorWithHistoryDto
{
    /// <summary>Sensor con última lectura derivada del histórico.</summary>
    public SensorDto Sensor { get; set; } = new();

    /// <summary>Serie histórica del rango solicitado.</summary>
    public List<HistoryPointDto> History { get; set; } = new();

    /// <summary>Rango aplicado (today|7d|30d|6m).</summary>
    public string Range { get; set; } = "7d";
}

/// <summary>Respuesta de salud del servicio.</summary>
public class HealthDto
{
    /// <summary>Estado general (ok|degraded).</summary>
    public string Status { get; set; } = "ok";

    /// <summary>Estado de la base (connected|unavailable).</summary>
    public string Database { get; set; } = "connected";
}
