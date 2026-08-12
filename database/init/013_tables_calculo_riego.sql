/*
  Registros de la calculadora de riego por usuario.
  Convención DTI Yarqua: Yarqtb* / Prefijo_Campo
  Autor: AGROSAVIA · Yarqua | 2026-08-12
*/
USE [dbYarqua];
GO

IF OBJECT_ID(N'dbo.YarqtbCalculoRiego', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YarqtbCalculoRiego (
        Calc_Id                 INT IDENTITY(1,1) NOT NULL,
        Usua_Id                 NVARCHAR(450)     NOT NULL,
        Calc_CultivoNombre      NVARCHAR(100)     NOT NULL,
        Cult_Id                 INT               NULL,
        Calc_CapacidadCampo     DECIMAL(8,2)      NOT NULL,
        Calc_LimiteMaxRiego     DECIMAL(8,2)      NOT NULL,
        Calc_DecisionRiego      DECIMAL(8,2)      NOT NULL,
        Calc_FechaConsulta      DATE              NOT NULL,
        Calc_HumedadManana      DECIMAL(8,2)      NOT NULL,
        Calc_HumedadTarde       DECIMAL(8,2)      NOT NULL,
        Calc_Recomendacion      NVARCHAR(20)      NOT NULL,
        Calc_RealizoRiego       NVARCHAR(10)      NULL,
        Calc_Observacion        NVARCHAR(500)     NULL,
        Calc_Activo             BIT               NOT NULL CONSTRAINT DF_YarqtbCalculoRiego_Activo DEFAULT (1),
        Calc_FechaCreacion      DATETIME2(7)      NOT NULL CONSTRAINT DF_YarqtbCalculoRiego_Creacion DEFAULT (SYSUTCDATETIME()),
        Calc_FechaActualizacion DATETIME2(7)      NOT NULL CONSTRAINT DF_YarqtbCalculoRiego_Actualizacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_YarqtbCalculoRiego PRIMARY KEY (Calc_Id),
        CONSTRAINT FK_YarqtbCalculoRiego_Usuario FOREIGN KEY (Usua_Id)
            REFERENCES dbo.YarqtbUsuario (Usua_Id),
        CONSTRAINT FK_YarqtbCalculoRiego_Cultivo FOREIGN KEY (Cult_Id)
            REFERENCES dbo.YarqtbCultivo (Cult_Id)
    );

    CREATE INDEX IX_YarqtbCalculoRiego_Usua_Cultivo
        ON dbo.YarqtbCalculoRiego (Usua_Id, Calc_CultivoNombre, Calc_FechaConsulta DESC);
END
GO
