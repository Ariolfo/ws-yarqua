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
    public DateTime UsuaFechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha de creación del registro.</summary>
    public DateTime UsuaFechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha de última actualización.</summary>
    public DateTime UsuaFechaActualizacion { get; set; } = DateTime.UtcNow;

    /// <summary>Indica si el usuario está activo.</summary>
    public bool UsuaActivo { get; set; } = true;
}
