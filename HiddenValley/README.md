# Hidden Valley

Este proyecto es una aplicación web completa que utiliza una arquitectura de microservicios contenerizada con Docker. Incluye una API robusta y un frontend interactivo desarrollado en Blazor. Es un proyecto universitario para el curso de Analisi de Sistemas. Se basa en una aplicacion para llevar el control y registros de cabañas y visitantes de un turicentro llamado Hidden Valley ubicado en Monjas, Jalapa, Guatemala

## Tecnologías Utilizadas

* **Backend:** .NET 9 Web API
* **Frontend:** Blazor WebAssembly / Server
* **Base de Datos:** PostgreSQL 16 (Entity Framework Core)
* **Contenedores:** Docker & Docker Compose
* **Documentación de la API:** Swagger (Swashbuckle)

## Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Entity Framework Core Tools](https://learn.microsoft.com/en-us/ef/core/get-started/dotnet-cli) (`dotnet tool install --global dotnet-ef`)

## Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/HiddenValley.git
cd HiddenValley
```

### 2. Levantar contenedores con Docker

Este comando descarga las imágenes, compila los proyectos y levanta los servicios. Las tablas de la base de datos se crean automáticamente la primera vez (a partir de `data/Tablas.sql`):

```bash
docker-compose up --build
```

> Si necesitas resetear la base de datos para que se vuelvan a crear las tablas:
> ```bash
> docker-compose down -v
> docker-compose up --build
> ```

### Puntos de acceso

* **Frontend:** http://localhost:7075
* **API (Swagger):** http://localhost:7084/swagger
* **Base de datos:** `localhost:5432` (usuario: `admin` / password: `Umg12345` / db: `HiddenValleyDB`)

---

## Módulos integrados

### Módulo Cabañas — PROYECT-61
`GET /api/cabanas`, `GET /api/cabanas/disponibilidad`, `PUT /api/cabanas/cambiar-estado`, `POST /api/cabanas/registrar`, `DELETE /api/cabanas/{id}`

### Módulo Personas — PROYECT-64
`POST /api/personas`, `GET /api/personas/dpi/{dpi}/existe`, `PATCH /api/personas/{id}/contacto`

### Módulo Clientes
`POST /api/clientes`, `GET /api/clientes/buscar?filtro=`, `GET /api/clientes/{idCliente}/historial`

### Módulo Reservaciones — PROYECT-60

Endpoints:

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/reservaciones` | Crear una reserva validando disponibilidad y capacidad |
| `GET` | `/api/reservaciones?nombre=&telefono=` | Listar todas las reservas con filtros opcionales |
| `GET` | `/api/reservaciones/{id}` | Detalle de una reserva específica (vista calendario) |
| `PATCH` | `/api/reservaciones/{id}` | Actualizar parcialmente los datos de la reserva |
| `PUT` | `/api/reservaciones/{id}/cancelar` | Cancelar una reserva |
| `DELETE` | `/api/reservaciones/{id}` | Eliminar reserva (solo Recibida o Cancelada) |

**Validaciones automáticas implementadas:**
* La cabaña no tiene otra reserva activa en esas fechas (no traslape)
* La capacidad de la cabaña es suficiente
* `CantidadPersonas` no permite negativos ni cero
* La cabaña existe
* El cliente existe
* El cliente no tiene otra reserva activa
* El rango de fechas es válido (salida > entrada, entrada no anterior a hoy en POST)
* El teléfono coincide con el del cliente

**Ejemplo POST `/api/reservaciones`:**

```json
{
  "idCliente": 1,
  "telefono": "55512345",
  "idCabana": 2,
  "fechaEntrada": "2026-05-10",
  "fechaSalida": "2026-05-13",
  "cantidadPersonas": 4,
  "idEmpleado": 1
}
```

> `idEmpleado` es opcional (default: `1`).

**Respuesta:**
```json
{
  "id": 1,
  "mensaje": "Reservación creada con éxito",
  "totalPagar": 750.00,
  "noches": 3
}
```

**Ejemplo PATCH `/api/reservaciones/1`** (todos los campos opcionales):
```json
{
  "fechaSalida": "2026-05-15",
  "cantidadPersonas": 5
}
```

**Ejemplo GET con filtros:**
```
GET /api/reservaciones?nombre=juan&telefono=5551
```

---

## Datos iniciales necesarios

Para que el POST de reservaciones funcione, la base debe tener registros base. Conéctate al contenedor de la BD y ejecuta:

```sql
-- 1. Insertar puesto y empleado (necesarios por la FK de RegistroReservacion)
INSERT INTO PuestoTrabajo (Nombre) VALUES ('Recepcionista');
INSERT INTO Persona (Nombres, Apellidos, Telefono, Direccion)
VALUES ('Sistema', 'Recepción', '00000000', 'N/A');
INSERT INTO Empleado (IdPersona, IdPuestoTrabajo) VALUES (1, 1);

-- 2. Estados y tipos de cabaña
INSERT INTO EstadoCabana (Nombre) VALUES ('Disponible'), ('Ocupada'), ('En limpieza');
INSERT INTO TipoCabana (Nombre, Capacidad, Precio)
VALUES ('Familiar', 6, 250.00), ('Pareja', 2, 150.00);

-- 3. Cabañas
INSERT INTO Cabana (IdTipoCabana, IdEstadoCabana) VALUES (1, 1), (2, 1);

-- 4. Cliente de prueba
INSERT INTO Persona (Nombres, Apellidos, DPI, Telefono, Direccion)
VALUES ('Juan', 'Pérez', '1234567890101', '55512345', 'Jalapa');
INSERT INTO Cliente (IdPersona) VALUES (2);
```

---

### Modulo Dashboard — PROYECT-84
`GET /api/dashboard`

Retorna en una sola peticion todas las metricas operativas del dia:

| Campo | Descripcion |
|---|---|
| `cabanasOcupadasHoy` | Cabanas con reserva activa entre hoy y manana |
| `cabanasDisponibles` | Cabanas en estado "Disponible" que no estan ocupadas hoy |
| `personasEsperadasHoy` | Suma de personas cuyo check-in es hoy |
| `proximasReservas` | Lista de las proximas 5 reservas a partir de hoy, ordenadas cronologicamente |

**Ejemplo GET `/api/dashboard`:**

```json
{
  "cabanasOcupadasHoy": 3,
  "cabanasDisponibles": 5,
  "personasEsperadasHoy": 12,
  "proximasReservas": [
    {
      "id": 7,
      "nombreCliente": "Juan Perez",
      "tipoCabana": "Familiar",
      "fechaEntrada": "2026-05-22T00:00:00",
      "fechaSalida": "2026-05-25T00:00:00",
      "cantidadPersonas": 4,
      "estadoReserva": "Confirmada"
    }
  ]
}
```

Los archivos que conforman este modulo son:
- `HiddenValley.Shared/DTOs/DashboardReservaDto.cs`
- `HiddenValley.Shared/DTOs/DashboardResumenDto.cs`
- `HiddenValley.API/Interfaces/IDashboardService.cs`
- `HiddenValley.API/Services/DashboardService.cs`
- `HiddenValley.API/Controllers/DashboardController.cs`
- `HiddenValley.Frontend/Interfaces/IDashboardService.cs`
- `HiddenValley.Frontend/Service/DashboardService.cs`
- `HiddenValley.Frontend/Pages/Dashboard.razor`

---

### Modulo Dashboard Frontend — PROYECT-91

Vista de operaciones diarias del turicentro. Accesible desde `/dashboard`.

Consume el endpoint `GET /api/dashboard` y muestra:

- Tarjeta de cabanas disponibles
- Tarjeta de cabanas ocupadas hoy
- Tarjeta de personas esperadas hoy (check-in del dia)
- Tabla de proximas reservaciones (hasta 5), con nombre del cliente, tipo de cabana, fechas, cantidad de personas y estado
- Mensaje amigable cuando no hay reservaciones proximas programadas

---

## Pruebas Unitarias — `HiddenValley.Tests`

Proyecto de pruebas unitarias independiente que valida la capa de servicios de la API sin acceso a base de datos real. Utiliza **EF Core InMemory** como mock del contexto de base de datos.

### Tecnologías del proyecto de pruebas

* **Framework:** xUnit
* **Base de datos simulada:** Microsoft.EntityFrameworkCore.InMemory
* **Target:** .NET 10

### Cobertura de tests

| Archivo | Servicio probado | Tests |
|---|---|---|
| `CabanaServiceTests.cs` | `CabanaService` | 7 |
| `PersonaServiceTests.cs` | `PersonaService` | 8 |
| `ClienteServiceTests.cs` | `ClienteService` | 8 |
| `ReservacionServiceTests.cs` | `ReservacionService` | 10 |
| `EmpleadoServiceTests.cs` | `EmpleadoService` | 11 |
| **Total** | | **44** |

### Cómo ejecutar las pruebas

**Todos los tests:**
```bash
dotnet test HiddenValley.Tests/HiddenValley.Tests.csproj
```

**Con detalle de cada test:**
```bash
dotnet test HiddenValley.Tests/HiddenValley.Tests.csproj --logger "console;verbosity=normal"
```

**Solo un servicio específico:**
```bash
dotnet test HiddenValley.Tests/HiddenValley.Tests.csproj --filter "CabanaServiceTests"
dotnet test HiddenValley.Tests/HiddenValley.Tests.csproj --filter "ReservacionServiceTests"
dotnet test HiddenValley.Tests/HiddenValley.Tests.csproj --filter "ClienteServiceTests"
dotnet test HiddenValley.Tests/HiddenValley.Tests.csproj --filter "PersonaServiceTests"
dotnet test HiddenValley.Tests/HiddenValley.Tests.csproj --filter "EmpleadoServiceTests"
```

**Desde la raíz de la solución:**
```bash
dotnet test HiddenValley.sln
```

### Principios F.I.R.S.T. aplicados

Los tests están diseñados siguiendo los principios F.I.R.S.T. que garantizan pruebas confiables y mantenibles:

**F — Fast (Rápidos)**
Cada test usa una base de datos en memoria (`UseInMemoryDatabase`). No hay conexiones reales a PostgreSQL, ni operaciones de red ni disco. Toda la suite de 44 tests corre en menos de 5 segundos.

**I — Isolated (Aislados)**
Cada test crea su propia instancia de `ApplicationDbContext` con un nombre de base de datos único generado con `Guid.NewGuid()`. Esto garantiza que ningún test comparte estado con otro, sin importar el orden de ejecución o si corren en paralelo.

```csharp
// Helpers/DbContextHelper.cs
.UseInMemoryDatabase($"{dbName}_{Guid.NewGuid()}")
```

**R — Repeatable (Repetibles)**
Los tests no dependen de datos externos, variables de entorno, fecha del sistema ni estado previo. Cada ejecución produce exactamente el mismo resultado.

**S — Self-validating (Auto-validables)**
Cada test tiene su propio `Assert` que determina si pasa o falla sin intervención manual. No se requiere revisar logs ni salidas externas.

**T — Timely (Oportunos)**
Los tests fueron escritos junto con el análisis de los servicios, cubriendo tanto los caminos exitosos (`RetornaSuccess`) como los casos de error y validaciones de negocio (`RetornaError`).

### Casos de prueba por servicio

**CabanaService**
- Registrar cabaña con tipo existente → éxito
- Registrar cabaña con tipo inexistente → error
- Eliminar cabaña sin reservaciones → éxito
- Eliminar cabaña con reservaciones asociadas → error
- Eliminar cabaña inexistente → error
- Cambiar estado a uno válido → éxito
- Cambiar estado a uno inválido → error

**PersonaService**
- Crear persona con DPI nuevo → éxito
- Crear persona con DPI duplicado → error
- Verificar existencia de DPI registrado → `true`
- Verificar existencia de DPI no registrado → `false`
- Actualizar persona existente → éxito y verifica el cambio
- Actualizar persona inexistente → error
- Eliminar persona sin vínculos → éxito
- Eliminar persona vinculada a cliente → error

**ClienteService**
- Crear cliente con persona existente y no registrada → éxito
- Crear cliente con persona inexistente → error
- Crear cliente con persona ya registrada como cliente → error
- Eliminar cliente sin historial de reservas → éxito
- Eliminar cliente con historial de reservas → error
- Eliminar cliente inexistente → error
- Actualizar teléfono de cliente existente → éxito y verifica el cambio
- Buscar cliente por teléfono → retorna DTO correcto
- Buscar con filtro inexistente → retorna `null`

**ReservacionService**
- Crear reservación con datos válidos → éxito, verifica noches y total
- Crear con fecha de salida menor a entrada → error
- Crear con cliente inexistente → error
- Crear con teléfono que no coincide → error
- Crear excediendo capacidad de la cabaña → error
- Crear con traslape de fechas en la misma cabaña → error
- Obtener reservación por ID existente → retorna DTO
- Obtener reservación por ID inexistente → retorna `null`
- Actualizar reservación cancelada → error
- Actualizar con estado inválido → error

**EmpleadoService**
- Crear empleado con datos válidos → éxito
- Crear con persona inexistente → error
- Crear con puesto inexistente → error
- Crear con persona ya registrada como empleado → error
- Eliminar empleado existente → éxito
- Eliminar empleado inexistente → error
- Actualizar teléfono de empleado existente → éxito y verifica el cambio
- Actualizar empleado inexistente → error
- Actualizar con nuevo puesto inexistente → error

---

## Autores
- Jorge Danilo Ucelo
- Christian Eduardo Lopez Lemus
- Delmi Maria Fajardo
- Keily Atalia Lopez Hernandez
- Cristian Eduardo Chamo Morales
- Ludin Eduardo Carranza Guerra 

ESTUDIANTES DE INGENIERIA EN SISTEMAS DE LA UNIVERSIDAD MARIANO GALVEZ DE GUATEMALA SEDE EN JALAPA, JALAPA
