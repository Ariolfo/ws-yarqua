namespace Yarqua.Domain.Entities;

/// <summary>
/// Catálogo de departamentos / provincias.
/// </summary>
public class YarqtbDepartamento
{
    /// <summary>Identificador del departamento.</summary>
    public int DepoId { get; set; }

    /// <summary>Identificador del país al que pertenece.</summary>
    public int PaisId { get; set; }

    /// <summary>Código textual del departamento.</summary>
    public string DepoCode { get; set; } = string.Empty;

    /// <summary>Nombre del departamento.</summary>
    public string DepoNombre { get; set; } = string.Empty;

    /// <summary>País navegación.</summary>
    public YarqtbPais? Pais { get; set; }

    /// <summary>Ciudades del departamento.</summary>
    public ICollection<YarqtbCiudad> Ciudades { get; set; } = new List<YarqtbCiudad>();
}
