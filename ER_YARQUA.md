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
- **YarqtbRol / YarqtbUsuarioRol**: roles Identity (`Admin`, `User`). Sembrados al inicio en `Program.cs`.
- **YarqtbUsuarioClaim / YarqtbUsuarioLogin / YarqtbUsuarioToken / YarqtbRolClaim**: tablas auxiliares de Identity.

### Sensores y riego
- **YarqtbRed**: red de sensores ligada a un país (p. ej. RED ASORUT, RED ECUADOR, RED HONDURA).
- **YarqtbCultivo**: parámetros de riego (capacidad de campo, % máximo, % decisión).
- **YarqtbSensor**: nodo/sensor; pertenece a una red y opcionalmente a un cultivo; guarda coordenadas, estado y conectividad.

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
| Red | País | `Pais_Id` |
| Sensor | Red | `Red_Id` |
| Sensor | Cultivo | `Cult_Id` (nullable) |
