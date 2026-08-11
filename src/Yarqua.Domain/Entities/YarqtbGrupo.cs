namespace Yarqua.Domain.Entities;

/// <summary>
/// Grupo de usuarios con un rol asignado.
/// </summary>
public class YarqtbGrupo
{
    /// <summary>Identificador del grupo.</summary>
    public int GrupId { get; set; }

    /// <summary>Nombre del grupo.</summary>
    public string GrupNombre { get; set; } = string.Empty;

    /// <summary>Descripción del grupo.</summary>
    public string? GrupDescripcion { get; set; }

    /// <summary>Rol Identity asociado (Admin | Operador | Visualizador).</summary>
    public string GrupRol { get; set; } = string.Empty;

    /// <summary>Indica si el grupo está activo.</summary>
    public bool GrupActivo { get; set; } = true;

    /// <summary>Fecha de creación.</summary>
    public DateTime GrupFechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>Miembros del grupo.</summary>
    public ICollection<YarqtbUsuarioGrupo> Miembros { get; set; } = [];
}
