select * from  TipoServicio s;
select * from servicio;

CREATE TABLE TipoServicio (
  IdTipoServicio SERIAL PRIMARY KEY,
  Nombre VARCHAR(50) NOT NULL,
  Descripcion TEXT
);

ALTER TABLE servicio 
ADD COLUMN idtiposervicio INT;

ALTER TABLE servicio
ADD CONSTRAINT fk_servicio_tiposervicio
FOREIGN KEY (idtiposervicio) 
REFERENCES tiposervicio(idtiposervicio)
ON DELETE RESTRICT;