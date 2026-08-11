namespace Yarqua.Domain.Entities;

/// <summary>
/// Relación muchos-a-muchos entre usuario y grupo.
/// </summary>
public class YarqtbUsuarioGrupo
{
    /// <summary>FK al Id de ApplicationUser (GUID string).</summary>
    public string UsuaId { get; set; } = string.Empty;

    /// <summary>FK al grupo.</summary>
    public int GrupId { get; set; }

    /// <summary>Fecha de asignación.</summary>
    public DateTime UgrFechaAsignacion { get; set; } = DateTime.UtcNow;

    /// <summary>Navegación al grupo.</summary>
    public YarqtbGrupo Grupo { get; set; } = null!;
}
