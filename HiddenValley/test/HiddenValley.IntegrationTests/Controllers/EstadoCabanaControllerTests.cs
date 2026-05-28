using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class EstadoCabanaControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public EstadoCabanaControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_DeberiaRetornarTodosLosEstadosDeLaBaseDeDatos()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);

            db.EstadosCabanas.AddRange(
                new EstadoCabana { IdEstadoCabana = 1, Nombre = "Disponible", Descripcion = "Lista para usarse" },
                new EstadoCabana { IdEstadoCabana = 2, Nombre = "Mantenimiento", Descripcion = "En limpieza profunda" }
            );
            await db.SaveChangesAsync();
        }
        var response = await _client.GetAsync("api/estadocabana");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var estados = await response.Content.ReadFromJsonAsync<IEnumerable<EstadoCabanaDto>>();
        
        Assert.NotNull(estados);
        Assert.Equal(2, estados.Count());
        Assert.Contains(estados, e => e.Nombre == "Disponible");
        Assert.Contains(estados, e => e.Nombre == "Mantenimiento");
    }


    [Fact]
    public async Task GetById_CuandoExiste_DeberiaRetornarEstadoEspecifico()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);

            db.EstadosCabanas.Add(new EstadoCabana { IdEstadoCabana = 5, Nombre = "Ocupada", Descripcion = "Huéspedes activos" });
            await db.SaveChangesAsync();
        }
        var response = await _client.GetAsync("api/estadocabana/5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var estado = await response.Content.ReadFromJsonAsync<EstadoCabanaDto>();
        Assert.NotNull(estado);
        Assert.Equal("Ocupada", estado.Nombre);
    }

    [Fact]
    public async Task GetById_CuandoNoExiste_DeberiaRetornarNotFound()
    {
        var response = await _client.GetAsync("api/estadocabana/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_CuandoDatosSonValidos_DeberiaGuardarYRetornarCreatedWithLocation()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);
            await db.SaveChangesAsync();
        }

        var dto = new EstadoCabanaDto { Nombre = "Reservada", Descripcion = "Apartada por cliente" };
        var response = await _client.PostAsJsonAsync("api/estadocabana", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode); 

        var locationHeader = response.Headers.Location;
        Assert.NotNull(locationHeader);
        Assert.Contains("api/EstadoCabana", locationHeader.ToString());
    }

    [Fact]
    public async Task Create_CuandoNombreYaExiste_DeberiaRetornarBadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);
            db.EstadosCabanas.Add(new EstadoCabana { IdEstadoCabana = 10, Nombre = "Sucia", Descripcion = "Falta personal" });
            await db.SaveChangesAsync();
        }

        var dtoDuplicado = new EstadoCabanaDto { Nombre = "sucia", Descripcion = "Otro clon" };

        var response = await _client.PostAsJsonAsync("api/estadocabana", dtoDuplicado);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("Ya existe un estado con este nombre.", mensaje);
    }

    [Fact]
    public async Task Patch_CuandoEstadoExiste_DeberiaModificarCampos()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);
            db.EstadosCabanas.Add(new EstadoCabana { IdEstadoCabana = 8, Nombre = "Inactiva", Descripcion = "Por reparaciones" });
            await db.SaveChangesAsync();
        }

        var dtoModificado = new EstadoCabanaDto { Descripcion = "Remodelación total de madera" };
        var response = await _client.PatchAsJsonAsync("api/estadocabana/8", dtoModificado);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var estadoDb = await db.EstadosCabanas.FindAsync(8);
            Assert.NotNull(estadoDb);
            Assert.Equal("Inactiva", estadoDb.Nombre); 
            Assert.Equal("Remodelación total de madera", estadoDb.Descripcion); 
        }
    }

    [Fact]
    public async Task Delete_EstadoExistente_DeberiaRemoverloFisicamente()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);
            db.EstadosCabanas.Add(new EstadoCabana { IdEstadoCabana = 15, Nombre = "Temporal", Descripcion = "Borrar" });
            await db.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync("api/estadocabana/15");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existe = await db.EstadosCabanas.AnyAsync(e => e.IdEstadoCabana == 15);
            Assert.False(existe);
        }
    }
}