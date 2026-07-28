using Yarqua.Domain.Entities;

namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Repositorio de usuarios (YarqtbUsuario).
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Busca un usuario por clave natural (nombre + códigos geo).
    /// </summary>
    Task<YarqtbUsuario?> FindAsync(
        string nombre,
        string codigoPais,
        string codigoDepartamento,
        string codigoCiudad,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un usuario nuevo (pendiente de SaveChanges).
    /// </summary>
    void Add(YarqtbUsuario usuario);
}
