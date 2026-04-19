using HiddenValley.Data.Context;
using HiddenValley.Domain.Entities;

namespace HiddenValley.Data.Repositories;

public class CabanaRepositorio : ICabanaRepositorio
{
    private readonly IConexionFactory _conexionFactory;

    public CabanaRepositorio(IConexionFactory conexionFactory)
    {
        _conexionFactory = conexionFactory;
    }

    public int Agregar(Cabana cabana)
    {
        using var conexion = _conexionFactory.CrearConexion();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
            INSERT INTO Cabanas 
                (Nombre, Descripcion, Capacidad, NumeroHabitaciones, 
                 PrecioPorNoche, Ubicacion, EstadoDisponibilidad, FechaRegistro, Activa)
            VALUES 
                ($nombre, $descripcion, $capacidad, $habitaciones,
                 $precio, $ubicacion, $estado, $fecha, $activa);
            SELECT last_insert_rowid();";

        comando.Parameters.AddWithValue("$nombre", cabana.Nombre);
        comando.Parameters.AddWithValue("$descripcion", cabana.Descripcion);
        comando.Parameters.AddWithValue("$capacidad", cabana.Capacidad);
        comando.Parameters.AddWithValue("$habitaciones", cabana.NumeroHabitaciones);
        comando.Parameters.AddWithValue("$precio", cabana.PrecioPorNoche);
        comando.Parameters.AddWithValue("$ubicacion", cabana.Ubicacion);
        comando.Parameters.AddWithValue("$estado", cabana.EstadoDisponibilidad);
        comando.Parameters.AddWithValue("$fecha", cabana.FechaRegistro.ToString("o"));
        comando.Parameters.AddWithValue("$activa", cabana.Activa ? 1 : 0);

        var idGenerado = Convert.ToInt32(comando.ExecuteScalar());
        cabana.Id = idGenerado;
        return idGenerado;
    }

    public IEnumerable<Cabana> ObtenerTodas()
    {
        var resultado = new List<Cabana>();

        using var conexion = _conexionFactory.CrearConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT * FROM Cabanas ORDER BY Nombre;";

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            resultado.Add(new Cabana
            {
                Id = lector.GetInt32(lector.GetOrdinal("Id")),
                Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
                Descripcion = lector.GetString(lector.GetOrdinal("Descripcion")),
                Capacidad = lector.GetInt32(lector.GetOrdinal("Capacidad")),
                NumeroHabitaciones = lector.GetInt32(lector.GetOrdinal("NumeroHabitaciones")),
                PrecioPorNoche = Convert.ToDecimal(lector.GetDouble(lector.GetOrdinal("PrecioPorNoche"))),
                Ubicacion = lector.GetString(lector.GetOrdinal("Ubicacion")),
                EstadoDisponibilidad = lector.GetString(lector.GetOrdinal("EstadoDisponibilidad")),
                FechaRegistro = DateTime.Parse(lector.GetString(lector.GetOrdinal("FechaRegistro"))),
                Activa = lector.GetInt32(lector.GetOrdinal("Activa")) == 1
            });
        }

        return resultado;
    }

    public int ContarRegistros()
    {
        using var conexion = _conexionFactory.CrearConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM Cabanas;";
        return Convert.ToInt32(comando.ExecuteScalar());
    }
}
