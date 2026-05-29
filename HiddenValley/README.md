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

## Pruebas E2E

El proyecto cuenta con pruebas end-to-end reales que verifican el sistema completo: desde el cliente HTTP hasta la API y la base de datos PostgreSQL, sin mockear nada.

### Herramientas utilizadas

* **pytest** — framework para correr las pruebas
* **requests** — para las pruebas de API (llamadas HTTP directas)
* **playwright** — para las pruebas de frontend (navegador real)
* **Python 3** — lenguaje base

### Instalación de dependencias

```bash
python -m pip install pytest requests pytest-playwright
python -m playwright install chromium
```

### Archivos de prueba

```
tests/
├── conftest.py               # configuración base y fixture de sesión
├── test_reservaciones.py     # pruebas de API (7 pruebas)
└── test_frontend.py          # pruebas de UI con Playwright (10 pruebas)
```

### Cómo correr las pruebas

Con Docker levantado y los datos base insertados, desde la raíz del proyecto:

```bash
# Solo pruebas de API
python -m pytest tests/test_reservaciones.py -v

# Solo pruebas de frontend
python -m pytest tests/test_frontend.py -v

# Pruebas de frontend con navegador visible
python -m pytest tests/test_frontend.py -v --headed

# Todas las pruebas juntas
python -m pytest tests/ -v

# Generar reporte HTML
python -m pytest tests/ -v --html=reporte.html
```

### Cobertura de las pruebas de API

| Prueba | Endpoint | Descripción |
|---|---|---|
| `test_crear_persona` | `POST /api/personas` | Crea una persona con DPI único |
| `test_buscar_cliente_existente` | `GET /api/clientes/buscar` | Busca cliente por teléfono |
| `test_listar_cabanas` | `GET /api/cabanas` | Lista todas las cabañas |
| `test_crear_reservacion_exitosa` | `POST /api/reservaciones` | Flujo completo de reservación |
| `test_reservacion_fechas_invalidas` | `POST /api/reservaciones` | Valida que fechas inválidas retornen 400 |
| `test_listar_reservaciones` | `GET /api/reservaciones` | Lista todas las reservaciones |
| `test_dashboard` | `GET /api/dashboard` | Verifica métricas del día |

### Cobertura de las pruebas de frontend

| Prueba | Página | Descripción |
|---|---|---|
| `test_dashboard_carga` | `/dashboard` | El dashboard carga correctamente |
| `test_dashboard_tiene_tabla` | `/dashboard` | Se muestra el resumen de operaciones |
| `test_cabanas_carga` | `/cabanas` | La página de cabañas carga con su título |
| `test_cabanas_boton_registrar_visible` | `/cabanas` | El botón de registro es visible |
| `test_cabanas_abrir_modal_registro` | `/cabanas` | El modal de nueva cabaña se abre |
| `test_cabanas_cerrar_modal` | `/cabanas` | El modal se puede cerrar |
| `test_reservaciones_carga` | `/reservaciones` | La página de reservaciones carga |
| `test_reservaciones_boton_nueva_visible` | `/reservaciones` | El botón Nueva Reservación es visible |
| `test_reservaciones_abrir_modal_crear` | `/reservaciones` | El modal de crear reservación se abre |
| `test_reservaciones_buscador` | `/reservaciones` | El buscador de reservaciones funciona |

### Bug encontrado durante las pruebas

Durante la ejecución de las pruebas E2E se detectó un bug en `ReservacionService.cs` línea 128 donde una línea de prueba (`cabana.IdCabana = 2`) fue dejada accidentalmente en producción, causando un `InvalidOperationException` al intentar modificar la clave primaria de una entidad trackeada por Entity Framework. El bug fue identificado por las pruebas con error 500 y corregido eliminando dicha línea.

---

## Autores
- Jorge Danilo Ucelo
- Christian Eduardo Lopez Lemus
- Delmi Maria Fajardo
- Keily Atalia Lopez Hernandez
- Cristian Eduardo Chamo Morales
- Ludin Eduardo Carranza Guerra 

ESTUDIANTES DE INGENIERIA EN SISTEMAS DE LA UNIVERSIDAD MARIANO GALVEZ DE GUATEMALA SEDE EN JALAPA, JALAPA