using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yarqua.Domain.Entities;

namespace Yarqua.Infrastructure.Persistence.Configurations;

/// <summary>Configuración EF de YarqtbPais.</summary>
public class YarqtbPaisConfiguration : IEntityTypeConfiguration<YarqtbPais>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbPais> builder)
    {
        builder.ToTable("YarqtbPais", t => t.ExcludeFromMigrations());
        builder.HasKey(x => x.PaisId);
        builder.Property(x => x.PaisId).HasColumnName("Pais_Id");
        builder.Property(x => x.PaisNombre).HasColumnName("Pais_Nombre").HasMaxLength(255).IsRequired();
        builder.Property(x => x.PaisEstado).HasColumnName("Pais_Estado").HasDefaultValue(2);
    }
}

/// <summary>Configuración EF de YarqtbDepartamento.</summary>
public class YarqtbDepartamentoConfiguration : IEntityTypeConfiguration<YarqtbDepartamento>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbDepartamento> builder)
    {
        builder.ToTable("YarqtbDepartamento", t => t.ExcludeFromMigrations());
        builder.HasKey(x => x.DepoId);
        builder.Property(x => x.DepoId).HasColumnName("Depo_Id");
        builder.Property(x => x.PaisId).HasColumnName("Pais_Id");
        builder.Property(x => x.DepoCode).HasColumnName("Depo_Code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.DepoNombre).HasColumnName("Depo_Nombre").HasMaxLength(255).IsRequired();
        builder.HasIndex(x => new { x.PaisId, x.DepoCode }).IsUnique();
        builder.HasOne(x => x.Pais).WithMany(p => p.Departamentos).HasForeignKey(x => x.PaisId);
    }
}

/// <summary>Configuración EF de YarqtbCiudad.</summary>
public class YarqtbCiudadConfiguration : IEntityTypeConfiguration<YarqtbCiudad>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbCiudad> builder)
    {
        builder.ToTable("YarqtbCiudad", t => t.ExcludeFromMigrations());
        builder.HasKey(x => x.CiuId);
        builder.Property(x => x.CiuId).HasColumnName("Ciu_Id");
        builder.Property(x => x.CiuNombre).HasColumnName("Ciu_Nombre").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DepoId).HasColumnName("Depo_Id");
        builder.Property(x => x.CiuCod).HasColumnName("Ciu_Cod").HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.DepoId, x.CiuCod }).IsUnique();
        builder.HasOne(x => x.Departamento).WithMany(d => d.Ciudades).HasForeignKey(x => x.DepoId);
    }
}
