/*
  Semilla: redes–país, cultivos (calculadora) y sensores (API Visualiti + IDI-DPA-010).
  Autor: AGROSAVIA · Hidrix | 2026-08-07
*/
USE [dbHidrix];
GO

/* Redes ↔ país */
MERGE dbo.HidrtbRed AS t
USING (VALUES
    (N'RED ASORUT', 170),
    (N'RED ECUADOR', 218),
    (N'RED HONDURA', 340)
) AS s (Red_Nombre, Pais_Id)
ON t.Red_Nombre = s.Red_Nombre
WHEN MATCHED THEN UPDATE SET Pais_Id = s.Pais_Id
WHEN NOT MATCHED THEN INSERT (Red_Nombre, Pais_Id) VALUES (s.Red_Nombre, s.Pais_Id);
GO

/* Cultivos de la calculadora */
MERGE dbo.HidrtbCultivo AS t
USING (VALUES
    (N'Aguacate', CAST(39 AS DECIMAL(8,2)), CAST(31.20 AS DECIMAL(8,2)), CAST(24.96 AS DECIMAL(8,2))),
    (N'Cacao',    CAST(34 AS DECIMAL(8,2)), CAST(27.20 AS DECIMAL(8,2)), CAST(21.76 AS DECIMAL(8,2))),
    (N'Lima',     CAST(36 AS DECIMAL(8,2)), CAST(28.80 AS DECIMAL(8,2)), CAST(23.04 AS DECIMAL(8,2))),
    (N'Papaya',   CAST(34 AS DECIMAL(8,2)), CAST(27.20 AS DECIMAL(8,2)), CAST(21.76 AS DECIMAL(8,2)))
) AS s (Cult_Nombre, Cult_CapacidadCampo, Cult_PorcentajeMaximo, Cult_DecisionRiego)
ON t.Cult_Nombre = s.Cult_Nombre
WHEN MATCHED THEN UPDATE SET
    Cult_CapacidadCampo = s.Cult_CapacidadCampo,
    Cult_PorcentajeMaximo = s.Cult_PorcentajeMaximo,
    Cult_DecisionRiego = s.Cult_DecisionRiego,
    Cult_FechaActualizacion = SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT (Cult_Nombre, Cult_CapacidadCampo, Cult_PorcentajeMaximo, Cult_DecisionRiego)
    VALUES (s.Cult_Nombre, s.Cult_CapacidadCampo, s.Cult_PorcentajeMaximo, s.Cult_DecisionRiego);
GO

/* Sensores: API ago 2026 + coordenadas IDI-DPA-010 */
;WITH Src AS (
    SELECT * FROM (VALUES
    -- Colombia · RED ASORUT (coords hoja RED COL)
    (N'M312', N'RED ASORUT', N'Lima',     N'Finca El Vergel',     CAST(4.5328100 AS DECIMAL(10,7)), CAST(-76.0704000 AS DECIMAL(10,7))),
    (N'M313', N'RED ASORUT', N'Lima',     N'Finca El Vergel',     CAST(4.5329700 AS DECIMAL(10,7)), CAST(-76.0704000 AS DECIMAL(10,7))),
    (N'M314', N'RED ASORUT', N'Lima',     N'Finca El Vergel',     CAST(4.5331400 AS DECIMAL(10,7)), CAST(-76.0703900 AS DECIMAL(10,7))),
    (N'M315', N'RED ASORUT', N'Aguacate', N'Finca San Antonio',   CAST(4.5220500 AS DECIMAL(10,7)), CAST(-76.0772100 AS DECIMAL(10,7))),
    (N'M316', N'RED ASORUT', N'Aguacate', N'Finca San Antonio',   CAST(4.5219800 AS DECIMAL(10,7)), CAST(-76.0773200 AS DECIMAL(10,7))),
    (N'M317', N'RED ASORUT', N'Aguacate', N'Finca San Antonio',   CAST(4.5219100 AS DECIMAL(10,7)), CAST(-76.0774000 AS DECIMAL(10,7))),
    (N'M318', N'RED ASORUT', N'Cacao',    N'Finca San Antonio',   CAST(4.5236600 AS DECIMAL(10,7)), CAST(-76.0783300 AS DECIMAL(10,7))),
    (N'M319', N'RED ASORUT', N'Cacao',    N'Finca San Antonio',   CAST(4.5236900 AS DECIMAL(10,7)), CAST(-76.0781900 AS DECIMAL(10,7))),
    (N'M320', N'RED ASORUT', N'Cacao',    N'Finca San Antonio',   CAST(4.5239100 AS DECIMAL(10,7)), CAST(-76.0779700 AS DECIMAL(10,7))),
    (N'M321', N'RED ASORUT', N'Papaya',   N'Finca La Floresta',   CAST(4.4719000 AS DECIMAL(10,7)), CAST(-76.0893700 AS DECIMAL(10,7))),
    (N'M322', N'RED ASORUT', N'Papaya',   N'Finca La Floresta',   CAST(4.4719500 AS DECIMAL(10,7)), CAST(-76.0894500 AS DECIMAL(10,7))),
    (N'M323', N'RED ASORUT', N'Papaya',   N'Finca La Floresta',   CAST(4.4720100 AS DECIMAL(10,7)), CAST(-76.0895200 AS DECIMAL(10,7))),
    (N'M336', N'RED ASORUT', N'Lima',     N'Parcela Tahití',      CAST(4.5329700 AS DECIMAL(10,7)), CAST(-76.0704000 AS DECIMAL(10,7))),
    -- Ecuador · RED ECUADOR (hoja RED ECU; M0333/M0334 → M333/M334 API)
    (N'M333', N'RED ECUADOR', N'Cacao', NULL, CAST(-1.1717500 AS DECIMAL(10,7)), CAST(-80.3915560 AS DECIMAL(10,7))),
    (N'M334', N'RED ECUADOR', N'Cacao', NULL, CAST(-1.1727400 AS DECIMAL(10,7)), CAST(-80.3932900 AS DECIMAL(10,7))),
    (N'M335', N'RED ECUADOR', N'Cacao', N'Parcela Cacao Productor', CAST(-1.2626100 AS DECIMAL(10,7)), CAST(-80.4162300 AS DECIMAL(10,7))),
    -- Honduras · RED HONDURA (hoja RED HND)
    (N'M324', N'RED HONDURA', N'Lima',   NULL, CAST(14.0100000 AS DECIMAL(10,7)), CAST(-87.0031600 AS DECIMAL(10,7))),
    (N'M325', N'RED HONDURA', N'Lima',   NULL, CAST(14.0102500 AS DECIMAL(10,7)), CAST(-87.0037200 AS DECIMAL(10,7))),
    (N'M326', N'RED HONDURA', N'Lima',   NULL, CAST(14.0102500 AS DECIMAL(10,7)), CAST(-87.0039400 AS DECIMAL(10,7))),
    (N'M327', N'RED HONDURA', N'Papaya', NULL, CAST(14.0102700 AS DECIMAL(10,7)), CAST(-87.0024100 AS DECIMAL(10,7))),
    (N'M328', N'RED HONDURA', N'Papaya', NULL, CAST(14.0099400 AS DECIMAL(10,7)), CAST(-87.0023600 AS DECIMAL(10,7))),
    (N'M329', N'RED HONDURA', N'Papaya', NULL, CAST(14.0101300 AS DECIMAL(10,7)), CAST(-87.0023000 AS DECIMAL(10,7))),
    (N'M330', N'RED HONDURA', N'Cacao',  NULL, CAST(13.9992200 AS DECIMAL(10,7)), CAST(-86.9877500 AS DECIMAL(10,7))),
    (N'M331', N'RED HONDURA', N'Cacao',  NULL, CAST(13.9994100 AS DECIMAL(10,7)), CAST(-86.9877200 AS DECIMAL(10,7))),
    (N'M332', N'RED HONDURA', N'Cacao',  NULL, CAST(13.9996600 AS DECIMAL(10,7)), CAST(-86.9875500 AS DECIMAL(10,7)))
    ) AS v (Sens_Nombre, Red_Nombre, Cult_Nombre, Sens_Finca, Sens_Latitud, Sens_Longitud)
)
MERGE dbo.HidrtbSensor AS t
USING (
    SELECT
        s.Sens_Nombre,
        r.Red_Id,
        c.Cult_Id,
        s.Sens_Latitud,
        s.Sens_Longitud,
        s.Sens_Finca
    FROM Src s
    INNER JOIN dbo.HidrtbRed r ON r.Red_Nombre = s.Red_Nombre
    LEFT JOIN dbo.HidrtbCultivo c ON c.Cult_Nombre = s.Cult_Nombre
) AS x
ON t.Sens_Nombre = x.Sens_Nombre
WHEN MATCHED THEN UPDATE SET
    Red_Id = x.Red_Id,
    Cult_Id = x.Cult_Id,
    Sens_Latitud = x.Sens_Latitud,
    Sens_Longitud = x.Sens_Longitud,
    Sens_Finca = x.Sens_Finca,
    Sens_Estado = COALESCE(t.Sens_Estado, N'desconocido'),
    Sens_Conectividad = COALESCE(t.Sens_Conectividad, N'offline'),
    Sens_FechaActualizacion = SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT (
    Sens_Nombre, Red_Id, Cult_Id, Sens_Latitud, Sens_Longitud,
    Sens_Estado, Sens_Conectividad, Sens_Finca, Sens_Canales
) VALUES (
    x.Sens_Nombre, x.Red_Id, x.Cult_Id, x.Sens_Latitud, x.Sens_Longitud,
    N'desconocido', N'offline', x.Sens_Finca, 2
);
GO
