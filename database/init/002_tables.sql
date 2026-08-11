/*
  Esquema Yarqtb* — nomenclatura PAUTAS_BD / Guía de Arquitectura AGROSAVIA.
  Geo: País → Departamento → Ciudad (sin FK redundantes).
  Usuario referencia solo Ciudad.
  Autor: AGROSAVIA · Yarqua | 2026-08-07
*/
USE [dbYarqua];
GO

IF OBJECT_ID(N'dbo.YarqtbPais', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbPais (
        Pais_Id     INT           NOT NULL,
        Pais_Nombre NVARCHAR(255) NOT NULL,
        Pais_Estado INT           NOT NULL CONSTRAINT DF_YarqtbPais_Estado DEFAULT (2),
        CONSTRAINT PK_YarqtbPais PRIMARY KEY (Pais_Id)
    );
END
GO

IF OBJECT_ID(N'dbo.YarqtbDepartamento', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbDepartamento (
        Depo_Id     INT           NOT NULL,
        Pais_Id     INT           NOT NULL,
        Depo_Code   NVARCHAR(50)  NOT NULL,
        Depo_Nombre NVARCHAR(255) NOT NULL,
        CONSTRAINT PK_YarqtbDepartamento PRIMARY KEY (Depo_Id),
        CONSTRAINT FK_YarqtbDepartamento_Pais FOREIGN KEY (Pais_Id) REFERENCES dbo.YarqtbPais (Pais_Id),
        CONSTRAINT UQ_YarqtbDepartamento_Pais_Code UNIQUE (Pais_Id, Depo_Code)
    );
END
GO

IF OBJECT_ID(N'dbo.YarqtbCiudad', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbCiudad (
        Ciu_Id     INT           NOT NULL,
        Ciu_Nombre NVARCHAR(255) NOT NULL,
        Depo_Id    INT           NOT NULL,
        Ciu_Cod    NVARCHAR(50)  NOT NULL,
        CONSTRAINT PK_YarqtbCiudad PRIMARY KEY (Ciu_Id),
        CONSTRAINT FK_YarqtbCiudad_Depo FOREIGN KEY (Depo_Id) REFERENCES dbo.YarqtbDepartamento (Depo_Id),
        CONSTRAINT UQ_YarqtbCiudad_Depo_Cod UNIQUE (Depo_Id, Ciu_Cod)
    );
END
GO

-- YarqtbUsuario is managed by EF Core (ASP.NET Identity / ApplicationUser).
-- It is created by 'dotnet ef database update' with nvarchar(450) PK.

-- Logs Serilog (misma BD; columnas estándar del sink MSSqlServer)
IF OBJECT_ID(N'dbo.YarqtbLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbLog (
        Id              INT IDENTITY(1,1) NOT NULL,
        Message         NVARCHAR(MAX) NULL,
        MessageTemplate NVARCHAR(MAX) NULL,
        Level           NVARCHAR(128) NULL,
        TimeStamp       DATETIME NOT NULL,
        Exception       NVARCHAR(MAX) NULL,
        Properties      NVARCHAR(MAX) NULL,
        CONSTRAINT PK_YarqtbLog PRIMARY KEY CLUSTERED (Id)
    );
    CREATE INDEX idx_YarqtbLog_TimeStamp ON dbo.YarqtbLog (TimeStamp);
    CREATE INDEX idx_YarqtbLog_Level ON dbo.YarqtbLog (Level);
END
GO
