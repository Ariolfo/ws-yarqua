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

IF OBJECT_ID(N'dbo.YarqtbEventoUsuario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbEventoUsuario (
        Even_Id                 BIGINT IDENTITY(1,1) NOT NULL,
        Usua_Nombre             NVARCHAR(150) NOT NULL,
        Even_Fecha              DATE          NOT NULL CONSTRAINT DF_YarqtbEvento_Fecha DEFAULT (CAST(SYSUTCDATETIME() AS DATE)),
        Even_Hora               TIME(7)       NOT NULL CONSTRAINT DF_YarqtbEvento_Hora DEFAULT (CAST(SYSUTCDATETIME() AS TIME)),
        Even_Evento             NVARCHAR(20)  NOT NULL,
        Even_SensorId           NVARCHAR(20)  NULL,
        Even_FechaCreacion      DATETIME2(7)  NOT NULL CONSTRAINT DF_YarqtbEvento_Creacion DEFAULT (SYSUTCDATETIME()),
        Even_FechaActualizacion DATETIME2(7)  NOT NULL CONSTRAINT DF_YarqtbEvento_Actualizacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_YarqtbEventoUsuario PRIMARY KEY (Even_Id),
        CONSTRAINT CK_YarqtbEvento_Tipo CHECK (Even_Evento IN (N'REGISTRO', N'ACCESO', N'CONSULTA'))
    );
END
GO

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

IF OBJECT_ID(N'dbo.YarqtbDevicePushToken', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbDevicePushToken (
        Dpt_Id                 BIGINT IDENTITY(1,1) NOT NULL,
        Usua_Id                NVARCHAR(64)  NOT NULL,
        Dpt_PushToken          NVARCHAR(512) NOT NULL,
        Dpt_Platform           NVARCHAR(16)  NOT NULL,
        Dpt_Activo             BIT           NOT NULL CONSTRAINT DF_YarqtbDpt_Activo DEFAULT (1),
        Dpt_FechaRegistro      DATETIME2(7)  NOT NULL CONSTRAINT DF_YarqtbDpt_Registro DEFAULT (SYSUTCDATETIME()),
        Dpt_FechaActualizacion DATETIME2(7)  NOT NULL CONSTRAINT DF_YarqtbDpt_Actualizacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_YarqtbDevicePushToken PRIMARY KEY (Dpt_Id),
        CONSTRAINT UQ_YarqtbDevicePushToken UNIQUE (Dpt_PushToken),
        CONSTRAINT CK_YarqtbDpt_Platform CHECK (Dpt_Platform IN (N'android', N'ios', N'local'))
    );
END
GO

IF OBJECT_ID(N'dbo.YarqtbUsuarioDispositivo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbUsuarioDispositivo (
        Udi_DeviceId           NVARCHAR(64) NOT NULL,
        Usua_Id                NVARCHAR(64) NOT NULL,
        Udi_Platform           NVARCHAR(16) NOT NULL,
        Udi_FechaRegistro      DATETIME2(7) NOT NULL CONSTRAINT DF_YarqtbUdi_Registro DEFAULT (SYSUTCDATETIME()),
        Udi_FechaActualizacion DATETIME2(7) NOT NULL CONSTRAINT DF_YarqtbUdi_Actualizacion DEFAULT (SYSUTCDATETIME()),
        Udi_Activo             BIT          NOT NULL CONSTRAINT DF_YarqtbUdi_Activo DEFAULT (1),
        CONSTRAINT PK_YarqtbUsuarioDispositivo PRIMARY KEY (Udi_DeviceId),
        CONSTRAINT CK_YarqtbUdi_Platform CHECK (Udi_Platform IN (N'android', N'ios', N'web', N'unknown'))
    );
END
GO
