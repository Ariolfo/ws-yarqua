using MediatR;
using Yarqua.Application.Common.Exceptions;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;
using Yarqua.Application.Services;

namespace Yarqua.Application.Features.Sensors.Queries.GetSensorHistory;

/// <summary>
/// Consulta histórico de humedad desde Visualiti (con caché por rango).
/// </summary>
public class GetSensorHistoryQuery : IRequest<IReadOnlyList<HistoryPointDto>>
{
    /// <summary>Id M316 o M316-1.</summary>
    public string SensorId { get; set; } = string.Empty;

    /// <summary>Rango: today|7d|30d|6m.</summary>
    public string Range { get; set; } = "7d";
}

/// <summary>
/// Handler de histórico de sensor.
/// </summary>
public class GetSensorHistoryQueryHandler : IRequestHandler<GetSensorHistoryQuery, IReadOnlyList<HistoryPointDto>>
{
    private readonly IVisualitiClient _visualiti;

    /// <summary>
    /// Inicializa el handler.
    /// </summary>
    /// <param name="visualiti">Cliente Visualiti.</param>
    public GetSensorHistoryQueryHandler(IVisualitiClient visualiti)
    {
        _visualiti = visualiti;
    }

    /// <summary>
    /// Obtiene puntos Cont Vol1/Vol2 (sensor_1 / sensor_2).
    /// </summary>
    public async Task<IReadOnlyList<HistoryPointDto>> Handle(
        GetSensorHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (!HistoryRangeHelper.IsValid(request.Range))
        {
            throw new AppException($"Rango inválido: {request.Range}");
        }

        var sensor = SensorCatalog.GetSensor(request.SensorId)
                     ?? throw new NotFoundException($"Sensor no encontrado: {request.SensorId}");

        var readings = await _visualiti.FetchMoistureReadingsForRangeAsync(
            sensor.Serial,
            request.Range,
            cancellationToken);

        return readings
            .Select(r => new HistoryPointDto
            {
                Timestamp = r.FechaHora,
                Depth10cm = r.Volumetrico1 ?? 0.0,
                Depth30cm = r.Volumetrico2 ?? 0.0,
            })
            .ToList();
    }
}
