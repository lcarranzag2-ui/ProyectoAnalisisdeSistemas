# Hidden Valley


## Descripción del Proyecto

Sistema web desarrollado para la gestión y administración integral del turicentro Hidden Valley, ubicado en Monjas, Jalapa, Guatemala. El proyecto surge con el propósito de modernizar y digitalizar los procesos administrativos relacionados con el control de cabañas, visitantes y reservaciones, reemplazando métodos manuales por una plataforma centralizada, segura y eficiente.

La aplicación permite llevar un registro organizado de los visitantes, administrar la disponibilidad de cabañas, controlar reservaciones y gestionar información relevante del negocio en tiempo real. Además, facilita el acceso rápido a la información, mejora la organización interna y optimiza los procesos operativos del turicentro.

El sistema fue diseñado utilizando una arquitectura moderna basada en microservicios y contenedores Docker, integrando un backend desarrollado en .NET 9, una API REST para la comunicación entre servicios, un frontend interactivo construido con Blazor y una base de datos PostgreSQL administrada mediante Entity Framework Core. Esta arquitectura permite que la aplicación sea escalable, mantenible y fácil de desplegar en diferentes entornos.

## Funcionalidades Principales

* Gestión y registro de visitantes
* Administración de cabañas y disponibilidad
* Control de reservaciones
* Gestión de usuarios y autenticación
* Persistencia y consulta de datos en tiempo real
* Comunicación entre frontend y backend mediante API REST
* Documentación de endpoints con Swagger
* Contenerización completa con Docker para facilitar la implementación
---
# Características Principales

* API RESTful
* Frontend interactivo con Blazor
* Persistencia con PostgreSQL
* Contenedores Docker
* Documentación Swagger
* Entity Framework Core
* Pruebas unitarias
* Pruebas de Integracion
* Pruebas End to End
---

## Objetivo del Proyecto

El objetivo principal del proyecto es aplicar conocimientos de análisis, diseño y desarrollo de software en un entorno real, implementando buenas prácticas de arquitectura, bases de datos, desarrollo web y despliegue de aplicaciones modernas.

Asimismo, el sistema busca proporcionar una solución tecnológica que facilite la administración del turicentro Hidden Valley, optimizando procesos internos, mejorando la organización de la información y brindando una mejor experiencia tanto para administradores como para visitantes.

Este proyecto fue desarrollado como parte del curso de Análisis de Sistemas de la carrera de Ingeniería en Sistemas en la Universidad Mariano Gálvez de Guatemala, sede Jalapa.


---


# Arquitectura del Proyecto

```text
PROYECTOANALISISDESISTEMAS/
│
├── HiddenValley/
│   ├── data/                    
│   ├── HiddenValley/           
│   ├── HiddenValley.API/       
│   ├── HiddenValley.Frontend/  
│   ├── HiddenValley.Shared/     
│   ├── HiddenValley.Tests/     
│   ├── test/                   
│   ├── docker-compose.yml       
│   ├── HiddenValley.sln        
│   └── README.md
```

---

# Tecnologías Utilizadas

| Tecnología            | Descripción                   |
| --------------------- | ----------------------------- |
| .NET 9                | Desarrollo backend y frontend |
| ASP.NET Core Web API  | Servicios REST                |
| Blazor                | Frontend interactivo          |
| Entity Framework Core | ORM y acceso a datos          |
| PostgreSQL            | Base de datos                 |
| Docker                | Contenedores                  |
| Docker Compose        | Orquestación                  |
| Swagger               | Documentación de API          |
DBeaver                 | Cliente para administrar la base de datos

---

# Requisitos Previos

Antes de ejecutar el proyecto debes tener instalado:

* [Docker Desktop](https://www.docker.com/products/docker-desktop/?utm_source=chatgpt.com)
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0?utm_source=chatgpt.com)
* [Visual Studio Code](https://code.visualstudio.com/?utm_source=chatgpt.com), [Visual Studio 2022](https://visualstudio.microsoft.com/es/vs/?utm_source=chatgpt.com) o [JetBrains Rider](https://www.jetbrains.com/rider/)
* [DBeaver](https://dbeaver.io/) 


Instalar Entity Framework Tools:

```bash
dotnet tool install --global dotnet-ef
```

---

# Instalación del Proyecto

## 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/HiddenValley.git
cd HiddenValley
```

---

# Ejecución del Proyecto


## Levantar contenedores con Docker

Ubicarse en la carpeta donde está el archivo `docker-compose.yml`

```bash
docker-compose up --build
```

### Ejecutar en segundo plano

```bash
docker-compose up -d
```

### Detener contenedores

```bash
docker-compose down
```

---

# Base de Datos

El proyecto utiliza PostgreSQL junto con Entity Framework Core para la persistencia y administración de datos. Además, se utiliza DBeaver como cliente gráfico para la administración y consulta de la base de datos.

## Configuración de Conexión en DBeaver

| Parámetro | Valor |
|---|---|
| Host | localhost |
| Puerto | 5432 |
| Base de Datos | HiddenValleyDB |
| Usuario | postgres |
| Contraseña | postgres |

> Los valores pueden cambiar dependiendo de la configuración definida en el archivo `docker-compose.yml` o en las variables de entorno del proyecto.

## Aplicar migraciones

```bash
dotnet ef database update
```

## Crear nueva migración

```bash
dotnet ef migrations add NombreMigracion
```
---



# Arquitectura de la API

Este proyecto contiene la lógica central del backend (Web API) construida sobre ASP.NET Core. Sigue un patrón de arquitectura limpia dividida por capas para separar los controladores de la lógica de negocio y el acceso a datos.

---

## Estructura del Proyecto y Propósito

```text
HiddenValley.API/
│
├── Controllers/          - Endpoints HTTP
├── Data/                 - Base de Datos y Contexto
├── Interfaces/           - Contratos de Servicios
├── Models/               - Entidades físicas de la base de datos
├── Properties/           - Configuraciones de entorno de ejecución local
└── Services/             - Lógica de control y mapeos

```

---

## ¿Que funcion cumple cada carpeta?

### Controllers

Es el punto de entrada de la API. Contiene las clases que heredan de `ControllerBase`. Sus únicas responsabilidades son:

* Recibir las solicitudes HTTP (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`).
* Validar los datos de entrada básicos de los DTOs.
* Inyectar las interfaces de los servicios correspondientes para delegar la lógica pesada.
* Retornar los códigos de estado HTTP adecuados (`200 OK`, `201 Created`, `400 BadRequest`, `404 NotFound`).

### Data

Contiene toda la infraestructura relacionada con Entity Framework Core. Aquí se encuentra:

* **`ApplicationDbContext.cs`**: El corazón del ORM que mapea tus modelos hacia las tablas reales en la base de datos, define las relaciones de llaves foráneas y configura comportamientos del motor de base de datos.
* **Migrations/** *(si aplica)*: Los archivos generados de forma automática para actualizar el esquema de tu base de datos mediante la CLI de .NET.

### Interfaces

Es la capa encargada de desacoplar el código para facilitar las pruebas unitarias y de integración. Aquí se definen los contratos puros (ej. `IServicioService`, `IDashboardService`).

* Evita que el controlador dependa de una implementación de código dura, permitiendo que en los entornos de prueba se puedan simular (`Mockear`) o sustituir comportamientos de manera ágil.

### Models

Representa el dominio del negocio y la estructura del hotel. Contiene las clases puras que representan fielmente las tablas de PostgreSQL (ej. `Cabana`, `RegistroReservacion`, `Servicio`).

* Aquí se mapean las propiedades físicas, tipos de datos, nulabilidades y las propiedades de navegación relacionales necesarias para hacer las consultas con LINQ.

### Properties

Contiene archivos de configuración internos del entorno de desarrollo de Visual Studio o VS Code, principalmente **`launchSettings.json`**.

* Define los perfiles de ejecución del servidor local, los puertos asignados (`http`/`https`), y variables de entorno iniciales.

### Services

Es donde vive toda la lógica de negocio y las reglas del complejo turicentro. Los controladores no operan datos directamente; invocan a las clases de esta carpeta (que implementan las interfaces). Sus funciones principales son:

* Filtrar, paginar, calcular totales (como en el `DashboardService`).
* Transformar las entidades de base de datos (`Models`) hacia los objetos de transferencia de datos (`Shared.DTOs`) antes de mandarlos al Frontend en Blazor.
* Manejar transacciones concurrentes o validaciones lógicas antes de guardar cambios.

---

## Archivos del Sistema de la Raíz

* `appsettings.json` / `appsettings.Development.json**`: Archivos de configuración general donde se almacenan las cadenas de conexión (`ConnectionStrings`) seguras a SQL Server, niveles de logueo y claves de APIs.
* **`.env`**: Archivo local para almacenar credenciales o variables de entorno críticas de forma aislada sin subirlas al repositorio de Git.
* **`Dockerfile`**: Archivo de instrucciones para empaquetar la API en un contenedor de Docker listo para producción o despliegue en la nube.
* **`Program.cs`**: El punto de arranque de la aplicación de .NET. Aquí se configuran los servicios del sistema, se inyectan las dependencias (Inyección de Dependencias), se configuran los CORS (para permitir peticiones de Blazor) y se define el pipeline de middleware HTTP.

---
---
# Arquitectura del Frontend

Este proyecto contiene la interfaz de usuario (UI) y la lógica del cliente desarrollada con Blazor. Utiliza un enfoque basado en componentes reutilizables y un consumo asíncrono de servicios REST para interactuar con la API.

---

## Estructura del Proyecto y Propósito

```text
HiddenValley.Frontend/
│
├── Interfaces/          - Consumo de API
├── Layout/              - Diseños de página globales y menús de navegación
├── Pages/               - Vistas y pantallas del sistema (.razor)
├── Properties/          - Configuraciones del servidor de desarrollo local
├── Service/             - Implementaciones HTTP que conectan con la API
└── wwwroot/             - Archivos estáticos públicos (CSS, imágenes, JS)

```

---

## ¿Que funcion cumple cada carpeta?

### Interfaces

Define las firmas y los contratos que los servicios del cliente deben cumplir (ej. `IServicioClient`, `ICabanaClient`).

* Permite desacoplar las vistas de la lógica de red. Si mañana cambia la forma de consumir los datos, los componentes de Blazor no se ven afectados mientras se respete la interfaz.

### Layout

Aloja los componentes de diseño estructural de la aplicación que se comparten entre múltiples páginas.

* **`MainLayout.razor`**: Define la plantilla general de la aplicación (la barra lateral de navegación, el encabezado superior y el contenedor donde se renderizan las páginas).
* **`NavMenu.razor`**: Contiene el menú principal con los enlaces a los diferentes módulos de Hidden Valley.

### Pages

Es el núcleo visual del usuario. Cada archivo `.razor` aquí representa una pantalla o vista en el navegador web (ej. `Cabanas.razor`, `Reservaciones.razor`).

* Utilizan la directiva `@page` para definir sus rutas de navegación.
* Manejan los ciclos de vida de Blazor (`OnInitializedAsync`) para cargar la información tan pronto como el usuario entra a la pantalla.
* Capturan los eventos de los formularios (creación, edición y borrado) mediante binding bidireccional (`@bind`).

### Properties

Al igual que en la API, contiene el archivo **`launchSettings.json`**.

* Configura los puertos locales y los perfiles de ejecución para depurar el sitio web de manera local desde el navegador.

### Service

Contiene la implementación real de la comunicación por red utilizando **`HttpClient`**.

* Son las clases encargadas de realizar las peticiones HTTP (`GetFromJsonAsync`, `PostAsJsonAsync`, `DeleteAsync`) hacia los endpoints correspondientes de tu API.
* Aquí se interceptan las cabeceras personalizadas de paginación (como `X-Total-Records`) enviadas por el backend para mapearlas y controlar los controles de tablas en la UI.

### wwwroot

Es la carpeta de activos estáticos del navegador. Todo lo que esté aquí se sirve públicamente de forma directa.

* Contiene las hojas de estilo CSS (archivos de diseño, Bootstrap o Tailwind), fuentes tipográficas, iconos del sistema e imágenes o logos del turicentro Hidden Valley.

---

## Archivos del Sistema de la Raíz

* **`_Imports.razor`**: Archivo central donde se declaran los `using` globales del frontend. Evita tener que importar las mismas librerías y DTOs al inicio de cada archivo `.razor`.
* **`App.razor`**: El componente raíz de Blazor que inicializa el enrutador (`Router`). Se encarga de buscar la página correspondiente a la URL que el usuario escribió y de pintar un mensaje de error si la ruta no existe.
* **`Dockerfile`**: Instrucciones de compilación para empaquetar el frontend en un contenedor liviano (usando Nginx o hosting estático) listo para producción.
* **`Program.cs`**: Configura el contenedor de dependencias del cliente. Aquí se registra la URL base de la API (`builder.Services.AddScoped(sp => new HttpClient { BaseAddress = ... })`) y se enlazan los servicios de la carpeta `Service` con sus respectivas interfaces.

---

---

# Arquitectura Proyecto Compartido (Shared)

Este proyecto funciona como un núcleo común de dependencias cross-cutting que es referenciado tanto por el Backend (HiddenValley.API) como por el Frontend (HiddenValley.Frontend). Su objetivo principal es asegurar que ambas capas hablen el mismo idioma sin duplicar código.

---

## Estructura del Proyecto y Propósito

```text
HiddenValley.Shared/
│
└── DTOs/                # Objetos de Transferencia de Datos (Data Transfer Objects)

```

---

## Que funcion cumple cada carpeta?

### DTOs

Contiene clases planas de datos (DTOs) que sirven exclusivamente para transportar información a través de la red mediante formato JSON.

* **Separación de Capas:** Evita exponer las entidades físicas de la base de datos (`Models`) directamente al cliente Blazor por razones de seguridad y rendimiento.
* **Modelos de Entrada:** Clases destinadas a la creación o actualización de registros que solo llevan las propiedades requeridas (ej. `ServicioCreateDto`, `TipoCabanaCreateDTO`).
* **Modelos de Salida:** Clases optimizadas para la lectura con datos ya formateados, unificados o calculados listos para ser renderizados en las tablas del Frontend (ej. `ServicioReadDto`, `DashboardResumenDto`).

---

## Características Técnicas del Proyecto

* **Librería de Clases (.NET Standard / .NET 9):** Al ser una biblioteca de clases pura, no contiene lógica de ejecución (como un método `Main` o controladores), lo que le permite ser compilada e importada de manera ligera en cualquier entorno de la solución.
* **Validaciones Centralizadas:** Si utilizas `DataAnnotations` (como `[Required]`, `[StringLength]` o `[Range]`) sobre las propiedades de estos DTOs, la validación se ejecuta automáticamente de forma simétrica:
1. **En el Frontend:** Blazor la usa en sus componentes `<DataAnnotationsValidator>` para mostrar mensajes de error al usuario en tiempo real antes de enviar el formulario.
2. **En el Backend:** La API la procesa mediante `ModelState` para rechazar peticiones corruptas, asegurando una validación blindada en ambos extremos de la aplicación.


---

# Ejecutar Manualmente por Proyecto

## Ejecutar Backend API

Entrar al proyecto API:

```bash
cd HiddenValley.API
```

Restaurar paquetes:

```bash
dotnet restore
```

Ejecutar migraciones:

```bash
dotnet ef database update
```

Ejecutar API:

```bash
dotnet run
```

La API quedará disponible en:

```text
http://localhost:5017/swagger/index.html
```

---

## Ejecutar Frontend Blazor

Abrir otra terminal:

```bash
cd HiddenValley.Frontend
```

Restaurar dependencias:

```bash
dotnet restore
```

Ejecutar aplicación:

```bash
dotnet run
```

Frontend disponible en:

```text
http://localhost:7075
```

---

# Documentación Swagger

Swagger permite probar los endpoints de la API desde el navegador.

Instalar Swagger:

```bash
dotnet add package Swashbuckle.AspNetCore
```

Acceder desde:

```text
http://localhost:5017/swagger/index.html
```

---
# Accesos del Sistema

| Servicio        | URL                                                            |
| --------------- | -------------------------------------------------------------- |
| Frontend Blazor | [http://localhost:7075](http://localhost:7075)                 |
| API REST        | [http://localhost:7084](http://localhost:7084)                 |
| Swagger         | [http://localhost:7084/swagger](http://localhost:7084/swagger) |                                               |
---

# Pruebas Unitarias — `HiddenValley.Tests`

El proyecto incluye pruebas dentro de:

```text
HiddenValley.Tests/
```


Proyecto de pruebas unitarias independiente que valida la capa de servicios de la API sin acceso a base de datos real. Utiliza EF Core InMemory como mock del contexto de base de datos.

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

# Pruebas de Integracion
Este módulo contiene la suite de pruebas de integración para la API de Hidden Valley. Las pruebas están diseñadas para validar el comportamiento de los endpoints, las reglas de negocio del servidor y la consistencia de la base de datos relacional simulada en memoria.

Muevete a la carpeta test
```bash
cd test
```

El proyecto incluye pruebas dentro de:

``` bash
cd HiddenValley.IntegrationTests/
```


## Tecnologías Utilizadas

* **xUnit:** Framework de pruebas unitarias y de integración.
* **Microsoft.AspNetCore.Mvc.Testing:** Permite la creación de servidores de prueba en memoria (`WebApplicationFactory`) para simular peticiones HTTP reales.
* **Microsoft.EntityFrameworkCore.InMemory:** Proveedor de base de datos en memoria para aislar los entornos de prueba de la base de datos de producción o desarrollo.

---

## Estructura de la carpeta

```text
test/HiddenValley.IntegrationTests/
│
├── CustomWebApplicationFactory.cs      # Configuración del servidor de pruebas y BD en memoria
├── README.md                           # Documentación de las pruebas (Este archivo)
│
└── Controllers/                        # Controladores bajo prueba
    ├── CabanasControllerTests.cs       # Pruebas de estados, filtros y disponibilidad
    ├── DashboardControllerTests.cs    # Pruebas de métricas, totales diarios y excepciones
    ├── ReservacionServiciosControllerTests.cs  # Pruebas de consumos por reservación
    ├── ServiciosControllerTests.cs     # Pruebas del catálogo maestro y paginación para Blazor
    └── TipoCabanaControllerTests.cs    # Pruebas de integridad referencial y borrado de tipos

```

---

## Algunos de los Escenarios de Prueba Clave

### 1. Gestión de Reservaciones y Consumos (`ReservacionServicios`)

* **Acumulación de Cantidades:** Valida que si se agrega un servicio que ya existía en la habitación, el sistema sume la cantidad en lugar de duplicar la fila.
* **Estructura de Datos Agrupada:** Asegura que las consultas traigan de forma consistente las relaciones de clientes, habitaciones y catálogos de servicios.

### 2. Catálogo Maestro de Servicios (`Servicios`)

* **Paginación Segura:** Comprueba el cálculo de registros y páginas inyectando las cabeceras HTTP personalizadas (`X-Total-Records`, `X-Total-Pages`) necesarias para el renderizado de tablas en Blazor.
* **Actualizaciones Parciales (`PATCH`):** Verifica que la API altere únicamente los campos enviados (ej. cambiar solo el precio), manteniendo intactos el nombre y la descripción originales.

### 3. Tipos de Cabaña e Integridad (`TipoCabana`)

* **Control de Eliminación Rígido:** Valida que el sistema bloquee el borrado de un tipo de cabaña si existen registros físicos en la tabla de `Cabanas` que dependan de él, retornando un código `400 BadRequest`.

### 4. Métricas de Control (`Dashboard`)

* **Cálculo en Tiempo Real:** Verifica el conteo exacto de cabañas ocupadas hoy, cabañas disponibles y la suma de personas que realizan Check-In en la fecha actual.
* **Filtros de Exclusión:** Asegura que el servicio ignore por completo los registros con estado `"Cancelada"`.

---

## Ejecución de las Pruebas

Para compilar el proyecto y ejecutar toda la suite de pruebas desde la terminal, posicionate en la raíz de la solución o de la carpeta de pruebas y ejecuta el siguiente comando:

```bash
dotnet test

```

### Comandos Avanzados

* **Ejecutar un archivo de pruebas específico:**
```bash
dotnet test --filter "FullyQualifiedName~DashboardControllerTests"

```


* **Ejecutar una prueba individual:**
```bash
dotnet test --filter "FullyQualifiedName~Create_ConDatosValidos_DeberiaRegistrarServicioYRetornarOk"

```

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

## Prácticas Implementadas

1. **Aislamiento de Datos (Arrange):** Cada prueba ejecuta un bloque de limpieza explícito (`RemoveRange`) sobre las tablas maestras y relacionales antes de sembrar información nueva, garantizando que el orden de ejecución no altere los resultados.
2. **Uso del Bloque `using(var scope)`:** La resolución de servicios del DbContext se maneja a través de alcances (`IServiceScope`) efímeros independientes para simular el ciclo de vida real de las solicitudes HTTP de ASP.NET Core.
3. **Verificación en Dos Pasos (Assert):** No solo se valida el código de estado de la respuesta HTTP (`Ok`, `Created`, `NoContent`), sino que se realiza una consulta directa a la base de datos para confirmar que los cambios se hayan persistido físicamente.


---

# Autores

* Jorge Danilo Ucelo Roman
* Christian Eduardo López Lemus
* Delmi María Fajardo Borrayo
* Cristian Eduardo Chamo Morales
* Keily Atalia López Hernández
* Ludin Eduardo Carranza Guerra

---

# Universidad

Proyecto desarrollado por estudiantes de Ingeniería en Sistemas de la:

Universidad Mariano Gálvez de Guatemala
Sede Jalapa, Guatemala.
