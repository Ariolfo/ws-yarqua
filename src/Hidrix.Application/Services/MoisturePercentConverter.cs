namespace Hidrix.Application.Services;

/// <summary>
/// Conversión de valores Visualiti a porcentaje de humedad.
/// </summary>
public static class MoisturePercentConverter
{
    /// <summary>
    /// Si el valor es fracción (≤ 1.5) lo multiplica por 100; si no, lo redondea.
    /// </summary>
    /// <param name="value">Valor crudo de Visualiti.</param>
    /// <returns>Porcentaje con 2 decimales.</returns>
    public static double ToPercent(double value)
    {
        if (value is >= 0 and <= 1.5)
        {
            return Math.Round(value * 100.0, 2);
        }

        return Math.Round(value, 2);
    }
}

/// <summary>
/// Interpretación de humedad según capacidad de campo (CC) del cultivo.
/// Estados: excess, attention_high, irrigate, attention_low, deficit.
/// </summary>
public static class MoistureStatusInterpreter
{
    /// <summary>
    /// Interpreta el estado a partir de la humedad y el cultivo.
    /// </summary>
    /// <param name="value">Porcentaje o null.</param>
    /// <param name="cultivo">Cultivo del sensor (para resolver CC).</param>
    /// <returns>Status y mensaje de alerta (sin prefijo «ALERTA:»).</returns>
    public static (string Status, string? AlertMessage) Interpret(
        double? value,
        string? cultivo = null)
    {
        if (value is null)
        {
            return ("no_data", null);
        }

        var profile = CropMoistureProfiles.Resolve(cultivo);
        return Interpret(value.Value, profile);
    }

    /// <summary>
    /// Interpreta el estado con un perfil CC explícito.
    /// </summary>
    /// <param name="value">Porcentaje de humedad.</param>
    /// <param name="profile">Perfil del cultivo.</param>
    /// <returns>Status y mensaje de alerta.</returns>
    public static (string Status, string? AlertMessage) Interpret(
        double value,
        CropMoistureProfile profile)
    {
        // Exceso: por encima de la capacidad de campo.
        if (value > profile.FieldCapacity)
        {
            return ("excess", "EXCESO");
        }

        // Atención (arriba de CC): 80 % CC ≤ humedad ≤ CC.
        if (value >= profile.MaxIrrigationLimit)
        {
            return ("attention_high", "ATENCIÓN: HUMEDAD ARRIBA DE CC");
        }

        // Regar: 64 % CC ≤ humedad &lt; 80 % CC.
        if (value >= profile.IrrigationDecision)
        {
            return ("irrigate", "REGAR");
        }

        // Atención (abajo de CC): 10 % ≤ humedad &lt; 64 % CC.
        if (value >= CropMoistureProfiles.SoilFloor)
        {
            return ("attention_low", "ATENCIÓN: HUMEDAD ABAJO DE CC");
        }

        // Déficit: por debajo del 10 % de humedad de suelo.
        return ("deficit", "DÉFICIT");
    }
}
