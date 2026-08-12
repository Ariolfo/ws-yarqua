using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yarqua.Domain.Entities;
using Yarqua.Infrastructure.Identity;

namespace Yarqua.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core con soporte de Identity. Todas las tablas usan prefijo Yarqtb.
/// </summary>
public class YarquaDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    /// <summary>Inicializa el contexto.</summary>
    public YarquaDbContext(DbContextOptions<YarquaDbContext> options) : base(options)
    {
    }

    /// <summary>Países.</summary>
    public DbSet<YarqtbPais> Paises => Set<YarqtbPais>();

    /// <summary>Departamentos.</summary>
    public DbSet<YarqtbDepartamento> Departamentos => Set<YarqtbDepartamento>();

    /// <summary>Ciudades.</summary>
    public DbSet<YarqtbCiudad> Ciudades => Set<YarqtbCiudad>();

    /// <summary>Redes de sensores.</summary>
    public DbSet<YarqtbRed> Redes => Set<YarqtbRed>();

    /// <summary>Cultivos.</summary>
    public DbSet<YarqtbCultivo> Cultivos => Set<YarqtbCultivo>();

    /// <summary>Sensores.</summary>
    public DbSet<YarqtbSensor> Sensores => Set<YarqtbSensor>();

    /// <summary>Métodos para capacidad de campo.</summary>
    public DbSet<YarqtbMetodoCC> MetodosCC => Set<YarqtbMetodoCC>();

    /// <summary>Registros de la calculadora de riego.</summary>
    public DbSet<YarqtbCalculoRiego> CalculosRiego => Set<YarqtbCalculoRiego>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity configures its entities first
        base.OnModelCreating(modelBuilder);

        // Apply our EF configurations (picks up ApplicationUserConfiguration, etc.)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(YarquaDbContext).Assembly);

        // Rename remaining Identity tables to Yarqtb prefix
        modelBuilder.Entity<IdentityRole>().ToTable("YarqtbRol");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("YarqtbUsuarioRol");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("YarqtbUsuarioClaim");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("YarqtbUsuarioLogin");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("YarqtbUsuarioToken");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("YarqtbRolClaim");
    }
}
