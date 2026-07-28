namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Servicio de emisión y validación de JWT HS256.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Crea un token de acceso.
    /// </summary>
    /// <param name="userId">Identificador lógico del usuario.</param>
    /// <param name="name">Nombre para el claim name.</param>
    /// <returns>JWT firmado.</returns>
    string CreateAccessToken(string userId, string? name = null);

    /// <summary>
    /// Crea un token de refresco.
    /// </summary>
    /// <param name="userId">Identificador lógico del usuario.</param>
    /// <param name="name">Nombre para el claim name.</param>
    /// <returns>JWT firmado.</returns>
    string CreateRefreshToken(string userId, string? name = null);

    /// <summary>
    /// Valida un token y exige el tipo indicado (access|refresh).
    /// </summary>
    /// <param name="token">JWT a validar.</param>
    /// <param name="expectedType">Tipo esperado del claim type.</param>
    /// <returns>Claims principales: Sub y Name.</returns>
    (string Sub, string? Name) ValidateToken(string token, string expectedType);
}
