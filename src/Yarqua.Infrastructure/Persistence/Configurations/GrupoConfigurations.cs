using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yarqua.Domain.Entities;

namespace Yarqua.Infrastructure.Persistence.Configurations;

/// <summary>Configuración EF de YarqtbGrupo.</summary>
public class YarqtbGrupoConfiguration : IEntityTypeConfiguration<YarqtbGrupo>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbGrupo> builder)
    {
        builder.ToTable("YarqtbGrupo");
        builder.HasKey(x => x.GrupId);
        builder.Property(x => x.GrupId).HasColumnName("Grup_Id").ValueGeneratedOnAdd();
        builder.Property(x => x.GrupNombre).HasColumnName("Grup_Nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.GrupDescripcion).HasColumnName("Grup_Descripcion").HasMaxLength(500);
        builder.Property(x => x.GrupRol).HasColumnName("Grup_Rol").HasMaxLength(50).IsRequired();
        builder.Property(x => x.GrupActivo).HasColumnName("Grup_Activo");
        builder.Property(x => x.GrupFechaCreacion).HasColumnName("Grup_FechaCreacion");
        builder.HasIndex(x => x.GrupNombre).IsUnique();
    }
}

/// <summary>Configuración EF de YarqtbUsuarioGrupo.</summary>
public class YarqtbUsuarioGrupoConfiguration : IEntityTypeConfiguration<YarqtbUsuarioGrupo>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbUsuarioGrupo> builder)
    {
        builder.ToTable("YarqtbUsuarioGrupo");
        builder.HasKey(x => new { x.UsuaId, x.GrupId });
        builder.Property(x => x.UsuaId).HasColumnName("Usua_Id").HasMaxLength(450).IsRequired();
        builder.Property(x => x.GrupId).HasColumnName("Grup_Id");
        builder.Property(x => x.UgrFechaAsignacion).HasColumnName("Ugr_FechaAsignacion");
        builder.HasOne(x => x.Grupo).WithMany(g => g.Miembros).HasForeignKey(x => x.GrupId);
    }
}
