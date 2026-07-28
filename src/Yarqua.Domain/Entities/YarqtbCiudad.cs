namespace Yarqua.Domain.Entities;

/// <summary>
/// Catálogo de ciudades / municipios.
/// </summary>
public class YarqtbCiudad
{
    /// <summary>Identificador de la ciudad.</summary>
    public int CiuId { get; set; }

    /// <summary>Nombre de la ciudad.</summary>
    public string CiuNombre { get; set; } = string.Empty;

    /// <summary>Identificador del departamento.</summary>
    public int DepoId { get; set; }

    /// <summary>Identificador del país.</summary>
    public int PaisId { get; set; }

    /// <summary>Código de la ciudad.</summary>
    public string CiuCod { get; set; } = string.Empty;

    /// <summary>Código del departamento (desnormalizado).</summary>
    public string DepoCod { get; set; } = string.Empty;

    /// <summary>Departamento navegación.</summary>
    public YarqtbDepartamento? Departamento { get; set; }

    /// <summary>País navegación.</summary>
    public YarqtbPais? Pais { get; set; }
}
