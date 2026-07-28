/*
  Crea la base de datos dbYarqua en SQL Server 2019.
  Collation corporativa DTI: Modern_Spanish_CI_AS
  Autor: AGROSAVIA · Yarqua
*/
IF DB_ID(N'dbYarqua') IS NULL
BEGIN
    CREATE DATABASE [dbYarqua]
        COLLATE Modern_Spanish_CI_AS;
END
GO
