# Diagrama ER — Yarqua (`dbYarqua`)

Modelo actual de la base de datos (nomenclatura `Yarqtb*`).
Desde la migración a ASP.NET Core Identity, `YarqtbUsuario` usa `nvarchar(450)` como PK (GUID de Identity).

## Diagrama

```mermaid
erDiagram
    YarqtbPais ||--o{ YarqtbDepartamento : "tiene"
    YarqtbDepartamento ||--o{ YarqtbCiudad : "tiene"
    YarqtbCiudad ||--o{ YarqtbUsuario : "reside_en"
    YarqtbPais ||--o{ YarqtbRed : "opera_en"
    YarqtbRed ||--o{ YarqtbSensor : "agrupa"
    YarqtbCultivo ||--o{ YarqtbSensor : "asocia"
    YarqtbUsuario ||--o{ YarqtbUsuarioRol : "tiene"
    YarqtbRol ||--o{ YarqtbUsuarioRol : "agrupa"
    YarqtbUsuario ||--o{ YarqtbUsuarioGrupo : "pertenece"
    YarqtbGrupo ||--o{ YarqtbUsuarioGrupo : "contiene"

    YarqtbPais {
        int Pais_Id PK
        nvarchar Pais_Nombre
        int Pais_Estado
    }

    YarqtbDepartamento {
        int Depo_Id PK
        int Pais_Id FK
        nvarchar Depo_Code
        nvarchar Depo_Nombre
    }

    YarqtbCiudad {
        int Ciu_Id PK
        nvarchar Ciu_Nombre
        int Depo_Id FK
        nvarchar Ciu_Cod
    }

    YarqtbUsuario {
        nvarchar Usua_Id PK "GUID Identity (450)"
        nvarchar Usua_Nombre
        nvarchar UserName
        nvarchar NormalizedUserName
        nvarchar Email
        nvarchar NormalizedEmail
        nvarchar PasswordHash
        int Ciu_Id FK
        datetime2 Usua_FechaRegistro
        datetime2 Usua_FechaCreacion
        datetime2 Usua_FechaActualizacion
        bit Usua_Activo
    }

    YarqtbRol {
        nvarchar Id PK
        nvarchar Name
        nvarchar NormalizedName
    }

    YarqtbUsuarioRol {
        nvarchar UserId PK "FK → YarqtbUsuario"
        nvarchar RoleId PK "FK → YarqtbRol"
    }

    YarqtbUsuarioClaim {
        int Id PK
        nvarchar UserId FK
        nvarchar ClaimType
        nvarchar ClaimValue
    }

    YarqtbUsuarioLogin {
        nvarchar LoginProvider PK
        nvarchar ProviderKey PK
        nvarchar UserId FK
    }

    YarqtbUsuarioToken {
        nvarchar UserId PK "FK"
        nvarchar LoginProvider PK
        nvarchar Name PK
        nvarchar Value
    }

    YarqtbRolClaim {
        int Id PK
        nvarchar RoleId FK
        nvarchar ClaimType
        nvarchar ClaimValue
    }

    YarqtbGrupo {
        int Grup_Id PK
        nvarchar Grup_Nombre
        nvarchar Grup_Descripcion
        nvarchar Grup_Rol "Admin|Operador|Visualizador"
        bit Grup_Activo
        datetime2 Grup_FechaCreacion
    }

    YarqtbUsuarioGrupo {
        nvarchar Usua_Id PK "FK → YarqtbUsuario"
        int Grup_Id PK "FK → YarqtbGrupo"
        datetime2 Ugr_FechaAsignacion
    }

    YarqtbRed {
        int Red_Id PK
        nvarchar Red_Nombre
        int Pais_Id FK
    }

    YarqtbCultivo {
        int Cult_Id PK
        nvarchar Cult_Nombre
        decimal Cult_CapacidadCampo
        decimal Cult_PorcentajeMaximo
        decimal Cult_DecisionRiego
        bit Cult_Activo
    }

    YarqtbSensor {
        int Sens_Id PK
        nvarchar Sens_Nombre
        int Red_Id FK
        int Cult_Id FK
        decimal Sens_Latitud
        decimal Sens_Longitud
        nvarchar Sens_Estado
        nvarchar Sens_Conectividad
        nvarchar Sens_Finca
        int Sens_Canales
        bit Sens_Activo
    }

    YarqtbMetodoCC {
        int MCC_Id PK
        nvarchar MCC_Nombre
        bit MCC_Activo
    }

    YarqtbEventoUsuario {
        bigint Even_Id PK
        nvarchar Usua_Nombre
        date Even_Fecha
        time Even_Hora
        nvarchar Even_Evento
        nvarchar Even_SensorId
        datetime2 Even_FechaCreacion
        datetime2 Even_FechaActualizacion
    }

    YarqtbUsuarioDispositivo {
        nvarchar Udi_DeviceId PK
        nvarchar Usua_Id
        nvarchar Udi_Platform
        bit Udi_Activo
        datetime2 Udi_FechaRegistro
        datetime2 Udi_FechaActualizacion
    }

    YarqtbDevicePushToken {
        bigint Dpt_Id PK
        nvarchar Usua_Id
        nvarchar Dpt_PushToken
        nvarchar Dpt_Platform
        bit Dpt_Activo
        datetime2 Dpt_FechaRegistro
        datetime2 Dpt_FechaActualizacion
    }

    YarqtbLog {
        int Id PK
        nvarchar Message
        nvarchar Level
        datetime TimeStamp
    }
```

## Explicación

### Geografía
- **YarqtbPais**: catálogo de países (Colombia, Ecuador, Honduras).
- **YarqtbDepartamento**: provincias/departamentos; pertenece a un país (`Pais_Id`).
- **YarqtbCiudad**: municipios/cantones; pertenece solo al departamento (`Depo_Id`). El país se obtiene vía Departamento → País.

### Usuarios e Identity
- **YarqtbUsuario**: usuario de la app; mapeado a `ApplicationUser : IdentityUser`. La PK `Usua_Id` es un GUID `nvarchar(450)` generado por Identity. Su ubicación es la ciudad (`Ciu_Id`).
- **YarqtbRol / YarqtbUsuarioRol**: roles Identity (`Admin`, `Operador`, `Visualizador`). Sembrados al inicio en `Program.cs`.
- **YarqtbUsuarioClaim / YarqtbUsuarioLogin / YarqtbUsuarioToken / YarqtbRolClaim**: tablas auxiliares de Identity.
- **YarqtbGrupo**: agrupación lógica de usuarios con un rol asociado.
- **YarqtbUsuarioGrupo**: relación muchos-a-muchos usuario ↔ grupo.
- **YarqtbEventoUsuario**: auditoría de eventos (`REGISTRO`, `ACCESO`, `CONSULTA`).
- **YarqtbUsuarioDispositivo** / **YarqtbDevicePushToken**: vínculo dispositivo y tokens push (referencia lógica al `Usua_Id` de Identity).

### Sensores y riego
- **YarqtbRed**: red de sensores ligada a un país (p. ej. RED ASORUT, RED ECUADOR, RED HONDURA).
- **YarqtbCultivo**: parámetros de riego (capacidad de campo, % máximo, % decisión).
- **YarqtbSensor**: nodo/sensor; pertenece a una red y opcionalmente a un cultivo; guarda coordenadas, estado y conectividad.
- **YarqtbMetodoCC**: catálogo de métodos para calcular capacidad de campo.

### Soporte
- **YarqtbLog**: logs de aplicación (Serilog).

### Relaciones (FK)
| Desde | Hacia | Columna |
|-------|-------|---------|
| Departamento | País | `Pais_Id` |
| Ciudad | Departamento | `Depo_Id` |
| Usuario | Ciudad | `Ciu_Id` |
| UsuarioRol | Usuario | `UserId` |
| UsuarioRol | Rol | `RoleId` |
| UsuarioGrupo | Usuario | `Usua_Id` |
| UsuarioGrupo | Grupo | `Grup_Id` |
| Red | País | `Pais_Id` |
| Sensor | Red | `Red_Id` |
| Sensor | Cultivo | `Cult_Id` (nullable) |
