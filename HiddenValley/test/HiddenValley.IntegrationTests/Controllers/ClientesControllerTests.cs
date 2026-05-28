using System.Net;
using System.Net.Http.Json;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class ClientesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ClientesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ConFiltroBusqueda_DeberiaRetornarSoloCoincidencias()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);

            var p1 = new Persona { IdPersona = 10, Nombres = "Cristian", Apellidos = "Chamo", DPI = "3001000000101", Telefono = "12345678" };
            var p2 = new Persona { IdPersona = 11, Nombres = "Keily", Apellidos = "Lopez", DPI = "2002000000101", Telefono = "87654321" };
            db.Personas.AddRange(p1, p2);

            db.Clientes.AddRange(
                new Cliente { IdCliente = 1, IdPersona = 10 },
                new Cliente { IdCliente = 2, IdPersona = 11 }
            );
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/clientes?search=3001000000101&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jsonTexto = await response.Content.ReadAsStringAsync();
        
        Assert.Contains("Cristian Chamo", jsonTexto);
        Assert.DoesNotContain("Keily Gomez", jsonTexto);
    }

    [Fact]
    public async Task Buscar_PorFiltroTelefono_DeberiaRetornarClienteEspecifico()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);

            var persona = new Persona { IdPersona = 20, Nombres = "Juan", Apellidos = "Pérez", DPI = "1111222233333", Telefono = "55554444" };
            db.Personas.Add(persona);
            db.Clientes.Add(new Cliente { IdCliente = 5, IdPersona = 20 });
            await db.SaveChangesAsync();
        }
        var response = await _client.GetAsync("api/clientes/buscar?filtro=55554444");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var clienteDto = await response.Content.ReadFromJsonAsync<ClienteDetalleDTO>();
        Assert.NotNull(clienteDto);
        Assert.Equal("1111222233333", clienteDto.DPI);
    }

    [Fact]
    public async Task Buscar_CuandoFiltroNoExiste_DeberiaRetornarNotFound()
    {
        var response = await _client.GetAsync("api/clientes/buscar?filtro=Inexistente999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_CuandoPersonaNoExiste_DeberiaRetornarBadRequest()
    {
        var dto = new ClienteCreateDTO { IdPersona = 99999 };
        var response = await _client.PostAsJsonAsync("api/clientes", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("La persona no existe.", mensaje);
    }

    [Fact]
    public async Task Create_CuandoPersonaYaEsCliente_DeberiaRetornarBadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);

            db.Personas.Add(new Persona { IdPersona = 30, Nombres = "Luis", Apellidos = "Lopez", DPI = "777" });
            db.Clientes.Add(new Cliente { IdCliente = 10, IdPersona = 30 }); 
            await db.SaveChangesAsync();
        }

        var dto = new ClienteCreateDTO { IdPersona = 30 };

        var response = await _client.PostAsJsonAsync("api/clientes", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("Esta persona ya es cliente.", mensaje);
    }

    [Fact]
    public async Task Patch_CuandoClienteExiste_DeberiaModificarPropiedadesDePersona()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);

            var persona = new Persona { IdPersona = 40, Nombres = "Original", Apellidos = "Original", Gmail = "viejo@gmail.com" };
            db.Personas.Add(persona);
            db.Clientes.Add(new Cliente { IdCliente = 15, IdPersona = 40 });
            await db.SaveChangesAsync();
        }

        var dto = new ClientePatchDTO 
        { 
            Nombres = "NombreEditado", 
            Gmail = "nuevo@gmail.com" 
        };

        var response = await _client.PatchAsJsonAsync("api/clientes/15", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var personaEnDb = await db.Personas.FindAsync(40);
            Assert.NotNull(personaEnDb);
            Assert.Equal("NombreEditado", personaEnDb.Nombres);
            Assert.Equal("nuevo@gmail.com", personaEnDb.Gmail);
        }
    }

    [Fact]
    public async Task Delete_ClienteConHistorial_DeberiaRetornarBadRequest()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Clientes.RemoveRange(db.Clientes);

            db.Clientes.Add(new Cliente { IdCliente = 50, IdPersona = 1 });
            
            db.RegistroReservacion.Add(new RegistroReservacion
            {
                Id = 500,
                IdCliente = 50,
                FechaEntrada = DateTime.Now.AddDays(1),
                FechaSalida = DateTime.Now.AddDays(3),
                IdCabana = 1,
                EstadoReserva = "Confirmada"
            });
            await db.SaveChangesAsync();
        }
        var response = await _client.DeleteAsync("api/clientes/50");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("No se puede eliminar un cliente con historial.", mensaje);
    }


    [Fact]
    public async Task Create_CuandoPersonaExiste_DeberiaCrearClienteExitosamente()
    {
        int idPersonaCreada;
        
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);

            var nuevaPersona = new Persona 
            { 
                Nombres = "Christian", 
                Apellidos = "Chamo", 
                DPI = "2222333344445", 
                Telefono = "44332211",
                Gmail = "cchamo@hiddenvalley.com"
            };

            db.Personas.Add(nuevaPersona);
            await db.SaveChangesAsync();
            idPersonaCreada = nuevaPersona.IdPersona;
        }

        var dto = new ClienteCreateDTO 
        { 
            IdPersona = idPersonaCreada 
        };

        var response = await _client.PostAsJsonAsync("api/clientes", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var clienteEnDb = await db.Clientes
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(c => c.IdPersona == idPersonaCreada);

            Assert.NotNull(clienteEnDb); 
            Assert.NotNull(clienteEnDb.Persona); 
            Assert.Equal("Christian", clienteEnDb.Persona.Nombres); 
            Assert.Equal("2222333344445", clienteEnDb.Persona.DPI);
        }
    }
}