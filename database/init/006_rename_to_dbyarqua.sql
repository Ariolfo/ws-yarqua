/*
  Ajuste de nomenclatura: renombra Yarqua → dbYarqua y aplica collation Modern_Spanish_CI_AS.
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

    ALTER DATABASE [dbYarqua] COLLATE Modern_Spanish_CI_AS;

    ALTER DATABASE [dbYarqua] SET MULTI_USER;
    PRINT N'Collation de dbYarqua actualizada a Modern_Spanish_CI_AS.';
END
GO
