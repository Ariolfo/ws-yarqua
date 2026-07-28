namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Resultado de resolución geográfica a códigos de catálogo.
/// </summary>
public class ResolvedLocation
{
    /// <summary>Código de país (Pais_Id como texto).</summary>
    public string CodigoPais { get; set; } = string.Empty;

    /// <summary>Código de departamento.</summary>
    public string CodigoDepartamento { get; set; } = string.Empty;

    /// <summary>Código de ciudad (máx. 15).</summary>
    public string CodigoCiudad { get; set; } = string.Empty;

    /// <summary>Nombre canónico del país.</summary>
    public string PaisNombre { get; set; } = string.Empty;

    /// <summary>Nombre canónico del departamento.</summary>
    public string DepartamentoNombre { get; set; } = string.Empty;

    /// <summary>Nombre canónico de la ciudad.</summary>
    public string CiudadNombre { get; set; } = string.Empty;
}

/// <summary>
/// Resuelve nombres geográficos contra YarqtbPais/Departamento/Ciudad.
/// </summary>
public interface IGeoResolver
{
    /// <summary>
    /// Resuelve país, departamento y ciudad a códigos master.
    /// </summary>
    /// <param name="country">Nombre o id de país.</param>
    /// <param name="department">Nombre o código de departamento.</param>
    /// <param name="city">Nombre o código de ciudad.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Ubicación resuelta.</returns>
    Task<ResolvedLocation> ResolveAsync(
        string country,
        string department,
        string city,
        CancellationToken cancellationToken = default);
}
