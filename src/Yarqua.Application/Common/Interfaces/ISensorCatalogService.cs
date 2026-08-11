using Yarqua.Application.DTOs;
using Yarqua.Application.Services;

namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Catálogo de sensores persistido (reemplaza el array estático).
/// </summary>
public interface ISensorCatalogService
{
    /// <summary>Lista sensores activos como PhysicalSensor.</summary>
    Task<IReadOnlyList<PhysicalSensor>> ListSensorsAsync(CancellationToken cancellationToken = default);

    /// <summary>Lista sensores con coordenadas.</summary>
    Task<IReadOnlyList<PhysicalSensor>> ListGeolocatedSensorsAsync(CancellationToken cancellationToken = default);

    /// <summary>Busca por serial físico o id lógico.</summary>
    Task<PhysicalSensor?> GetSensorAsync(string sensorId, CancellationToken cancellationToken = default);

    /// <summary>Lista DTOs agrupables por país/red.</summary>
    Task<IReadOnlyList<CatalogSensorDto>> ListCatalogAsync(CancellationToken cancellationToken = default);

    /// <summary>Obtiene un sensor del catálogo por id interno.</summary>
    Task<CatalogSensorDto?> GetCatalogByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Crea un sensor manualmente.</summary>
    Task<CatalogSensorDto> CreateAsync(CreateCatalogSensorRequest request, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un sensor del catálogo.</summary>
    Task<CatalogSensorDto> UpdateAsync(int id, CreateCatalogSensorRequest request, CancellationToken cancellationToken = default);

    /// <summary>Lista redes con país.</summary>
    Task<IReadOnlyList<NetworkDto>> ListNetworksAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Catálogo de cultivos persistido.
/// </summary>
public interface ICropCatalogService
{
    /// <summary>Lista cultivos activos.</summary>
    Task<IReadOnlyList<CropDto>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Obtiene un cultivo por id.</summary>
    Task<CropDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Crea un cultivo.</summary>
    Task<CropDto> CreateAsync(CreateCropRequest request, CancellationToken cancellationToken = default);

    /// <summary>Actualiza un cultivo.</summary>
    Task<CropDto> UpdateAsync(int id, CreateCropRequest request, CancellationToken cancellationToken = default);

    /// <summary>Resuelve perfil por nombre/texto (fallback Cacao).</summary>
    Task<CropMoistureProfile> ResolveAsync(string? cultivoOrText, CancellationToken cancellationToken = default);
}
