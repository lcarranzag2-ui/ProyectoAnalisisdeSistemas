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

## Autores
- Jorge Danilo Ucelo
- Christian Eduardo Lopez Lemus
- Delmi Maria Fajardo
- Keily Atalia Lopez Hernandez
- Cristian Eduardo Chamo Morales
- Ludin Eduardo Carranza Guerra (Módulo Reservaciones — PROYECT-60)

ESTUDIANTES DE INGENIERIA EN SISTEMAS DE LA UNIVERSIDAD MARIANO GALVEZ DE GUATEMALA SEDE EN JALAPA, JALAPA
