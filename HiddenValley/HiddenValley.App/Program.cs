using HiddenValley.Data.Context;
using HiddenValley.Data.Repositories;
using HiddenValley.Domain.Entities;
using HiddenValley.Services;

namespace HiddenValley.App;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("===========================================");
        Console.WriteLine("   SISTEMA HIDDEN VALLEY - INICIALIZACION  ");
        Console.WriteLine("===========================================");
        Console.WriteLine();

        var configuracion = new ConfiguracionBaseDatos();
        var conexionFactory = new SqliteConexionFactory(configuracion);
        var inicializador = new InicializadorBaseDatos(conexionFactory);
        var servicioArranque = new ServicioArranque(inicializador, conexionFactory);

        var resultado = servicioArranque.IniciarSistema();

        Console.WriteLine($"Ruta de base de datos : {resultado.RutaBaseDatos}");
        Console.WriteLine($"Ya existia            : {resultado.BaseDatosExistia}");
        Console.WriteLine($"Inicializacion OK     : {resultado.InicializacionExitosa}");
        Console.WriteLine();

        var repositorioCabanas = new CabanaRepositorio(conexionFactory);
        var cantidadPrevia = repositorioCabanas.ContarRegistros();
        Console.WriteLine($"Cabanas registradas previamente: {cantidadPrevia}");

        if (cantidadPrevia == 0)
        {
            Console.WriteLine("Insertando cabana de prueba para verificar persistencia...");

            repositorioCabanas.Agregar(new Cabana
            {
                Nombre = "Cabana del Bosque",
                Descripcion = "Cabana acogedora rodeada de pinos",
                Capacidad = 4,
                NumeroHabitaciones = 2,
                PrecioPorNoche = 450.00m,
                Ubicacion = "Sector Norte",
                EstadoDisponibilidad = "Disponible",
                FechaRegistro = DateTime.Now,
                Activa = true
            });
        }

        Console.WriteLine();
        Console.WriteLine("Listado actual de cabanas:");
        Console.WriteLine("----------------------------------------");

        foreach (var cabana in repositorioCabanas.ObtenerTodas())
        {
            Console.WriteLine($"  [{cabana.Id}] {cabana.Nombre} - Q{cabana.PrecioPorNoche:N2} / noche");
        }

        Console.WriteLine();
        Console.WriteLine("Reinicie el programa para comprobar que los datos persisten.");
    }
}
