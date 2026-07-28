using Microsoft.EntityFrameworkCore;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;
using Yarqua.Domain.Entities;
using Yarqua.Infrastructure.Persistence;

namespace Yarqua.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio EF del catálogo geográfico.
/// </summary>
public sealed class GeoRepository : IGeoRepository
{
    private readonly YarquaDbContext _db;

    /// <summary>
    /// Inicializa el repositorio.
    /// </summary>
    public GeoRepository(YarquaDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GeoCountryDto>> GetCountriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Paises
            .AsNoTracking()
            .OrderBy(p => p.PaisNombre)
            .Select(p => new GeoCountryDto { Id = p.PaisId, Name = p.PaisNombre })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GeoDepartmentDto>> GetDepartmentsByPaisIdAsync(
        int paisId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Departamentos
            .AsNoTracking()
            .Where(d => d.PaisId == paisId)
            .OrderBy(d => d.DepoNombre)
            .Select(d => new GeoDepartmentDto
            {
                Id = d.DepoId,
                Code = d.DepoCode,
                Name = d.DepoNombre,
                PaisId = d.PaisId,
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GeoCityDto>> GetCitiesByDepoIdAsync(
        int depoId,
        string? query,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Ciudades.AsNoTracking().Where(c => c.DepoId == depoId);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            q = q.Where(c =>
                EF.Functions.Like(c.CiuNombre, $"%{term}%") ||
                EF.Functions.Like(c.CiuCod, $"%{term}%"));
        }

        return await q
            .OrderBy(c => c.CiuNombre)
            .Take(limit)
            .Select(c => new GeoCityDto
            {
                Id = c.CiuId,
                Code = c.CiuCod,
                Name = c.CiuNombre,
                DepoId = c.DepoId,
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<YarqtbPais>> ListPaisesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Paises.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<YarqtbDepartamento>> ListDepartamentosByPaisIdAsync(
        int paisId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Departamentos
            .AsNoTracking()
            .Where(d => d.PaisId == paisId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<YarqtbCiudad>> ListCiudadesByPaisAndDepoAsync(
        int paisId,
        string depoCode,
        CancellationToken cancellationToken = default)
    {
        return await _db.Ciudades
            .AsNoTracking()
            .Where(c => c.PaisId == paisId && c.DepoCod == depoCode)
            .ToListAsync(cancellationToken);
    }
}
