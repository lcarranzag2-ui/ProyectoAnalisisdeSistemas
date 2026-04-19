using HiddenValley.Data.Context;

namespace HiddenValley.Services;

public interface IServicioArranque
{
    ResultadoArranque IniciarSistema();
}

public class ServicioArranque : IServicioArranque
{
    private readonly IInicializadorBaseDatos _inicializador;
    private readonly IConexionFactory _conexionFactory;

    public ServicioArranque(
        IInicializadorBaseDatos inicializador,
        IConexionFactory conexionFactory)
    {
        _inicializador = inicializador;
        _conexionFactory = conexionFactory;
    }

    public ResultadoArranque IniciarSistema()
    {
        var baseDatosYaExistia = _inicializador.BaseDatosExiste();

        _inicializador.Inicializar();

        return new ResultadoArranque
        {
            RutaBaseDatos = _conexionFactory.RutaBaseDatos,
            BaseDatosExistia = baseDatosYaExistia,
            InicializacionExitosa = true
        };
    }
}

public class ResultadoArranque
{
    public string RutaBaseDatos { get; set; } = string.Empty;
    public bool BaseDatosExistia { get; set; }
    public bool InicializacionExitosa { get; set; }
}
