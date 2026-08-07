using Yarqua.Domain.Entities;

namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Repositorio de usuarios (YarqtbUsuario).
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Busca un usuario por nombre + ciudad.
    /// </summary>
    Task<YarqtbUsuario?> FindAsync(
        string nombre,
        int ciuId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un usuario nuevo (pendiente de SaveChanges).
    /// </summary>
    void Add(YarqtbUsuario usuario);
}
