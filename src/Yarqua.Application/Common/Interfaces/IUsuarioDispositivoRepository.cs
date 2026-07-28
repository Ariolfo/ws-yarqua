using Yarqua.Domain.Entities;

namespace Yarqua.Application.Common.Interfaces;

/// <summary>
/// Repositorio de dispositivos vinculados (YarqtbUsuarioDispositivo).
/// </summary>
public interface IUsuarioDispositivoRepository
{
    /// <summary>
    /// Busca vínculo por id estable de dispositivo.
    /// </summary>
    Task<YarqtbUsuarioDispositivo?> FindByDeviceIdAsync(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un vínculo dispositivo–usuario (pendiente de SaveChanges).
    /// </summary>
    void Add(YarqtbUsuarioDispositivo dispositivo);
}
