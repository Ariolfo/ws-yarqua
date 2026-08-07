/*
  Catálogo Red–País, Cultivos y Sensores (persistidos).
  Autor: AGROSAVIA · Yarqua | 2026-08-07
*/
USE [dbYarqua];
GO

IF OBJECT_ID(N'dbo.YarqtbRed', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbRed (
        Red_Id     INT IDENTITY(1,1) NOT NULL,
        Red_Nombre NVARCHAR(50)      NOT NULL,
        Pais_Id    INT               NOT NULL,
        CONSTRAINT PK_YarqtbRed PRIMARY KEY (Red_Id),
        CONSTRAINT UQ_YarqtbRed_Nombre UNIQUE (Red_Nombre),
        CONSTRAINT FK_YarqtbRed_Pais FOREIGN KEY (Pais_Id) REFERENCES dbo.YarqtbPais (Pais_Id)
    );
END
GO

IF OBJECT_ID(N'dbo.YarqtbCultivo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbCultivo (
        Cult_Id               INT IDENTITY(1,1) NOT NULL,
        Cult_Nombre           NVARCHAR(100)     NOT NULL,
        Cult_CapacidadCampo   DECIMAL(8,2)      NOT NULL,
        Cult_PorcentajeMaximo DECIMAL(8,2)      NOT NULL,
        Cult_DecisionRiego    DECIMAL(8,2)      NOT NULL,
        Cult_Activo           BIT               NOT NULL CONSTRAINT DF_YarqtbCultivo_Activo DEFAULT (1),
        Cult_FechaCreacion    DATETIME2(7)      NOT NULL CONSTRAINT DF_YarqtbCultivo_Creacion DEFAULT (SYSUTCDATETIME()),
        Cult_FechaActualizacion DATETIME2(7)    NOT NULL CONSTRAINT DF_YarqtbCultivo_Actualizacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_YarqtbCultivo PRIMARY KEY (Cult_Id),
        CONSTRAINT UQ_YarqtbCultivo_Nombre UNIQUE (Cult_Nombre)
    );
END
GO

IF OBJECT_ID(N'dbo.YarqtbSensor', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbSensor (
        Sens_Id            INT IDENTITY(1,1) NOT NULL,
        Sens_Nombre        NVARCHAR(50)      NOT NULL,
        Red_Id             INT               NOT NULL,
        Cult_Id            INT               NULL,
        Sens_Latitud       DECIMAL(10,7)     NULL,
        Sens_Longitud      DECIMAL(10,7)     NULL,
        Sens_Estado        NVARCHAR(50)      NULL,
        Sens_Conectividad  NVARCHAR(20)      NULL,
        Sens_Finca         NVARCHAR(150)     NULL,
        Sens_Canales       INT               NOT NULL CONSTRAINT DF_YarqtbSensor_Canales DEFAULT (2),
        Sens_Activo        BIT               NOT NULL CONSTRAINT DF_YarqtbSensor_Activo DEFAULT (1),
        Sens_FechaCreacion DATETIME2(7)      NOT NULL CONSTRAINT DF_YarqtbSensor_Creacion DEFAULT (SYSUTCDATETIME()),
        Sens_FechaActualizacion DATETIME2(7) NOT NULL CONSTRAINT DF_YarqtbSensor_Actualizacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_YarqtbSensor PRIMARY KEY (Sens_Id),
        CONSTRAINT UQ_YarqtbSensor_Nombre UNIQUE (Sens_Nombre),
        CONSTRAINT FK_YarqtbSensor_Red FOREIGN KEY (Red_Id) REFERENCES dbo.YarqtbRed (Red_Id),
        CONSTRAINT FK_YarqtbSensor_Cult FOREIGN KEY (Cult_Id) REFERENCES dbo.YarqtbCultivo (Cult_Id)
    );
END
GO
