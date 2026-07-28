using Yarqua.Application.Common.Interfaces;
using Yarqua.Infrastructure.Persistence;

namespace Yarqua.Infrastructure.Persistence;

/// <summary>
/// Unidad de trabajo sobre <see cref="YarquaDbContext"/>.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly YarquaDbContext _db;

    /// <summary>
    /// Inicializa la unidad de trabajo.
    /// </summary>
    public UnitOfWork(YarquaDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default) =>
        _db.Database.CanConnectAsync(cancellationToken);
}
