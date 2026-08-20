namespace Hidrix.Application.DTOs;

/// <summary>DTO de cultivo.</summary>
public sealed class CropDto
{
    /// <summary>Id.</summary>
    public int Id { get; set; }

    /// <summary>Nombre.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Capacidad de campo (%).</summary>
    public double FieldCapacity { get; set; }

    /// <summary>% máximo.</summary>
    public double MaxIrrigationLimit { get; set; }

    /// <summary>% decisión de riego.</summary>
    public double IrrigationDecision { get; set; }
}

/// <summary>Request para crear/actualizar cultivo.</summary>
public sealed class CreateCropRequest
{
    /// <summary>Nombre del cultivo.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Capacidad de campo.</summary>
    public double FieldCapacity { get; set; }

    /// <summary>% máximo.</summary>
    public double MaxIrrigationLimit { get; set; }

    /// <summary>% decisión de riego.</summary>
    public double IrrigationDecision { get; set; }
}

/// <summary>DTO de método para capacidad de campo.</summary>
public sealed class MetodoCCDto
{
    /// <summary>Id.</summary>
    public int Id { get; set; }

    /// <summary>Nombre.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descripción.</summary>
    public string? Description { get; set; }
}

/// <summary>Request para crear/actualizar método CC.</summary>
public sealed class CreateMetodoCCRequest
{
    /// <summary>Nombre.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descripción.</summary>
    public string? Description { get; set; }
}

/// <summary>DTO de red con país.</summary>
public sealed class NetworkDto
{
    /// <summary>Id de red.</summary>
    public int Id { get; set; }

    /// <summary>Nombre de red.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Id de país.</summary>
    public int CountryId { get; set; }

    /// <summary>Nombre de país.</summary>
    public string CountryName { get; set; } = string.Empty;
}

/// <summary>DTO de sensor de catálogo.</summary>
public sealed class CatalogSensorDto
{
    /// <summary>Id interno.</summary>
    public int Id { get; set; }

    /// <summary>Nombre / serial.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Id de red.</summary>
    public int NetworkId { get; set; }

    /// <summary>Nombre de red.</summary>
    public string NetworkName { get; set; } = string.Empty;

    /// <summary>Id de país.</summary>
    public int CountryId { get; set; }

    /// <summary>Nombre de país.</summary>
    public string CountryName { get; set; } = string.Empty;

    /// <summary>Id de cultivo.</summary>
    public int? CropId { get; set; }

    /// <summary>Nombre de cultivo.</summary>
    public string? CropName { get; set; }

    /// <summary>Latitud.</summary>
    public double? Latitude { get; set; }

    /// <summary>Longitud.</summary>
    public double? Longitude { get; set; }

    /// <summary>Estado del sensor.</summary>
    public string? SensorStatus { get; set; }

    /// <summary>Conectividad (online/offline).</summary>
    public string? Connectivity { get; set; }

    /// <summary>Finca.</summary>
    public string? Farm { get; set; }
}

/// <summary>Request para crear/actualizar sensor manualmente.</summary>
public sealed class CreateCatalogSensorRequest
{
    /// <summary>Nombre del sensor (ej. M333-1).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Id de red.</summary>
    public int NetworkId { get; set; }

    /// <summary>Id de cultivo (opcional).</summary>
    public int? CropId { get; set; }

    /// <summary>Latitud.</summary>
    public double? Latitude { get; set; }

    /// <summary>Longitud.</summary>
    public double? Longitude { get; set; }

    /// <summary>Estado del sensor.</summary>
    public string? SensorStatus { get; set; }

    /// <summary>Conectividad.</summary>
    public string? Connectivity { get; set; }

    /// <summary>Finca.</summary>
    public string? Farm { get; set; }
}
