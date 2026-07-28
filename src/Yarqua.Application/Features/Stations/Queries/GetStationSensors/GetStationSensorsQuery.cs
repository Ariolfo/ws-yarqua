using MediatR;
using Yarqua.Application.Common.Exceptions;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;
using Yarqua.Application.Services;

namespace Yarqua.Application.Features.Stations.Queries.GetStationSensors;

/// <summary>
/// Consulta sensores de una estación.
/// </summary>
public class GetStationSensorsQuery : IRequest<IReadOnlyList<SensorDto>>
{
    /// <summary>Id de estación (fin-... o sn-...).</summary>
    public string StationId { get; set; } = string.Empty;
}

/// <summary>
/// Handler de sensores por estación.
/// </summary>
public class GetStationSensorsQueryHandler : IRequestHandler<GetStationSensorsQuery, IReadOnlyList<SensorDto>>
{
    private readonly IVisualitiClient _visualiti;

    /// <summary>
    /// Inicializa el handler.
    /// </summary>
    public GetStationSensorsQueryHandler(IVisualitiClient visualiti)
    {
        _visualiti = visualiti;
    }

    /// <summary>
    /// Obtiene sensores de la estación con lecturas en vivo.
    /// </summary>
    public async Task<IReadOnlyList<SensorDto>> Handle(
        GetStationSensorsQuery request,
        CancellationToken cancellationToken)
    {
        string kind;
        string key;
        try
        {
            (kind, key) = StationIds.ParseStationId(request.StationId);
        }
        catch (ArgumentException)
        {
            throw new NotFoundException($"Estación no encontrada: {request.StationId}");
        }

        List<PhysicalSensor> sensors;
        string responseStationId;

        if (kind == "serial")
        {
            var sensor = SensorCatalog.GetSensor(key)
                         ?? throw new NotFoundException($"Estación no encontrada: {request.StationId}");
            sensors = [sensor];
            responseStationId = StationIds.EncodeStationId(key);
        }
        else
        {
            sensors = SensorCatalog.ListSensors()
                .Where(s => s.Finca is not null && StationIds.FincaSlug(s.Finca) == key)
                .ToList();
            if (sensors.Count == 0)
            {
                throw new NotFoundException($"Estación no encontrada: {request.StationId}");
            }

            responseStationId = request.StationId;
        }

        var latest = new Dictionary<string, VisualitiReading?>(StringComparer.Ordinal);
        await Parallel.ForEachAsync(
            sensors.Select(s => s.Serial),
            new ParallelOptions { MaxDegreeOfParallelism = 8, CancellationToken = cancellationToken },
            async (serial, ct) =>
            {
                var reading = await _visualiti.GetLatestReadingCachedAsync(serial, ct);
                lock (latest)
                {
                    latest[serial] = reading;
                }
            });

        return sensors
            .SelectMany(s => SensorMapper.ToLogicalSensorDtos(
                s,
                responseStationId,
                latest.GetValueOrDefault(s.Serial)))
            .ToList();
    }
}
