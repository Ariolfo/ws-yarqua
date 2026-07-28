/*
  Migra BD existente:
  - Elimina YarqtbBug (reemplazada por Serilog → YarqtbLog)
  - Crea YarqtbLog para Serilog.Sinks.MSSqlServer
*/
USE [dbYarqua];
GO

IF OBJECT_ID(N'dbo.YarqtbBug', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.YarqtbBug;
END
GO

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
