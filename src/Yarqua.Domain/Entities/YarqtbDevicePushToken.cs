namespace Yarqua.Domain.Entities;

/// <summary>
/// Token push FCM/APNs asociado a un usuario.
/// </summary>
public class YarqtbDevicePushToken
{
    /// <summary>Identificador del token.</summary>
    public long DptId { get; set; }

    /// <summary>Identificador lógico del usuario (u-...).</summary>
    public string UsuaId { get; set; } = string.Empty;

    /// <summary>Token push del dispositivo.</summary>
    public string DptPushToken { get; set; } = string.Empty;

    /// <summary>Plataforma: android, ios o local.</summary>
    public string DptPlatform { get; set; } = "android";

    /// <summary>Indica si el token está activo.</summary>
    public bool DptActivo { get; set; } = true;

    /// <summary>Fecha de registro.</summary>
    public DateTime DptFechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha de actualización.</summary>
    public DateTime DptFechaActualizacion { get; set; } = DateTime.UtcNow;
}
