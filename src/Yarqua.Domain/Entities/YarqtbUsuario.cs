namespace Yarqua.Domain.Entities;

/// <summary>
/// Usuario de la aplicación. Clave primaria compuesta sin id sustituto.
/// </summary>
public class YarqtbUsuario
{
    /// <summary>Nombre del usuario (parte de la PK).</summary>
    public string UsuaNombre { get; set; } = string.Empty;

    /// <summary>Código de país (parte de la PK).</summary>
    public string UsuaCodigoPais { get; set; } = string.Empty;

    /// <summary>Código de departamento (parte de la PK).</summary>
    public string UsuaCodigoDepartamento { get; set; } = string.Empty;

    /// <summary>Código de ciudad (parte de la PK).</summary>
    public string UsuaCodigoCiudad { get; set; } = string.Empty;

    /// <summary>Fecha de registro.</summary>
    public DateTimeOffset UsuaFechaRegistro { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Fecha de creación del registro.</summary>
    public DateTimeOffset UsuaFechaCreacion { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Fecha de última actualización.</summary>
    public DateTimeOffset UsuaFechaActualizacion { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Indica si el usuario está activo.</summary>
    public bool UsuaActivo { get; set; } = true;
}
