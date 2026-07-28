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
        builder.ToTable("YarqtbPais");
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
        builder.ToTable("YarqtbDepartamento");
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
        builder.ToTable("YarqtbCiudad");
        builder.HasKey(x => x.CiuId);
        builder.Property(x => x.CiuId).HasColumnName("Ciu_Id");
        builder.Property(x => x.CiuNombre).HasColumnName("Ciu_Nombre").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DepoId).HasColumnName("Depo_Id");
        builder.Property(x => x.PaisId).HasColumnName("Pais_Id");
        builder.Property(x => x.CiuCod).HasColumnName("Ciu_Cod").HasMaxLength(50).IsRequired();
        builder.Property(x => x.DepoCod).HasColumnName("Depo_Cod").HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.CiuCod, x.DepoCod, x.PaisId }).IsUnique();
        builder.HasOne(x => x.Departamento).WithMany(d => d.Ciudades).HasForeignKey(x => x.DepoId);
        builder.HasOne(x => x.Pais).WithMany(p => p.Ciudades).HasForeignKey(x => x.PaisId);
    }
}

/// <summary>Configuración EF de YarqtbUsuario.</summary>
public class YarqtbUsuarioConfiguration : IEntityTypeConfiguration<YarqtbUsuario>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbUsuario> builder)
    {
        builder.ToTable("YarqtbUsuario");
        builder.HasKey(x => new { x.UsuaNombre, x.UsuaCodigoPais, x.UsuaCodigoDepartamento, x.UsuaCodigoCiudad });
        builder.Property(x => x.UsuaNombre).HasColumnName("Usua_Nombre").HasMaxLength(150).IsRequired();
        builder.Property(x => x.UsuaCodigoPais).HasColumnName("Usua_CodigoPais").HasMaxLength(3).IsRequired();
        builder.Property(x => x.UsuaCodigoDepartamento).HasColumnName("Usua_CodigoDepartamento").HasMaxLength(10).IsRequired();
        builder.Property(x => x.UsuaCodigoCiudad).HasColumnName("Usua_CodigoCiudad").HasMaxLength(15).IsRequired();
        builder.Property(x => x.UsuaFechaRegistro).HasColumnName("Usua_FechaRegistro");
        builder.Property(x => x.UsuaFechaCreacion).HasColumnName("Usua_FechaCreacion");
        builder.Property(x => x.UsuaFechaActualizacion).HasColumnName("Usua_FechaActualizacion");
        builder.Property(x => x.UsuaActivo).HasColumnName("Usua_Activo");
    }
}

/// <summary>Configuración EF de YarqtbEventoUsuario.</summary>
public class YarqtbEventoUsuarioConfiguration : IEntityTypeConfiguration<YarqtbEventoUsuario>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbEventoUsuario> builder)
    {
        builder.ToTable("YarqtbEventoUsuario");
        builder.HasKey(x => x.EvenId);
        builder.Property(x => x.EvenId).HasColumnName("Even_Id").ValueGeneratedOnAdd();
        builder.Property(x => x.UsuaNombre).HasColumnName("Usua_Nombre").HasMaxLength(150).IsRequired();
        builder.Property(x => x.EvenFecha).HasColumnName("Even_Fecha");
        builder.Property(x => x.EvenHora).HasColumnName("Even_Hora");
        builder.Property(x => x.EvenEvento).HasColumnName("Even_Evento").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EvenSensorId).HasColumnName("Even_SensorId").HasMaxLength(20);
        builder.Property(x => x.EvenFechaCreacion).HasColumnName("Even_FechaCreacion");
        builder.Property(x => x.EvenFechaActualizacion).HasColumnName("Even_FechaActualizacion");
    }
}

/// <summary>Configuración EF de YarqtbDevicePushToken.</summary>
public class YarqtbDevicePushTokenConfiguration : IEntityTypeConfiguration<YarqtbDevicePushToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbDevicePushToken> builder)
    {
        builder.ToTable("YarqtbDevicePushToken");
        builder.HasKey(x => x.DptId);
        builder.Property(x => x.DptId).HasColumnName("Dpt_Id").ValueGeneratedOnAdd();
        builder.Property(x => x.UsuaId).HasColumnName("Usua_Id").HasMaxLength(64).IsRequired();
        builder.Property(x => x.DptPushToken).HasColumnName("Dpt_PushToken").HasMaxLength(512).IsRequired();
        builder.Property(x => x.DptPlatform).HasColumnName("Dpt_Platform").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DptActivo).HasColumnName("Dpt_Activo");
        builder.Property(x => x.DptFechaRegistro).HasColumnName("Dpt_FechaRegistro");
        builder.Property(x => x.DptFechaActualizacion).HasColumnName("Dpt_FechaActualizacion");
        builder.HasIndex(x => x.DptPushToken).IsUnique();
    }
}

/// <summary>Configuración EF de YarqtbUsuarioDispositivo.</summary>
public class YarqtbUsuarioDispositivoConfiguration : IEntityTypeConfiguration<YarqtbUsuarioDispositivo>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<YarqtbUsuarioDispositivo> builder)
    {
        builder.ToTable("YarqtbUsuarioDispositivo");
        builder.HasKey(x => x.UdiDeviceId);
        builder.Property(x => x.UdiDeviceId).HasColumnName("Udi_DeviceId").HasMaxLength(64);
        builder.Property(x => x.UsuaId).HasColumnName("Usua_Id").HasMaxLength(64).IsRequired();
        builder.Property(x => x.UdiPlatform).HasColumnName("Udi_Platform").HasMaxLength(16).IsRequired();
        builder.Property(x => x.UdiFechaRegistro).HasColumnName("Udi_FechaRegistro");
        builder.Property(x => x.UdiFechaActualizacion).HasColumnName("Udi_FechaActualizacion");
        builder.Property(x => x.UdiActivo).HasColumnName("Udi_Activo");
    }
}
