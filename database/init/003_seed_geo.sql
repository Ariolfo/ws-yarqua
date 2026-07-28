/*
  Semilla geográfica base — Colombia, Ecuador, Honduras.
  Incluye Valle del Cauca (zona RED ASORUT) y capitales/principales para registro.
  Autor: AGROSAVIA · Yarqua | 2026-07-22
*/
USE [dbYarqua];
GO

MERGE dbo.YarqtbPais AS t
USING (VALUES
    (170, N'COLOMBIA', 2),
    (218, N'ECUADOR', 2),
    (340, N'HONDURAS', 2)
) AS s (Pais_Id, Pais_Nombre, Pais_Estado)
ON t.Pais_Id = s.Pais_Id
WHEN MATCHED THEN UPDATE SET Pais_Nombre = s.Pais_Nombre, Pais_Estado = s.Pais_Estado
WHEN NOT MATCHED THEN INSERT (Pais_Id, Pais_Nombre, Pais_Estado) VALUES (s.Pais_Id, s.Pais_Nombre, s.Pais_Estado);
GO

MERGE dbo.YarqtbDepartamento AS t
USING (VALUES
    -- Colombia (subset DANE)
    (24, 170, N'76', N'VALLE DEL CAUCA'),
    (1, 170, N'05', N'ANTIOQUIA'),
    (3, 170, N'11', N'BOGOTÁ, D.C.'),
    (11, 170, N'25', N'CUNDINAMARCA'),
    (19, 170, N'63', N'QUINDÍO'),
    (20, 170, N'66', N'RISARALDA'),
    -- Ecuador
    (1001, 218, N'17', N'PICHINCHA'),
    (1002, 218, N'09', N'GUAYAS'),
    (1003, 218, N'13', N'MANABÍ'),
    -- Honduras
    (2001, 340, N'08', N'FRANCISCO MORAZÁN'),
    (2002, 340, N'05', N'CORTÉS'),
    (2003, 340, N'18', N'YORO')
) AS s (Depo_Id, Pais_Id, Depo_Code, Depo_Nombre)
ON t.Depo_Id = s.Depo_Id
WHEN MATCHED THEN UPDATE SET Pais_Id = s.Pais_Id, Depo_Code = s.Depo_Code, Depo_Nombre = s.Depo_Nombre
WHEN NOT MATCHED THEN INSERT (Depo_Id, Pais_Id, Depo_Code, Depo_Nombre) VALUES (s.Depo_Id, s.Pais_Id, s.Depo_Code, s.Depo_Nombre);
GO

MERGE dbo.YarqtbCiudad AS t
USING (VALUES
    -- Valle del Cauca
    (76001, N'CALI', 24, 170, N'76001', N'76'),
    (76109, N'BUENAVENTURA', 24, 170, N'76109', N'76'),
    (76111, N'BUGALAGRANDE', 24, 170, N'76111', N'76'),
    (76113, N'BUGA', 24, 170, N'76113', N'76'),
    (76147, N'CARTAGO', 24, 170, N'76147', N'76'),
    (76248, N'EL CERRITO', 24, 170, N'76248', N'76'),
    (76306, N'GINEBRA', 24, 170, N'76306', N'76'),
    (76364, N'JAMUNDÍ', 24, 170, N'76364', N'76'),
    (76400, N'LA UNIÓN', 24, 170, N'76400', N'76'),
    (76497, N'OBANDO', 24, 170, N'76497', N'76'),
    (76520, N'PALMIRA', 24, 170, N'76520', N'76'),
    (76622, N'ROLDANILLO', 24, 170, N'76622', N'76'),
    (76670, N'SAN PEDRO', 24, 170, N'76670', N'76'),
    (76834, N'TULUÁ', 24, 170, N'76834', N'76'),
    (76892, N'YUMBO', 24, 170, N'76892', N'76'),
    (76895, N'ZARZAL', 24, 170, N'76895', N'76'),
    -- Antioquia / Bogotá
    (5001, N'MEDELLÍN', 1, 170, N'05001', N'05'),
    (11001, N'BOGOTÁ, D.C.', 3, 170, N'11001', N'11'),
    -- Ecuador
    (170150, N'QUITO', 1001, 218, N'170150', N'17'),
    (90150, N'GUAYAQUIL', 1002, 218, N'090150', N'09'),
    (130150, N'PORTOVIEJO', 1003, 218, N'130150', N'13'),
    -- Honduras
    (80101, N'TEGUCIGALPA', 2001, 340, N'080101', N'08'),
    (50101, N'SAN PEDRO SULA', 2002, 340, N'050101', N'05'),
    (180101, N'EL PROGRESO', 2003, 340, N'180101', N'18')
) AS s (Ciu_Id, Ciu_Nombre, Depo_Id, Pais_Id, Ciu_Cod, Depo_Cod)
ON t.Ciu_Id = s.Ciu_Id
WHEN MATCHED THEN UPDATE SET
    Ciu_Nombre = s.Ciu_Nombre, Depo_Id = s.Depo_Id, Pais_Id = s.Pais_Id,
    Ciu_Cod = s.Ciu_Cod, Depo_Cod = s.Depo_Cod
WHEN NOT MATCHED THEN INSERT (Ciu_Id, Ciu_Nombre, Depo_Id, Pais_Id, Ciu_Cod, Depo_Cod)
    VALUES (s.Ciu_Id, s.Ciu_Nombre, s.Depo_Id, s.Pais_Id, s.Ciu_Cod, s.Depo_Cod);
GO
