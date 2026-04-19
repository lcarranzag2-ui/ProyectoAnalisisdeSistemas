using Microsoft.Data.Sqlite;

namespace HiddenValley.Data.Context;

public interface IInicializadorBaseDatos
{
    void Inicializar();
    bool BaseDatosExiste();
}

public class InicializadorBaseDatos : IInicializadorBaseDatos
{
    private readonly IConexionFactory _conexionFactory;
    private const string RutaScriptSchema = "Scripts/SchemaInicial.sql";

    public InicializadorBaseDatos(IConexionFactory conexionFactory)
    {
        _conexionFactory = conexionFactory;
    }

    public bool BaseDatosExiste() => File.Exists(_conexionFactory.RutaBaseDatos);

    public void Inicializar()
    {
        using var conexion = _conexionFactory.CrearConexion();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            var script = LeerScriptSchema();
            EjecutarScript(conexion, transaccion, script);
            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static string LeerScriptSchema()
    {
        var rutaCompleta = Path.Combine(AppContext.BaseDirectory, RutaScriptSchema);

        if (!File.Exists(rutaCompleta))
        {
            throw new FileNotFoundException(
                $"No se encontró el script de inicialización en: {rutaCompleta}");
        }

        return File.ReadAllText(rutaCompleta);
    }

    private static void EjecutarScript(
        SqliteConnection conexion,
        SqliteTransaction transaccion,
        string script)
    {
        using var comando = conexion.CreateCommand();
        comando.Transaction = transaccion;
        comando.CommandText = script;
        comando.ExecuteNonQuery();
    }
}
