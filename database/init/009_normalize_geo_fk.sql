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

-- YarqtbUsuario is managed by EF Core (ASP.NET Identity).
-- No manual schema changes needed here.
GO
