using MediatR;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;
using Yarqua.Application.Services;

namespace Yarqua.Application.Features.Stations.Queries.GetNearbyStations;

/// <summary>
/// Consulta estaciones cercanas por Haversine sobre el catálogo estático.
/// </summary>
public class GetNearbyStationsQuery : IRequest<IReadOnlyList<StationDto>>
{
    /// <summary>Latitud del usuario.</summary>
    public double Lat { get; set; }

    /// <summary>Longitud del usuario.</summary>
    public double Lng { get; set; }

    /// <summary>Radio en km (default 50).</summary>
    public double Radius { get; set; } = 50;

    /// <summary>Si true, incluye sensores con lecturas cacheadas.</summary>
    public bool IncludeSensors { get; set; }
}

/// <summary>
/// Handler de estaciones cercanas.
/// </summary>
public class GetNearbyStationsQueryHandler : IRequestHandler<GetNearbyStationsQuery, IReadOnlyList<StationDto>>
{
    private readonly IVisualitiClient _visualiti;

    /// <summary>
    /// Inicializa el handler.
    /// </summary>
    public GetNearbyStationsQueryHandler(IVisualitiClient visualiti)
    {
        _visualiti = visualiti;
    }

    /// <summary>
    /// Obtiene estaciones dentro del radio.
    /// </summary>
    public async Task<IReadOnlyList<StationDto>> Handle(
        GetNearbyStationsQuery request,
        CancellationToken cancellationToken)
    {
        var radiusKm = Math.Clamp(request.Radius <= 0 ? 50 : request.Radius, 0.1, 500);
        var radiusM = radiusKm * 1000.0;

        var nearby = SensorCatalog.ListGeolocatedSensors()
            .Select(s => (Sensor: s, Dist: SensorCatalog.HaversineM(
                request.Lat, request.Lng, s.Latitud!.Value, s.Longitud!.Value)))
            .Where(x => x.Dist <= radiusM)
            .OrderBy(x => x.Dist)
            .ToList();

        var grouped = new Dictionary<string, (string Nombre, List<PhysicalSensor> Sensors, double MinDist)>(
            StringComparer.Ordinal);

        foreach (var (sensor, dist) in nearby)
        {
            var grupo = StationIds.GrupoFromSensor(sensor.Serial, sensor.Finca);
            if (!grouped.TryGetValue(grupo, out var entry))
            {
                var nombre = string.IsNullOrWhiteSpace(sensor.Finca)
                    ? $"Sensor {sensor.Serial}"
                    : grupo;
                grouped[grupo] = (nombre, [sensor], dist);
            }
            else
            {
                entry.Sensors.Add(sensor);
                grouped[grupo] = (entry.Nombre, entry.Sensors, Math.Min(entry.MinDist, dist));
            }
        }

        var stations = new List<StationDto>();
        foreach (var (grupo, entry) in grouped)
        {
            var sensors = entry.Sensors;
            stations.Add(new StationDto
            {
                Id = StationIds.EncodeStationId(grupo),
                Name = entry.Nombre,
                Latitude = Math.Round(sensors.Average(s => s.Latitud!.Value), 7),
                Longitude = Math.Round(sensors.Average(s => s.Longitud!.Value), 7),
                SensorCount = sensors.Sum(s => s.Canales),
                DistanceKm = Math.Round(entry.MinDist / 1000.0, 2),
                Sensors = [],
            });
        }

        stations = stations.OrderBy(s => s.DistanceKm ?? 0).ToList();

        if (!request.IncludeSensors)
        {
            return stations;
        }

        var allSerials = grouped.Values.SelectMany(e => e.Sensors.Select(s => s.Serial)).Distinct().ToList();
        var latest = new Dictionary<string, VisualitiReading?>(StringComparer.Ordinal);
        await Parallel.ForEachAsync(
            allSerials,
            new ParallelOptions { MaxDegreeOfParallelism = 8, CancellationToken = cancellationToken },
            async (serial, ct) =>
            {
                var reading = await _visualiti.GetLatestReadingCachedAsync(serial, ct);
                lock (latest)
                {
                    latest[serial] = reading;
                }
            });

        var sensorsByStation = new Dictionary<string, List<SensorDto>>(StringComparer.Ordinal);
        foreach (var (grupo, entry) in grouped)
        {
            var stationId = StationIds.EncodeStationId(grupo);
            sensorsByStation[stationId] = entry.Sensors
                .SelectMany(s => SensorMapper.ToLogicalSensorDtos(
                    s,
                    stationId,
                    latest.GetValueOrDefault(s.Serial)))
                .ToList();
        }

        foreach (var station in stations)
        {
            station.Sensors = sensorsByStation.GetValueOrDefault(station.Id) ?? [];
        }

        return stations;
    }
}
