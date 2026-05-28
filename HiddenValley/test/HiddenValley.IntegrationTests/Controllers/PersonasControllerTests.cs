using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class PersonasControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PersonasControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ConFiltroDpi_DeberiaRetornarSoloLaPersonaQueCoincida()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Empleados.RemoveRange(db.Empleados);
            db.Personas.RemoveRange(db.Personas);

            db.Personas.AddRange(
                new Persona { Nombres = "Cristian", Apellidos = "Chamo", DPI = "1234567890101", Telefono = "5555" },
                new Persona { Nombres = "Keily", Apellidos = "Gomez", DPI = "9876543210101", Telefono = "4444" }
            );
            await db.SaveChangesAsync();
        }
        var response = await _client.GetAsync("api/personas?search=123456789&page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = await response.Content.ReadFromJsonAsync<PagedResponse<PersonaResponseDto>>();
        
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.TotalRecords);
        Assert.Contains(resultado.Items, p => p.Nombres == "Cristian");
        Assert.DoesNotContain(resultado.Items, p => p.Nombres == "Keily");
    }

    [Fact]
    public async Task VerificarDpi_CuandoDpiYaExiste_DeberiaRetornarTrue()
    {
        var dpiExistente = "5555444433321";
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Personas.RemoveRange(db.Personas);
            db.Personas.Add(new Persona { Nombres = "Test", Apellidos = "DPI", DPI = dpiExistente, Telefono = "0000" });
            await db.SaveChangesAsync();
        }
        var response = await _client.GetAsync($"api/personas/dpi/{dpiExistente}/existe");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"existe\":true", json);
    }

    [Fact]
    public async Task Create_CuandoDpiYaExiste_DeberiaRetornarBadRequest()
    {
        var dpiRepetido = "1111222233334";
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Personas.RemoveRange(db.Personas);
            db.Personas.Add(new Persona { Nombres = "Original", Apellidos = "User", DPI = dpiRepetido, Telefono = "11" });
            await db.SaveChangesAsync();
        }

        var dtoDuplicado = new PersonaCreateDto 
        { 
            Nombres = "Clon", 
            Apellidos = "Persona", 
            DPI = dpiRepetido, 
            Telefono = "22" 
        };

        var response = await _client.PostAsJsonAsync("api/personas", dtoDuplicado);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Ya existe una persona con ese DPI.", json);
    }

    [Fact]
    public async Task Delete_CuandoPersonaEstaVinculadaAEmpleado_DeberiaRetornarBadRequest()
    {
        int idPersonaBloqueada;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Empleados.RemoveRange(db.Empleados);
            db.Personas.RemoveRange(db.Personas);

            var persona = new Persona { Nombres = "Trabajador", Apellidos = "Hidden", DPI = "9999", Telefono = "123" };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            idPersonaBloqueada = persona.IdPersona;
            db.Empleados.Add(new Empleado { IdPersona = idPersonaBloqueada, IdPuestoTrabajo = 1 });
            await db.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/personas/{idPersonaBloqueada}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Persona vinculada a otros registros.", json);
    }

    [Fact]
    public async Task Delete_CuandoPersonaNoTieneVinculos_DeberiaEliminarExitosamente()
    {
        int idPersonaLibre;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Empleados.RemoveRange(db.Empleados);
            db.Personas.RemoveRange(db.Personas);

            var persona = new Persona { Nombres = "ExHuesped", Apellidos = "Temporal", DPI = "44445555", Telefono = "8888" };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            idPersonaLibre = persona.IdPersona;
        }
        var response = await _client.DeleteAsync($"api/personas/{idPersonaLibre}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existe = await db.Personas.AnyAsync(p => p.IdPersona == idPersonaLibre);
            Assert.False(existe);
        }
    }


    [Fact]
    public async Task Create_CuandoPersonaYaEsEmpleado_DeberiaPermitirCrearClienteExitosamente()
    {
        int idPersonaMixta;


        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Empleados.RemoveRange(db.Empleados);
            db.Personas.RemoveRange(db.Personas);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);
            await db.SaveChangesAsync();

            var puesto = new PuestoTrabajo { IdPuestoTrabajo = 1, Nombre = "Mantenimiento" };
            db.PuestosTrabajo.Add(puesto);
            var persona = new Persona 
            { 
                Nombres = "Juan Carlos", 
                Apellidos = "Chamo", 
                DPI = "5050444433221", 
                Telefono = "55443322" 
            };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            idPersonaMixta = persona.IdPersona;

            db.Empleados.Add(new Empleado 
            { 
                IdPersona = idPersonaMixta, 
                IdPuestoTrabajo = 1 
            });
            await db.SaveChangesAsync();
        }

        var dtoCliente = new ClienteCreateDTO 
        { 
            IdPersona = idPersonaMixta 
        };

        var response = await _client.PostAsJsonAsync("api/clientes", dtoCliente);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var esEmpleado = await db.Empleados.AnyAsync(e => e.IdPersona == idPersonaMixta);
            var esCliente = await db.Clientes.AnyAsync(c => c.IdPersona == idPersonaMixta);

            Assert.True(esEmpleado, "Debería seguir siendo un empleado registrado.");
            Assert.True(esCliente, "Ahora también debería existir en la tabla de clientes.");
        }
    }

    [Fact]
    public async Task Delete_CuandoSeEliminaUnCliente_DeberiaMantenerALaPersonaEnLaBaseDeDatos()
    {
        int idPersonaBase;
        int idClienteAEliminar;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);
            await db.SaveChangesAsync();

            var persona = new Persona 
            { 
                Nombres = "Estuardo", 
                Apellidos = "Anzueto", 
                DPI = "4040555566661", 
                Telefono = "24241111" 
            };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            idPersonaBase = persona.IdPersona;

            var cliente = new Cliente { IdPersona = idPersonaBase };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            idClienteAEliminar = cliente.IdCliente;
        }
        var response = await _client.DeleteAsync($"api/clientes/{idClienteAEliminar}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var clienteExiste = await db.Clientes.AnyAsync(c => c.IdCliente == idClienteAEliminar);
            var personaExiste = await db.Personas.AnyAsync(p => p.IdPersona == idPersonaBase);

            Assert.False(clienteExiste, "El registro del cliente debió ser removido.");
            Assert.True(personaExiste, "La persona base DEBE permanecer en la DB por integridad histórica.");
        }
    }
}