namespace Yarqua.Domain.Entities;

/// <summary>
/// Usuario de la aplicación. Referencia geográfica solo a Ciudad.
/// </summary>
public class YarqtbUsuario
{
    /// <summary>Identificador sustituto.</summary>
    public long UsuaId { get; set; }

    /// <summary>Nombre del usuario.</summary>
    public string UsuaNombre { get; set; } = string.Empty;

    /// <summary>Ciudad de residencia (única FK geográfica).</summary>
    public int CiuId { get; set; }

    /// <summary>Fecha de registro.</summary>
    public DateTime UsuaFechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha de creación del registro.</summary>
    public DateTime UsuaFechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha de última actualización.</summary>
    public DateTime UsuaFechaActualizacion { get; set; } = DateTime.UtcNow;

    /// <summary>Indica si el usuario está activo.</summary>
    public bool UsuaActivo { get; set; } = true;

    /// <summary>Ciudad navegación.</summary>
    public YarqtbCiudad? Ciudad { get; set; }
}
