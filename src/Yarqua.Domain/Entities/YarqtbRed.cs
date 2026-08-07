namespace Yarqua.Domain.Entities;

/// <summary>
/// Red de sensores vinculada a un país.
/// </summary>
public class YarqtbRed
{
    /// <summary>Identificador de la red.</summary>
    public int RedId { get; set; }

    /// <summary>Nombre (RED ASORUT, RED ECUADOR, RED HONDURA).</summary>
    public string RedNombre { get; set; } = string.Empty;

    /// <summary>País asociado.</summary>
    public int PaisId { get; set; }

    /// <summary>Navegación al país.</summary>
    public YarqtbPais? Pais { get; set; }

    /// <summary>Sensores de la red.</summary>
    public ICollection<YarqtbSensor> Sensores { get; set; } = new List<YarqtbSensor>();
}
