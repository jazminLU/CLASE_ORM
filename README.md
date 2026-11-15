# CLASE_ORM — API de Gestión Académica

API REST en **ASP.NET Core 8** para la gestión de Alumnos y Materias,
desarrollada como trabajo práctico integrador. Implementa **Clean
Architecture**, autenticación con **JWT** y **BCrypt**, autorización por
**roles**, **middlewares personalizados**, **gestión global de
excepciones** y **paginación/filtrado**, usando **Entity Framework Core**
con la modalidad **Code First** sobre SQL Server.

El frontend (Angular 20 + Angular Material) consume esta API y vive en un
repositorio aparte.

---

## Tecnologías

- **.NET 8** / ASP.NET Core Web API
- **Entity Framework Core 8** (SQL Server, Code First + Migrations)
- **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **BCrypt.Net-Next** para el hasheo de contraseñas
- **Swagger / Swashbuckle** para documentación interactiva

---

## Arquitectura (Clean Architecture)

La solución está dividida en 4 proyectos, cada uno con una responsabilidad
y dependencias en una sola dirección (hacia adentro):

```
CLASE_ORM.sln
├── CLASE_ORM.Domain            → Entidades puras, sin dependencias
│   └── Entities/                 Alumno, Materia, Profesor, Usuario
│
├── CLASE_ORM.Application       → Contratos e intención, depende solo de Domain
│   ├── DTOs/                     DTOs de entrada/salida por entidad
│   ├── Interfaces/                IAlumnoService, IMateriaService, IAuthService
│   ├── Exceptions/                NotFoundException, ConflictException
│   └── Common/                    PagedResult<T>, ErrorResponse
│
├── CLASE_ORM.Infrastructure     → Implementación concreta (EF Core, JWT, hashing)
│   ├── Persistence/AppDbContext.cs
│   ├── Migrations/
│   ├── Services/                 AlumnoService, MateriaService, AuthService
│   └── DependencyInjection.cs    Registra DbContext, Services y JWT
│
└── CLASE_ORM (Api)              → Punto de entrada HTTP
    ├── Controllers/              AlumnoController, MateriaController, AuthController
    ├── Middleware/                RequestLoggingMiddleware, ExceptionHandlingMiddleware
    ├── Program.cs
    └── appsettings.json
```

`Domain` no depende de nada. `Application` depende solo de `Domain`.
`Infrastructure` depende de `Application` (y transitivamente de `Domain`) y
es la única capa, junto con la Api, que conoce EF Core y ASP.NET Core.
`CLASE_ORM` (Api) depende de `Application` e `Infrastructure`, y resuelve
todo vía inyección de dependencias (`builder.Services.AddInfrastructure(...)`
en `Program.cs`).

---

## Funcionalidades

### Autenticación y roles
- Registro (`POST /api/Auth/register`) y login (`POST /api/Auth/login`)
  con contraseña hasheada con **BCrypt** y emisión de **JWT**.
- Dos roles: **Admin** y **Usuario**. El primer usuario registrado en una
  base vacía queda como Admin automáticamente; el resto, como Usuario.
- `GET` sobre Alumno/Materia: accesible para cualquier usuario autenticado.
- `POST` / `PUT` / `DELETE`: restringido a rol **Admin**
  (`[Authorize(Roles = "Admin")]`).

### Middlewares personalizados
- **`RequestLoggingMiddleware`**: loguea método, ruta, código de respuesta
  y tiempo de ejecución de cada request.
- **`ExceptionHandlingMiddleware`**: captura cualquier excepción no
  controlada y devuelve una respuesta JSON estructurada en vez de un
  error crudo:
  ```json
  { "statusCode": 404, "message": "No se encontró la materia con id 99.", "timestamp": "2026-11-15T01:52:46Z" }
  ```
  Mapea `NotFoundException` → 404, `ConflictException` → 409,
  `UnauthorizedAccessException` → 401, y cualquier otra excepción → 500
  (sin exponer detalles internos al cliente; el error completo queda
  logueado del lado del servidor).

### Paginación y filtrado
Los endpoints `GET /api/Alumno` y `GET /api/Materia` aceptan query params:

| Parámetro    | Tipo   | Default | Descripción                          |
|--------------|--------|---------|---------------------------------------|
| `pageNumber` | int    | 1       | Número de página                      |
| `pageSize`   | int    | 10      | Tamaño de página                      |
| `nombre`     | string | —       | Filtra por nombre (coincidencia parcial) |

Y devuelven un `PagedResult<T>`:
```json
{ "items": [ ... ], "pageNumber": 1, "pageSize": 10, "totalCount": 23, "totalPages": 3 }
```

---

## Cómo levantar el proyecto

### Requisitos
- .NET 8 SDK
- SQL Server (local o SQL Express)
- Herramienta `dotnet-ef` (`dotnet tool install --global dotnet-ef`)

### Pasos

1. Configurar la cadena de conexión en `appsettings.json` (clave `Con`) y,
   si hace falta, el secreto JWT en la sección `Jwt`.

2. Aplicar las migraciones (el `DbContext` vive en `CLASE_ORM.Infrastructure`,
   por eso hay que indicar `--project` y `--startup-project`):

   ```bash
   dotnet ef database update --project CLASE_ORM.Infrastructure --startup-project .
   ```

3. Levantar la API:

   ```bash
   dotnet run --launch-profile https
   ```

4. Acceder a Swagger:

   - `https://localhost:7197/swagger`
   - (también disponible en `http://localhost:5202`)

### Crear una nueva migración

```bash
dotnet ef migrations add NombreDeLaMigracion --project CLASE_ORM.Infrastructure --startup-project .
dotnet ef database update --project CLASE_ORM.Infrastructure --startup-project .
```

---

## Endpoints principales

| Método | Ruta                  | Auth           | Descripción                          |
|--------|-----------------------|----------------|----------------------------------------|
| POST   | `/api/Auth/register`  | Público        | Crea un usuario                        |
| POST   | `/api/Auth/login`     | Público        | Devuelve `{ token, usuario }`          |
| GET    | `/api/Alumno`         | Autenticado    | Lista paginada/filtrada                |
| GET    | `/api/Alumno/{id}`    | Autenticado    | Detalle                                |
| POST   | `/api/Alumno`         | Admin          | Crear                                  |
| PUT    | `/api/Alumno/{id}`    | Admin          | Actualizar                             |
| DELETE | `/api/Alumno/{id}`    | Admin          | Eliminar                               |
| GET    | `/api/Materia`        | Autenticado    | Lista paginada/filtrada                |
| GET    | `/api/Materia/{id}`   | Autenticado    | Detalle                                |
| POST   | `/api/Materia`        | Admin          | Crear                                  |
| PUT    | `/api/Materia/{id}`   | Admin          | Actualizar                             |
| DELETE | `/api/Materia/{id}`   | Admin          | Eliminar                               |

Todas las rutas (salvo `Auth`) requieren el header
`Authorization: Bearer <token>` obtenido en el login.

---

## Frontend

El cliente Angular que consume esta API está en un repositorio aparte
(Angular 20 + Angular Material: tablas, diálogos, formularios, paginador,
etc.), con login/registro, guard de rutas, interceptor de JWT y UI que se
adapta según el rol del usuario logueado.
