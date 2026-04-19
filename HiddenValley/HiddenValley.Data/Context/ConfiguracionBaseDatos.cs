namespace HiddenValley.Data.Context;

public class ConfiguracionBaseDatos
{
    public string NombreArchivo { get; init; } = "HiddenValley.db";
    public string CarpetaAplicacion { get; init; } = "HiddenValley";

    public string ObtenerRutaCompleta()
    {
        var carpetaBase = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        var carpetaDestino = Path.Combine(carpetaBase, CarpetaAplicacion);

        if (!Directory.Exists(carpetaDestino))
        {
            Directory.CreateDirectory(carpetaDestino);
        }

        return Path.Combine(carpetaDestino, NombreArchivo);
    }

    public string ObtenerCadenaConexion()
    {
        return $"Data Source={ObtenerRutaCompleta()};Foreign Keys=True;";
    }
}
