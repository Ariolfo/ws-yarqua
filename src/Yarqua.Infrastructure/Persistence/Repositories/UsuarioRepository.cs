using Microsoft.EntityFrameworkCore;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Domain.Entities;
using Yarqua.Infrastructure.Persistence;

namespace Yarqua.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio EF de usuarios.
/// </summary>
public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly YarquaDbContext _db;

    /// <summary>Inicializa el repositorio.</summary>
    public UsuarioRepository(YarquaDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<YarqtbUsuario?> FindAsync(
        string nombre,
        int ciuId,
        CancellationToken cancellationToken = default)
    {
        return _db.Usuarios.FirstOrDefaultAsync(
            u => u.UsuaNombre == nombre && u.CiuId == ciuId,
            cancellationToken);
    }

    /// <inheritdoc />
    public void Add(YarqtbUsuario usuario) => _db.Usuarios.Add(usuario);
}
