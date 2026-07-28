using FluentAssertions;
using Yarqua.Application.Services;

namespace Yarqua.Application.Tests;

/// <summary>
/// Pruebas de interpretación de estado por CC del cultivo.
/// </summary>
public class MoistureStatusInterpreterTests
{
    [Fact]
    public void Interpret_Null_IsNoData()
    {
        var (status, alert) = MoistureStatusInterpreter.Interpret(null, "Cacao");
        status.Should().Be("no_data");
        alert.Should().BeNull();
    }

    [Theory]
    [InlineData(48.0, "excess")]          // > CC cacao 34 %
    [InlineData(34.0, "attention_high")]  // exactamente CC
    [InlineData(30.0, "attention_high")]  // 80 % CC–CC
    [InlineData(27.2, "attention_high")]  // límite 80 % CC
    [InlineData(25.0, "irrigate")]        // 64 % CC–80 % CC
    [InlineData(21.76, "irrigate")]
    [InlineData(15.0, "attention_low")]   // 10 %–64 % CC
    [InlineData(10.0, "attention_low")]
    [InlineData(5.0, "deficit")]          // < 10 %
    public void Interpret_Cacao_UsesFieldCapacityBands(double value, string expected)
    {
        var (status, _) = MoistureStatusInterpreter.Interpret(value, "Cacao");
        status.Should().Be(expected);
    }

    [Fact]
    public void Interpret_Aguacate_ExcessAboveCc39()
    {
        var (status, alert) = MoistureStatusInterpreter.Interpret(40, "Aguacate");
        status.Should().Be("excess");
        alert.Should().Be("EXCESO");
    }

    [Fact]
    public void Interpret_Lima_28_IsIrrigate()
    {
        // 28 % Lima: 64 % CC=23.04, 80 % CC=28.8 → Regar
        var (status, alert) = MoistureStatusInterpreter.Interpret(28, "Lima ácida Tahiti");
        status.Should().Be("irrigate");
        alert.Should().Be("REGAR");
    }

    [Fact]
    public void Resolve_LimaAcidaTahiti_UsesLimaProfile()
    {
        CropMoistureProfiles.Resolve("Lima ácida Tahiti").Name.Should().Be("Lima");
    }
}
