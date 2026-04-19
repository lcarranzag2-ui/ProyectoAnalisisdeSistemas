CREATE TABLE IF NOT EXISTS Usuarios (
    Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
    NombreCompleto      TEXT    NOT NULL,
    CorreoElectronico   TEXT    NOT NULL UNIQUE,
    Telefono            TEXT    NOT NULL,
    ContrasenaHash      TEXT    NOT NULL,
    Rol                 TEXT    NOT NULL DEFAULT 'Cliente',
    FechaRegistro       TEXT    NOT NULL,
    Activo              INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Cabanas (
    Id                      INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre                  TEXT    NOT NULL UNIQUE,
    Descripcion             TEXT    NOT NULL,
    Capacidad               INTEGER NOT NULL CHECK (Capacidad > 0),
    NumeroHabitaciones      INTEGER NOT NULL CHECK (NumeroHabitaciones > 0),
    PrecioPorNoche          REAL    NOT NULL CHECK (PrecioPorNoche >= 0),
    Ubicacion               TEXT    NOT NULL,
    EstadoDisponibilidad    TEXT    NOT NULL DEFAULT 'Disponible',
    FechaRegistro           TEXT    NOT NULL,
    Activa                  INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Reservas (
    Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
    UsuarioId           INTEGER NOT NULL,
    CabanaId            INTEGER NOT NULL,
    FechaEntrada        TEXT    NOT NULL,
    FechaSalida         TEXT    NOT NULL,
    NumeroHuespedes     INTEGER NOT NULL CHECK (NumeroHuespedes > 0),
    MontoTotal          REAL    NOT NULL CHECK (MontoTotal >= 0),
    Estado              TEXT    NOT NULL DEFAULT 'Pendiente',
    FechaCreacion       TEXT    NOT NULL,
    Observaciones       TEXT    NULL,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios (Id) ON DELETE RESTRICT,
    FOREIGN KEY (CabanaId)  REFERENCES Cabanas  (Id) ON DELETE RESTRICT,
    CHECK (date(FechaSalida) > date(FechaEntrada))
);

CREATE INDEX IF NOT EXISTS IX_Reservas_UsuarioId ON Reservas (UsuarioId);
CREATE INDEX IF NOT EXISTS IX_Reservas_CabanaId  ON Reservas (CabanaId);
CREATE INDEX IF NOT EXISTS IX_Reservas_Fechas    ON Reservas (FechaEntrada, FechaSalida);
CREATE INDEX IF NOT EXISTS IX_Usuarios_Correo    ON Usuarios (CorreoElectronico);
