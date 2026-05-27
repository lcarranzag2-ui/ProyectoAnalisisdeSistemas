using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class ReservacionesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ReservacionesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_CuandoExisteTraslapeDeFechas_DeberiaRetornarBadRequest()
    {
        int idClienteParaTest;
        int idCabanaValida;
        string telefonoClienteA = "10101010";
        string telefonoClienteB = "55554444"; 

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Cabanas.RemoveRange(db.Cabanas);
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 

            await db.SaveChangesAsync();
            var tipo = new TipoCabana { Nombre = "Familiar VIP", Capacidad = 6, Precio = 500 };
            db.TiposCabana.Add(tipo);
            await db.SaveChangesAsync();

            var cabana = new Cabana { IdTipoCabana = tipo.IdTipoCabana, IdEstadoCabana = 1 };
            db.Cabanas.Add(cabana);
            await db.SaveChangesAsync();
            idCabanaValida = cabana.IdCabana;

            var personaA = new Persona { Nombres = "Kevin", Apellidos = "Chamo", DPI = "1010", Telefono = telefonoClienteA };
            db.Personas.Add(personaA);
            await db.SaveChangesAsync();
            var clienteA = new Cliente { IdPersona = personaA.IdPersona };
            db.Clientes.Add(clienteA);
            await db.SaveChangesAsync();

            var personaB = new Persona { Nombres = "Estudiante", Apellidos = "UMG", DPI = "9999", Telefono = telefonoClienteB };
            db.Personas.Add(personaB);
            await db.SaveChangesAsync();
            var clienteB = new Cliente { IdPersona = personaB.IdPersona };
            db.Clientes.Add(clienteB);
            await db.SaveChangesAsync();
            idClienteParaTest = clienteB.IdCliente;

            db.RegistroReservacion.Add(new RegistroReservacion
            {
                IdCliente = clienteA.IdCliente,
                IdCabana = idCabanaValida,
                FechaEntrada = new DateTime(2026, 06, 10),
                FechaSalida = new DateTime(2026, 06, 15),
                CantidadPersonas = 2,
                EstadoReserva = "Recibida",
                TotalPagar = 2500,
                IdEmpleado = 1
            });
            await db.SaveChangesAsync();
        }

        var dtoConTraslape = new ReservacionCreateDto
        {
            IdCliente = idClienteParaTest,
            IdCabana = idCabanaValida,
            Telefono = telefonoClienteB, 
            FechaEntrada = new DateTime(2026, 06, 12),
            FechaSalida = new DateTime(2026, 06, 18),
            CantidadPersonas = 3
        };

        var response = await _client.PostAsJsonAsync("api/reservaciones", dtoConTraslape);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(json);
        Assert.Equal("La cabaña ya tiene una reserva en esas fechas.", json["mensaje"]);
    }

    [Fact]
    public async Task Create_CuandoFlujoEsCorrecto_DeberiaCalcularTotalBasadoEnPrecioYNoches()
    {
        int idClienteValido;
        int idCabanaValida;
        string telefonoValido = "44444444";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Cabanas.RemoveRange(db.Cabanas);
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);
            db.TiposCabana.RemoveRange(db.TiposCabana);
            await db.SaveChangesAsync();

            var persona = new Persona { Nombres = "Maria", Apellidos = "Chamo", DPI = "4444", Telefono = telefonoValido };
            db.Personas.Add(persona);
            var tipo = new TipoCabana { Nombre = "Estándar", Capacidad = 4, Precio = 300 };
            db.TiposCabana.Add(tipo);
            await db.SaveChangesAsync();

            var cliente = new Cliente { IdPersona = persona.IdPersona };
            db.Clientes.Add(cliente);

            var cabana = new Cabana 
            { 
                IdTipoCabana = tipo.IdTipoCabana,
                IdEstadoCabana = 1
            };
            db.Cabanas.Add(cabana);
            await db.SaveChangesAsync();

            idClienteValido = cliente.IdCliente;
            idCabanaValida = cabana.IdCabana;
        }

        var dtoExitoso = new ReservacionCreateDto
        {
            IdCliente = idClienteValido,
            IdCabana = idCabanaValida,
            Telefono = telefonoValido,
            FechaEntrada = new DateTime(2026, 08, 20),
            FechaSalida = new DateTime(2026, 08, 23),
            CantidadPersonas = 2
        };

        var response = await _client.PostAsJsonAsync("api/reservaciones", dtoExitoso);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var cuerpo = await response.Content.ReadFromJsonAsync<ResponseCreacionAnonima>();
        Assert.NotNull(cuerpo);
        Assert.Equal(3, cuerpo.noches);
        Assert.Equal(900, cuerpo.totalPagar); 
    }

    [Fact]
    public async Task Create_CuandoExcedeCapacidadDeLaCabana_DeberiaRetornarBadRequest()
    {
        int idClienteValido;
        int idCabanaValida;
        string telefonoValido = "33333333";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var persona = new Persona { Nombres = "Luis", Apellidos = "Chamo", DPI = "5555", Telefono = telefonoValido };
            db.Personas.Add(persona);
            var tipo = new TipoCabana { Nombre = "Matrimonial", Capacidad = 2, Precio = 250 };
            db.TiposCabana.Add(tipo);
            await db.SaveChangesAsync();

            var cliente = new Cliente { IdPersona = persona.IdPersona };
            db.Clientes.Add(cliente);

            var cabana = new Cabana { IdTipoCabana = tipo.IdTipoCabana, IdEstadoCabana = 1 };
            db.Cabanas.Add(cabana);
            await db.SaveChangesAsync();

            idClienteValido = cliente.IdCliente;
            idCabanaValida = cabana.IdCabana;
        }

        var dtoExcedido = new ReservacionCreateDto
        {
            IdCliente = idClienteValido,
            IdCabana = idCabanaValida,
            Telefono = telefonoValido,
            FechaEntrada = new DateTime(2026, 09, 01),
            FechaSalida = new DateTime(2026, 09, 05),
            CantidadPersonas = 5 
        };

        var response = await _client.PostAsJsonAsync("api/reservaciones", dtoExcedido);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private class ResponseCreacionAnonima
    {
        public int id { get; set; }
        public string mensaje { get; set; } = string.Empty;
        public decimal totalPagar { get; set; }
        public int noches { get; set; }
    }
}