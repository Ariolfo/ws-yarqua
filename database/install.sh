#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
CONTAINER="${YARQUA_MSSQL_CONTAINER:-yarqua_mssql}"
SA_PASSWORD="${MSSQL_SA_PASSWORD:-Yarqua_Str0ng!Passw0rd}"

echo "==> Esperando SQL Server en $CONTAINER..."
for i in $(seq 1 60); do
  if docker exec "$CONTAINER" /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" &>/dev/null; then
    break
  fi
  sleep 2
done

for f in 001_create_database.sql 006_rename_to_dbyarqua.sql 002_tables.sql 003_seed_geo.sql 004_seed_colombia_geo_full.sql 005_drop_bug_add_serilog_log.sql 007_tables_catalog.sql 008_seed_catalog.sql 009_normalize_geo_fk.sql 010_seed_ecuador_honduras.sql; do
  echo "==> $f"
  docker cp "$ROOT/database/init/$f" "$CONTAINER:/tmp/$f"
  docker exec "$CONTAINER" /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -b -i "/tmp/$f"
done

echo "==> Verificación"
docker exec "$CONTAINER" /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d dbYarqua -Q \
  "SELECT 'Pais' t, COUNT(*) c FROM YarqtbPais
   UNION ALL SELECT 'Depo', COUNT(*) FROM YarqtbDepartamento
   UNION ALL SELECT 'Ciu', COUNT(*) FROM YarqtbCiudad
   UNION ALL SELECT 'Depo_CO', COUNT(*) FROM YarqtbDepartamento WHERE Pais_Id=170
   UNION ALL SELECT 'Ciu_CO', COUNT(*) FROM YarqtbCiudad c INNER JOIN YarqtbDepartamento d ON d.Depo_Id=c.Depo_Id WHERE d.Pais_Id=170
   UNION ALL SELECT 'Depo_EC', COUNT(*) FROM YarqtbDepartamento WHERE Pais_Id=218
   UNION ALL SELECT 'Ciu_EC', COUNT(*) FROM YarqtbCiudad c INNER JOIN YarqtbDepartamento d ON d.Depo_Id=c.Depo_Id WHERE d.Pais_Id=218
   UNION ALL SELECT 'Depo_HN', COUNT(*) FROM YarqtbDepartamento WHERE Pais_Id=340
   UNION ALL SELECT 'Ciu_HN', COUNT(*) FROM YarqtbCiudad c INNER JOIN YarqtbDepartamento d ON d.Depo_Id=c.Depo_Id WHERE d.Pais_Id=340
   UNION ALL SELECT 'Red', COUNT(*) FROM YarqtbRed
   UNION ALL SELECT 'Cultivo', COUNT(*) FROM YarqtbCultivo
   UNION ALL SELECT 'Sensor', COUNT(*) FROM YarqtbSensor;"
echo "OK: base dbYarqua lista."
