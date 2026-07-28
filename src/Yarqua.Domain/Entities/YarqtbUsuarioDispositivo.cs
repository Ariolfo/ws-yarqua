namespace Yarqua.Domain.Entities;

/// <summary>
/// Vinculación usuario ↔ dispositivo móvil.
/// </summary>
public class YarqtbUsuarioDispositivo
{
    /// <summary>Identificador estable del dispositivo (PK).</summary>
    public string UdiDeviceId { get; set; } = string.Empty;

    /// <summary>Identificador lógico del usuario (u-...).</summary>
    public string UsuaId { get; set; } = string.Empty;

    /// <summary>Plataforma: android, ios, web o unknown.</summary>
    public string UdiPlatform { get; set; } = "android";

    /// <summary>Fecha de registro.</summary>
    public DateTimeOffset UdiFechaRegistro { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Fecha de actualización.</summary>
    public DateTimeOffset UdiFechaActualizacion { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Indica si el vínculo está activo.</summary>
    public bool UdiActivo { get; set; } = true;
}
