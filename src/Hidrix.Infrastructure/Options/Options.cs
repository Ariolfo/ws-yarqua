namespace Hidrix.Infrastructure.Options;

/// <summary>
/// Opciones de JWT (sección Jwt).
/// </summary>
public class JwtOptions
{
    /// <summary>Nombre de sección en configuración.</summary>
    public const string SectionName = "Jwt";

    /// <summary>Secreto HS256.</summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>Minutos de vigencia del access token.</summary>
    public int AccessMinutes { get; set; } = 60;

    /// <summary>Días de vigencia del refresh token.</summary>
    public int RefreshDays { get; set; } = 30;
}

/// <summary>
/// Opciones del cliente Visualiti (sección Visualiti).
/// </summary>
public class VisualitiOptions
{
    /// <summary>Nombre de sección en configuración.</summary>
    public const string SectionName = "Visualiti";

    /// <summary>Habilita llamadas reales a Visualiti.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>URL de login.</summary>
    public string LoginUrl { get; set; } = "http://appgricultor.com/api/login";

    /// <summary>URL base de la API de datos.</summary>
    public string ApiUrl { get; set; } = "https://api.appgricultor.com";

    /// <summary>Cliente Visualiti.</summary>
    public string Cliente { get; set; } = string.Empty;

    /// <summary>Usuario Visualiti.</summary>
    public string Usuario { get; set; } = string.Empty;

    /// <summary>Contraseña Visualiti.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Verificar certificado SSL.</summary>
    public bool SslVerify { get; set; }
}
