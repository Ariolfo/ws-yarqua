/*
  Prepara la BD existente para ASP.NET Identity (migración EF IdentityMigration).
  - Elimina YarqtbUsuario antiguo (BIGINT) incompatible con Identity (NVARCHAR Id).
  - Elimina tablas legacy ya no usadas por el código actual.
  Los usuarios previos deben volver a registrarse (no había hash de contraseña).
  Autor: AGROSAVIA · Yarqua | 2026-08-12
*/
USE [dbYarqua];
GO

/* ---- Tablas legacy fuera del modelo actual ---- */
IF OBJECT_ID(N'dbo.YarqtbUsuarioDispositivo', N'U') IS NOT NULL
    DROP TABLE dbo.YarqtbUsuarioDispositivo;
GO

IF OBJECT_ID(N'dbo.YarqtbDevicePushToken', N'U') IS NOT NULL
    DROP TABLE dbo.YarqtbDevicePushToken;
GO

IF OBJECT_ID(N'dbo.YarqtbEventoUsuario', N'U') IS NOT NULL
    DROP TABLE dbo.YarqtbEventoUsuario;
GO

IF OBJECT_ID(N'dbo.YarqtbMetodoCC', N'U') IS NOT NULL
    DROP TABLE dbo.YarqtbMetodoCC;
GO

/* ---- Usuario antiguo (solo si aún es BIGINT / sin columnas Identity) ---- */
IF OBJECT_ID(N'dbo.YarqtbUsuario', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.YarqtbUsuario', N'PasswordHash') IS NULL
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'';

    SELECT @sql = @sql + N'ALTER TABLE dbo.YarqtbUsuario DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
    FROM sys.foreign_keys fk
    WHERE fk.parent_object_id = OBJECT_ID(N'dbo.YarqtbUsuario');

    SELECT @sql = @sql + N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id))
        + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id))
        + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
    FROM sys.foreign_keys fk
    WHERE fk.referenced_object_id = OBJECT_ID(N'dbo.YarqtbUsuario');

    IF LEN(@sql) > 0
        EXEC sp_executesql @sql;

    DROP TABLE dbo.YarqtbUsuario;
END
GO

/* Identity tables will be created by: dotnet ef database update */
PRINT N'OK: BD lista para IdentityMigration.';
GO
