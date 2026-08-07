using Microsoft.EntityFrameworkCore;
using Yarqua.Domain.Entities;

namespace Yarqua.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core mapeado a las tablas Yarqtb* (uso interno de repositorios).
/// </summary>
public class YarquaDbContext : DbContext
{
    /// <summary>
    /// Inicializa el contexto.
    /// </summary>
    /// <param name="options">Opciones de EF Core.</param>
    public YarquaDbContext(DbContextOptions<YarquaDbContext> options) : base(options)
    {
    }

    /// <summary>Países.</summary>
    public DbSet<YarqtbPais> Paises => Set<YarqtbPais>();

    /// <summary>Departamentos.</summary>
    public DbSet<YarqtbDepartamento> Departamentos => Set<YarqtbDepartamento>();

    /// <summary>Ciudades.</summary>
    public DbSet<YarqtbCiudad> Ciudades => Set<YarqtbCiudad>();

    /// <summary>Usuarios.</summary>
    public DbSet<YarqtbUsuario> Usuarios => Set<YarqtbUsuario>();

    /// <summary>Eventos de usuario.</summary>
    public DbSet<YarqtbEventoUsuario> EventosUsuario => Set<YarqtbEventoUsuario>();

    /// <summary>Tokens push.</summary>
    public DbSet<YarqtbDevicePushToken> DevicePushTokens => Set<YarqtbDevicePushToken>();

    /// <summary>Dispositivos vinculados.</summary>
    public DbSet<YarqtbUsuarioDispositivo> UsuarioDispositivos => Set<YarqtbUsuarioDispositivo>();

    /// <summary>Redes de sensores.</summary>
    public DbSet<YarqtbRed> Redes => Set<YarqtbRed>();

    /// <summary>Cultivos.</summary>
    public DbSet<YarqtbCultivo> Cultivos => Set<YarqtbCultivo>();

    /// <summary>Sensores.</summary>
    public DbSet<YarqtbSensor> Sensores => Set<YarqtbSensor>();

    /// <summary>Métodos para capacidad de campo.</summary>
    public DbSet<YarqtbMetodoCC> MetodosCC => Set<YarqtbMetodoCC>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(YarquaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
