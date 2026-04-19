# Hidden Valley - Configuración Inicial (PROYECT-54)

Inicialización de arquitectura y base de datos local para el sistema de reservaciones de cabañas Hidden Valley.

## Arquitectura

Proyecto estructurado por capas siguiendo separación de responsabilidades:

```
HiddenValley.sln
├── HiddenValley.Domain     → Entidades del dominio (Usuario, Cabana, Reserva)
├── HiddenValley.Data       → Acceso a datos, SQLite, scripts y repositorios
├── HiddenValley.Services   → Orquestación de arranque del sistema
└── HiddenValley.App        → Consola de verificación
```

## Motor de base de datos

Se utiliza **SQLite** a través de `Microsoft.Data.Sqlite`. Razones:

- 100% local, no requiere servidor ni conexión a internet
- El archivo `.db` se guarda en disco y sobrevive a reinicios del equipo
- Modo WAL activado para mayor resistencia a cortes de energía
- Restricciones de integridad referencial habilitadas (`PRAGMA foreign_keys = ON`)

## Ubicación del archivo de base de datos

```
%LOCALAPPDATA%\HiddenValley\HiddenValley.db
```

Se almacena en la carpeta local del usuario, lo que garantiza persistencia permanente.

## Tablas principales

El script `Scripts/SchemaInicial.sql` crea:

- **Usuarios** – clientes y administradores del sistema
- **Cabanas** – catálogo de cabañas disponibles
- **Reservas** – reservaciones con llaves foráneas a Usuarios y Cabanas

Incluye índices en las columnas más consultadas y restricciones `CHECK` para validar datos a nivel de base de datos.

## Ejecución

```bash
dotnet restore
dotnet build
dotnet run --project HiddenValley.App
```

## Validación de persistencia

1. Ejecutar la aplicación por primera vez → se crea la base de datos e inserta una cabaña de prueba
2. Cerrar la aplicación completamente
3. Reiniciar la computadora
4. Ejecutar la aplicación nuevamente → la cabaña sigue ahí y el contador refleja los registros anteriores

## Criterios de aceptación cumplidos

- [x] Motor de base de datos local configurado (SQLite)
- [x] Script inicial con tablas Cabañas, Reservas y Usuarios
- [x] Persistencia garantizada ante reinicios y cierre del sistema
