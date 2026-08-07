/*
  Normaliza FKs geográficas en BD existente:
  - Ciudad solo → Departamento (quita Pais_Id / Depo_Cod)
  - Usuario solo → Ciudad (Ciu_Id)
  Autor: AGROSAVIA · Yarqua | 2026-08-07
*/
USE [dbYarqua];
GO

/* -------- Ciudad -------- */
IF COL_LENGTH('dbo.YarqtbCiudad', 'Pais_Id') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_YarqtbCiudad_Pais')
        ALTER TABLE dbo.YarqtbCiudad DROP CONSTRAINT FK_YarqtbCiudad_Pais;

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_YarqtbCiudad_Cod' AND object_id = OBJECT_ID(N'dbo.YarqtbCiudad'))
        ALTER TABLE dbo.YarqtbCiudad DROP CONSTRAINT UQ_YarqtbCiudad_Cod;

    ALTER TABLE dbo.YarqtbCiudad DROP COLUMN Pais_Id;
END
GO

IF COL_LENGTH('dbo.YarqtbCiudad', 'Depo_Cod') IS NOT NULL
BEGIN
    ALTER TABLE dbo.YarqtbCiudad DROP COLUMN Depo_Cod;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_YarqtbCiudad_Depo_Cod' AND object_id = OBJECT_ID(N'dbo.YarqtbCiudad'))
BEGIN
    ALTER TABLE dbo.YarqtbCiudad
        ADD CONSTRAINT UQ_YarqtbCiudad_Depo_Cod UNIQUE (Depo_Id, Ciu_Cod);
END
GO

/* -------- Usuario -------- */
IF COL_LENGTH('dbo.YarqtbUsuario', 'Ciu_Id') IS NULL
BEGIN
    ALTER TABLE dbo.YarqtbUsuario ADD Ciu_Id INT NULL;
END
GO

IF COL_LENGTH('dbo.YarqtbUsuario', 'Usua_CodigoCiudad') IS NOT NULL
BEGIN
    ;WITH Mapped AS (
        SELECT
            u.Usua_Nombre,
            u.Usua_CodigoPais,
            u.Usua_CodigoDepartamento,
            u.Usua_CodigoCiudad,
            c.Ciu_Id
        FROM dbo.YarqtbUsuario u
        INNER JOIN dbo.YarqtbDepartamento d
            ON d.Pais_Id = TRY_CAST(u.Usua_CodigoPais AS INT)
           AND d.Depo_Code = u.Usua_CodigoDepartamento
        INNER JOIN dbo.YarqtbCiudad c
            ON c.Depo_Id = d.Depo_Id
           AND c.Ciu_Cod = u.Usua_CodigoCiudad
    )
    UPDATE u
    SET Ciu_Id = m.Ciu_Id
    FROM dbo.YarqtbUsuario u
    INNER JOIN Mapped m
        ON m.Usua_Nombre = u.Usua_Nombre
       AND m.Usua_CodigoPais = u.Usua_CodigoPais
       AND m.Usua_CodigoDepartamento = u.Usua_CodigoDepartamento
       AND m.Usua_CodigoCiudad = u.Usua_CodigoCiudad;

    DELETE FROM dbo.YarqtbUsuario WHERE Ciu_Id IS NULL;
END
GO

IF EXISTS (
    SELECT 1 FROM sys.key_constraints
    WHERE name = N'PK_YarqtbUsuario' AND parent_object_id = OBJECT_ID(N'dbo.YarqtbUsuario')
)
BEGIN
    ALTER TABLE dbo.YarqtbUsuario DROP CONSTRAINT PK_YarqtbUsuario;
END
GO

IF COL_LENGTH('dbo.YarqtbUsuario', 'Usua_Id') IS NULL
BEGIN
    ALTER TABLE dbo.YarqtbUsuario ADD Usua_Id BIGINT IDENTITY(1,1) NOT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.key_constraints
    WHERE name = N'PK_YarqtbUsuario' AND parent_object_id = OBJECT_ID(N'dbo.YarqtbUsuario')
)
BEGIN
    ALTER TABLE dbo.YarqtbUsuario ADD CONSTRAINT PK_YarqtbUsuario PRIMARY KEY (Usua_Id);
END
GO

IF COL_LENGTH('dbo.YarqtbUsuario', 'Ciu_Id') IS NOT NULL
BEGIN
    ALTER TABLE dbo.YarqtbUsuario ALTER COLUMN Ciu_Id INT NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_YarqtbUsuario_Ciudad')
        ALTER TABLE dbo.YarqtbUsuario
            ADD CONSTRAINT FK_YarqtbUsuario_Ciudad FOREIGN KEY (Ciu_Id) REFERENCES dbo.YarqtbCiudad (Ciu_Id);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_YarqtbUsuario_Nombre_Ciudad' AND object_id = OBJECT_ID(N'dbo.YarqtbUsuario'))
        ALTER TABLE dbo.YarqtbUsuario
            ADD CONSTRAINT UQ_YarqtbUsuario_Nombre_Ciudad UNIQUE (Usua_Nombre, Ciu_Id);
END
GO

IF COL_LENGTH('dbo.YarqtbUsuario', 'Usua_CodigoPais') IS NOT NULL
    ALTER TABLE dbo.YarqtbUsuario DROP COLUMN Usua_CodigoPais;
GO
IF COL_LENGTH('dbo.YarqtbUsuario', 'Usua_CodigoDepartamento') IS NOT NULL
    ALTER TABLE dbo.YarqtbUsuario DROP COLUMN Usua_CodigoDepartamento;
GO
IF COL_LENGTH('dbo.YarqtbUsuario', 'Usua_CodigoCiudad') IS NOT NULL
    ALTER TABLE dbo.YarqtbUsuario DROP COLUMN Usua_CodigoCiudad;
GO
