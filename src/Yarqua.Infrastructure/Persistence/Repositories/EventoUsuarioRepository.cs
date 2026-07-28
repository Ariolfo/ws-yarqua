using Yarqua.Application.Common.Interfaces;
using Yarqua.Domain.Entities;
using Yarqua.Infrastructure.Persistence;

namespace Yarqua.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio EF de eventos de usuario.
/// </summary>
public sealed class EventoUsuarioRepository : IEventoUsuarioRepository
{
    private readonly YarquaDbContext _db;

    /// <summary>
    /// Inicializa el repositorio.
    /// </summary>
    public EventoUsuarioRepository(YarquaDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public void Add(YarqtbEventoUsuario evento) => _db.EventosUsuario.Add(evento);
}
