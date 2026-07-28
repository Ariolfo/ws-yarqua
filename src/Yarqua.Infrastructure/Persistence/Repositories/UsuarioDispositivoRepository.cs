using Microsoft.EntityFrameworkCore;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Domain.Entities;
using Yarqua.Infrastructure.Persistence;

namespace Yarqua.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio EF de dispositivos de usuario.
/// </summary>
public sealed class UsuarioDispositivoRepository : IUsuarioDispositivoRepository
{
    private readonly YarquaDbContext _db;

    /// <summary>
    /// Inicializa el repositorio.
    /// </summary>
    public UsuarioDispositivoRepository(YarquaDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<YarqtbUsuarioDispositivo?> FindByDeviceIdAsync(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        return _db.UsuarioDispositivos.FirstOrDefaultAsync(
            d => d.UdiDeviceId == deviceId,
            cancellationToken);
    }

    /// <inheritdoc />
    public void Add(YarqtbUsuarioDispositivo dispositivo) =>
        _db.UsuarioDispositivos.Add(dispositivo);
}
