using Yarqua.Domain.Entities;

namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Repositorio de eventos de usuario (YarqtbEventoUsuario).
/// </summary>
public interface IEventoUsuarioRepository
{
    /// <summary>
    /// Agrega un evento de auditoría (pendiente de SaveChanges).
    /// </summary>
    void Add(YarqtbEventoUsuario evento);
}
