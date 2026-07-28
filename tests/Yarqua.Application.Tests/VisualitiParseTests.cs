using System.Text.Json;
using FluentAssertions;
using Yarqua.Infrastructure.Services;

namespace Yarqua.Application.Tests;

/// <summary>
/// Pruebas del parseo de respuestas Visualiti.
/// </summary>
public class VisualitiParseTests
{
    [Theory]
    [InlineData("2026-07-15 9:00", 2026, 7, 15, 9, 0)]
    [InlineData("2026-07-15 09:00", 2026, 7, 15, 9, 0)]
    [InlineData("2026-07-15 9:00:00", 2026, 7, 15, 9, 0)]
    public void ParseReadingTimestamp_AcceptsSingleDigitHour(
        string raw, int y, int m, int d, int h, int min)
    {
        var ts = VisualitiClient.ParseReadingTimestamp(raw);
        ts.UtcDateTime.Year.Should().Be(y);
        ts.UtcDateTime.Month.Should().Be(m);
        ts.UtcDateTime.Day.Should().Be(d);
        ts.UtcDateTime.Hour.Should().Be(h);
        ts.UtcDateTime.Minute.Should().Be(min);
    }

    [Fact]
    public void ParsePayload_NotDataFound_ReturnsEmpty()
    {
        using var doc = JsonDocument.Parse("""{"device":"SUELO M316","detail":"Not Data Found"}""");
        VisualitiClient.ParsePayload(doc.RootElement).Should().BeEmpty();
    }

    [Fact]
    public void ParsePayload_VolumetricoChannels_ConvertsToPercent()
    {
        const string json = """
        {
          "device": "AGUACATE2 SUELO M316",
          "data": {
            "2026-07-15 9:00": [
              { "time": "2026-07-15 9:00", "sensor": "Volumetrico1", "value": 0.55, "medida": null },
              { "time": "2026-07-15 9:00", "sensor": "Volumetrico2", "value": 0.36, "medida": null }
            ]
          }
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var rows = VisualitiClient.ParsePayload(doc.RootElement);
        rows.Should().HaveCount(1);
        rows[0].FechaHora.UtcDateTime.Hour.Should().Be(9);
        rows[0].Valores[1].Should().BeApproximately(55.0, 0.01);
        rows[0].Valores[2].Should().BeApproximately(36.0, 0.01);
    }

    [Fact]
    public void ParseVisualitiStationId_LogicalId_Works()
    {
        VisualitiClient.ParseVisualitiStationId("M316-1").Should().Be(316);
        VisualitiClient.ParseVisualitiStationId("M316").Should().Be(316);
    }
}
