#  Configuración de Entity Framework Core con SQL Server (ASP.NET Core)

##  Objetivo

Configurar Entity Framework Core en un proyecto ASP.NET Core Web API, conectar con SQL Server (LocalDB) y generar la base de datos mediante migraciones.

---

##  1. Instalar paquetes necesarios

Abrir la **Consola del Administrador de paquetes NuGet** y ejecutar:

```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.0
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.0
```

---

##  2. Configurar cadena de conexión

Editar el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ClaseORM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

##  3. Crear DbContext

Crear una clase que herede de `DbContext`:

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Alumno> Alumnos { get; set; }
}
```

---

## 4. Registrar DbContext en Program.cs

Agregar la configuración en `Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
```

---

## 5. Crear primera migración

En la consola de NuGet:

```powershell
Add-Migration InitialCreate
```

Esto genera la estructura inicial de la base de datos.

---

## 6. Crear base de datos

Ejecutar:

```powershell
Update-Database
```

Resultado:

* Se crea la base de datos `ClaseORM`
* Se generan las tablas definidas en el `DbContext`

---

## 7. Verificar

* Abrir **Explorador de objetos SQL Server**
* Buscar:

```
(localdb)\MSSQLLocalDB
 └── Bases de datos
     └── ClaseORM
         └── Tables
```

---

## 8. Probar API

Ejecutar el proyecto y abrir Swagger:

```
https://localhost:xxxx/swagger
```

Probar endpoints (ej: GET `/api/Alumno`)

---

## Errores comunes

### Error 500 - No se puede conectar

* SQL Server no está corriendo
* Connection string incorrecta

### Cannot open database

* La base no existe → ejecutar `Update-Database`

###  No aparece proyecto en NuGet

* Abrir `.sln` en lugar de la carpeta

---

## Resultado final

* API conectada a SQL Server
* Base de datos creada automáticamente
* Entity Framework funcionando correctamente

---

## Nota

Este flujo cubre:

* Configuración de ORM
* Conexión a base de datos
* Migraciones
* Persistencia de datos


---
