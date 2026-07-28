/*
  Ajuste de nomenclatura: renombra Yarqua → dbYarqua y aplica collation Modern_Spanish_CI_AS.
  Los CHECK dependen de la collation; se recrean tras el cambio.
*/
SET NOCOUNT ON;

-- 1) Renombrar si aún existe el nombre antiguo
IF DB_ID(N'dbYarqua') IS NULL AND DB_ID(N'Yarqua') IS NOT NULL
BEGIN
    ALTER DATABASE [Yarqua] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    ALTER DATABASE [Yarqua] MODIFY NAME = [dbYarqua];
    ALTER DATABASE [dbYarqua] SET MULTI_USER;
    PRINT N'Renombrada Yarqua → dbYarqua.';
END
GO

IF DB_ID(N'dbYarqua') IS NULL
BEGIN
    PRINT N'dbYarqua no existe; ejecutar 001_create_database.sql.';
END
ELSE IF EXISTS (
    SELECT 1 FROM sys.databases
    WHERE name = N'dbYarqua' AND collation_name = N'Modern_Spanish_CI_AS'
)
BEGIN
    PRINT N'dbYarqua ya tiene collation Modern_Spanish_CI_AS.';
END
ELSE
BEGIN
    ALTER DATABASE [dbYarqua] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    -- Quitar CHECK dependientes de collation
    IF OBJECT_ID(N'dbYarqua.dbo.CK_YarqtbEvento_Tipo', N'C') IS NOT NULL
        ALTER TABLE dbYarqua.dbo.YarqtbEventoUsuario DROP CONSTRAINT CK_YarqtbEvento_Tipo;
    IF OBJECT_ID(N'dbYarqua.dbo.CK_YarqtbDpt_Platform', N'C') IS NOT NULL
        ALTER TABLE dbYarqua.dbo.YarqtbDevicePushToken DROP CONSTRAINT CK_YarqtbDpt_Platform;
    IF OBJECT_ID(N'dbYarqua.dbo.CK_YarqtbUdi_Platform', N'C') IS NOT NULL
        ALTER TABLE dbYarqua.dbo.YarqtbUsuarioDispositivo DROP CONSTRAINT CK_YarqtbUdi_Platform;

    ALTER DATABASE [dbYarqua] COLLATE Modern_Spanish_CI_AS;

    -- Restaurar CHECK
    IF OBJECT_ID(N'dbYarqua.dbo.YarqtbEventoUsuario', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbYarqua.dbo.CK_YarqtbEvento_Tipo', N'C') IS NULL
        ALTER TABLE dbYarqua.dbo.YarqtbEventoUsuario WITH CHECK
            ADD CONSTRAINT CK_YarqtbEvento_Tipo
            CHECK (Even_Evento IN (N'REGISTRO', N'ACCESO', N'CONSULTA'));

    IF OBJECT_ID(N'dbYarqua.dbo.YarqtbDevicePushToken', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbYarqua.dbo.CK_YarqtbDpt_Platform', N'C') IS NULL
        ALTER TABLE dbYarqua.dbo.YarqtbDevicePushToken WITH CHECK
            ADD CONSTRAINT CK_YarqtbDpt_Platform
            CHECK (Dpt_Platform IN (N'android', N'ios', N'local'));

    IF OBJECT_ID(N'dbYarqua.dbo.YarqtbUsuarioDispositivo', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbYarqua.dbo.CK_YarqtbUdi_Platform', N'C') IS NULL
        ALTER TABLE dbYarqua.dbo.YarqtbUsuarioDispositivo WITH CHECK
            ADD CONSTRAINT CK_YarqtbUdi_Platform
            CHECK (Udi_Platform IN (N'android', N'ios', N'web', N'unknown'));

    ALTER DATABASE [dbYarqua] SET MULTI_USER;
    PRINT N'Collation de dbYarqua actualizada a Modern_Spanish_CI_AS.';
END
GO
