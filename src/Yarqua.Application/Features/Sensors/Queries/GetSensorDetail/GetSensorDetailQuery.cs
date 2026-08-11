using Mediator;
using Yarqua.Application.Common.Exceptions;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;
using Yarqua.Application.Services;

namespace Yarqua.Application.Features.Sensors.Queries.GetSensorDetail;

/// <summary>
/// Consulta detalle de un sensor físico o lógico.
/// </summary>
public class GetSensorDetailQuery : IRequest<SensorDto>
{
    /// <summary>Id M316 o M316-1.</summary>
    public string SensorId { get; set; } = string.Empty;
}

/// <summary>
/// Handler de detalle de sensor.
/// </summary>
public class GetSensorDetailQueryHandler : IRequestHandler<GetSensorDetailQuery, SensorDto>
{
    private readonly IVisualitiClient _visualiti;
    private readonly ISensorCatalogService _catalog;

    /// <summary>
    /// Inicializa el handler.
    /// </summary>
    public GetSensorDetailQueryHandler(IVisualitiClient visualiti, ISensorCatalogService catalog)
    {
        _visualiti = visualiti;
        _catalog = catalog;
    }

    /// <summary>
    /// Obtiene el sensor con última lectura.
    /// </summary>
    public async ValueTask<SensorDto> Handle(GetSensorDetailQuery request, CancellationToken cancellationToken)
    {
        var sensor = await _catalog.GetSensorAsync(request.SensorId, cancellationToken)
                     ?? throw new NotFoundException($"Sensor no encontrado: {request.SensorId}");

        var (_, channel) = SensorCatalog.SplitLogicalId(request.SensorId);
        var reading = await _visualiti.GetLatestReadingCachedAsync(sensor.Serial, cancellationToken);
        var stationId = StationIds.EncodeStationId(StationIds.GrupoFromSensor(sensor.Serial, sensor.Finca));
        return SensorMapper.ToSensorDto(sensor, stationId, reading, channel);
    }
}
