/*
  Catálogo Red–País, Cultivos y Sensores (persistidos).
  Autor: AGROSAVIA · Hidrix | 2026-08-07
*/
USE [dbHidrix];
GO

IF OBJECT_ID(N'dbo.HidrtbRed', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HidrtbRed (
        Red_Id     INT IDENTITY(1,1) NOT NULL,
        Red_Nombre NVARCHAR(50)      NOT NULL,
        Pais_Id    INT               NOT NULL,
        CONSTRAINT PK_HidrtbRed PRIMARY KEY (Red_Id),
        CONSTRAINT UQ_HidrtbRed_Nombre UNIQUE (Red_Nombre),
        CONSTRAINT FK_HidrtbRed_Pais FOREIGN KEY (Pais_Id) REFERENCES dbo.HidrtbPais (Pais_Id)
    );
END
GO

IF OBJECT_ID(N'dbo.HidrtbCultivo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HidrtbCultivo (
        Cult_Id               INT IDENTITY(1,1) NOT NULL,
        Cult_Nombre           NVARCHAR(100)     NOT NULL,
        Cult_CapacidadCampo   DECIMAL(8,2)      NOT NULL,
        Cult_PorcentajeMaximo DECIMAL(8,2)      NOT NULL,
        Cult_DecisionRiego    DECIMAL(8,2)      NOT NULL,
        Cult_Activo           BIT               NOT NULL CONSTRAINT DF_HidrtbCultivo_Activo DEFAULT (1),
        Cult_FechaCreacion    DATETIME2(7)      NOT NULL CONSTRAINT DF_HidrtbCultivo_Creacion DEFAULT (SYSUTCDATETIME()),
        Cult_FechaActualizacion DATETIME2(7)    NOT NULL CONSTRAINT DF_HidrtbCultivo_Actualizacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_HidrtbCultivo PRIMARY KEY (Cult_Id),
        CONSTRAINT UQ_HidrtbCultivo_Nombre UNIQUE (Cult_Nombre)
    );
END
GO

IF OBJECT_ID(N'dbo.HidrtbSensor', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HidrtbSensor (
        Sens_Id            INT IDENTITY(1,1) NOT NULL,
        Sens_Nombre        NVARCHAR(50)      NOT NULL,
        Red_Id             INT               NOT NULL,
        Cult_Id            INT               NULL,
        Sens_Latitud       DECIMAL(10,7)     NULL,
        Sens_Longitud      DECIMAL(10,7)     NULL,
        Sens_Estado        NVARCHAR(50)      NULL,
        Sens_Conectividad  NVARCHAR(20)      NULL,
        Sens_Finca         NVARCHAR(150)     NULL,
        Sens_Canales       INT               NOT NULL CONSTRAINT DF_HidrtbSensor_Canales DEFAULT (2),
        Sens_Activo        BIT               NOT NULL CONSTRAINT DF_HidrtbSensor_Activo DEFAULT (1),
        Sens_FechaCreacion DATETIME2(7)      NOT NULL CONSTRAINT DF_HidrtbSensor_Creacion DEFAULT (SYSUTCDATETIME()),
        Sens_FechaActualizacion DATETIME2(7) NOT NULL CONSTRAINT DF_HidrtbSensor_Actualizacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_HidrtbSensor PRIMARY KEY (Sens_Id),
        CONSTRAINT UQ_HidrtbSensor_Nombre UNIQUE (Sens_Nombre),
        CONSTRAINT FK_HidrtbSensor_Red FOREIGN KEY (Red_Id) REFERENCES dbo.HidrtbRed (Red_Id),
        CONSTRAINT FK_HidrtbSensor_Cult FOREIGN KEY (Cult_Id) REFERENCES dbo.HidrtbCultivo (Cult_Id)
    );
END
GO
