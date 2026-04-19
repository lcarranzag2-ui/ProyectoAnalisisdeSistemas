using Microsoft.Data.Sqlite;

namespace HiddenValley.Data.Context;

public interface IConexionFactory
{
    SqliteConnection CrearConexion();
    string RutaBaseDatos { get; }
}

public class SqliteConexionFactory : IConexionFactory
{
    private readonly ConfiguracionBaseDatos _configuracion;

    public SqliteConexionFactory(ConfiguracionBaseDatos configuracion)
    {
        _configuracion = configuracion;
        RutaBaseDatos = configuracion.ObtenerRutaCompleta();
    }

    public string RutaBaseDatos { get; }

    public SqliteConnection CrearConexion()
    {
        var conexion = new SqliteConnection(_configuracion.ObtenerCadenaConexion());
        conexion.Open();

        using var pragma = conexion.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON; PRAGMA journal_mode = WAL;";
        pragma.ExecuteNonQuery();

        return conexion;
    }
}
