CREATE TABLE Persona(
  IdPersona SERIAL PRIMARY KEY,
  Nombres VARCHAR(100) NOT NULL,
  Apellidos VARCHAR(100) NOT NULL,
  FechaNacimiento DATE,
  FechaRegistro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  DPI VARCHAR(20) UNIQUE,
  Telefono VARCHAR(15) NOT NULL,
  Gmail VARCHAR(100) UNIQUE,
  Direccion TEXT NOT NULL
);

CREATE TABLE PuestoTrabajo(
  IdPuestoTrabajo SERIAL PRIMARY KEY,
  Nombre VARCHAR(50) NOT NULL,
  Descripcion TEXT
);

CREATE TABLE EstadoCabana(
  IdEstadoCabana SERIAL PRIMARY KEY,
  Nombre VARCHAR(50) NOT NULL UNIQUE,
  Descripcion TEXT
);

CREATE TABLE Empleado(
  IdEmpleado SERIAL PRIMARY KEY,
  IdPersona INTEGER UNIQUE NOT NULL,
  IdPuestoTrabajo INTEGER NOT NULL,
  UserId UUID UNIQUE NULL,
  CONSTRAINT fk_persona FOREIGN KEY(IdPersona) REFERENCES Persona(IdPersona) ON DELETE RESTRICT,
  CONSTRAINT fk_puesto FOREIGN KEY(IdPuestoTrabajo) REFERENCES PuestoTrabajo(IdPuestoTrabajo) ON DELETE RESTRICT
);

CREATE TABLE Cliente(
  IdCliente SERIAL PRIMARY KEY,
  IdPersona INTEGER UNIQUE NOT NULL,
  CONSTRAINT fk_persona_cliente FOREIGN KEY(IdPersona) REFERENCES Persona(IdPersona) ON DELETE RESTRICT
);

CREATE TABLE TipoCabana(
  IdTipoCabana SERIAL PRIMARY KEY,
  Nombre VARCHAR(50) NOT NULL,
  Descripcion TEXT,
  Capacidad INTEGER NOT NULL,
  Precio DECIMAL(10,2) NOT NULL CHECK(Precio >= 0)
);

CREATE TABLE Cabana(
  IdCabana SERIAL PRIMARY KEY,
  IdTipoCabana INTEGER NOT NULL,
  IdEstadoCabana INTEGER NOT NULL,
  CONSTRAINT fk_tipo FOREIGN KEY(IdTipoCabana) REFERENCES TipoCabana(IdTipoCabana) ON DELETE RESTRICT,
  CONSTRAINT fk_estado FOREIGN KEY(IdEstadoCabana) REFERENCES EstadoCabana(IdEstadoCabana) ON DELETE RESTRICT
);

CREATE TABLE TipoServicio (
  IdTipoServicio SERIAL PRIMARY KEY,
  Nombre VARCHAR(50) NOT NULL,
  Descripcion TEXT
);

CREATE TABLE Servicio (
  IdServicio SERIAL PRIMARY KEY,
  IdTipoServicio INTEGER,
  Nombre VARCHAR(50) NOT NULL UNIQUE,
  Descripcion TEXT,
  Precio DECIMAL(10,2) NOT NULL CHECK(Precio >= 0),
  CONSTRAINT fk_tiposervicio FOREIGN KEY(IdTipoServicio) REFERENCES TipoServicio(IdTipoServicio) ON DELETE RESTRICT
);

CREATE TABLE RegistroReservacion(
  Id SERIAL PRIMARY KEY,
  FechaEntrada DATE NOT NULL,
  FechaSalida DATE NOT NULL,
  CantidadPersonas INTEGER NOT NULL CHECK(CantidadPersonas > 0),
  EstadoReserva VARCHAR(20) NOT NULL CHECK(EstadoReserva IN ('Recibida','Confirmada','Cancelada','Pagada')),
  TotalPagar DECIMAL(10,2) NOT NULL CHECK(TotalPagar >= 0),
  IdCliente INTEGER NOT NULL,
  IdCabana INTEGER NOT NULL,
  IdEmpleado INTEGER NOT NULL,
  CONSTRAINT fk_cliente FOREIGN KEY(IdCliente) REFERENCES Cliente(IdCliente) ON DELETE RESTRICT,
  CONSTRAINT fk_cabana FOREIGN KEY(IdCabana) REFERENCES Cabana(IdCabana) ON DELETE RESTRICT,
  CONSTRAINT fk_empleado FOREIGN KEY(IdEmpleado) REFERENCES Empleado(IdEmpleado) ON DELETE RESTRICT,
  CHECK(FechaSalida > FechaEntrada)
);

CREATE TABLE ReservacionServicio(
  IdReservacion INTEGER NOT NULL,
  IdServicio INTEGER NOT NULL,
  Cantidad INTEGER NOT NULL CHECK(Cantidad > 0),
  PRIMARY KEY (IdReservacion, IdServicio),
  CONSTRAINT fk_reserva FOREIGN KEY(IdReservacion) REFERENCES RegistroReservacion(Id) ON DELETE RESTRICT,
  CONSTRAINT fk_servicio FOREIGN KEY(IdServicio) REFERENCES Servicio(IdServicio) ON DELETE RESTRICT
);

CREATE TABLE BitacoraEstados(
  IdBitacoraEstado SERIAL PRIMARY KEY,
  IdEstadoAnterior INTEGER NOT NULL,
  IdEstadoNuevo INTEGER NOT NULL,
  FechaCambio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  IdCabana INTEGER NOT NULL,
  IdEmpleado INTEGER NOT NULL,
  CONSTRAINT fk_est_ant FOREIGN KEY(IdEstadoAnterior) REFERENCES EstadoCabana(IdEstadoCabana) ON DELETE RESTRICT,
  CONSTRAINT fk_est_nue FOREIGN KEY(IdEstadoNuevo) REFERENCES EstadoCabana(IdEstadoCabana) ON DELETE RESTRICT,
  CONSTRAINT fk_cabana_bit FOREIGN KEY(IdCabana) REFERENCES Cabana(IdCabana) ON DELETE RESTRICT,
  CONSTRAINT fk_emp_bit FOREIGN KEY(IdEmpleado) REFERENCES Empleado(IdEmpleado) ON DELETE RESTRICT
);
