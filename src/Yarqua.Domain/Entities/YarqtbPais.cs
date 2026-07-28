namespace Yarqua.Domain.Entities;

/// <summary>
/// Catálogo de países.
/// </summary>
public class YarqtbPais
{
    /// <summary>Identificador numérico del país.</summary>
    public int PaisId { get; set; }

    /// <summary>Nombre del país.</summary>
    public string PaisNombre { get; set; } = string.Empty;

    /// <summary>Estado del registro (por defecto 2).</summary>
    public int PaisEstado { get; set; } = 2;

    /// <summary>Departamentos asociados al país.</summary>
    public ICollection<YarqtbDepartamento> Departamentos { get; set; } = new List<YarqtbDepartamento>();

    /// <summary>Ciudades asociadas al país.</summary>
    public ICollection<YarqtbCiudad> Ciudades { get; set; } = new List<YarqtbCiudad>();
}
