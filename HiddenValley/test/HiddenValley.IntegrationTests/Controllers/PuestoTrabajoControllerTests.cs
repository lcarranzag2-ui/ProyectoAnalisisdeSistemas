using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class PuestoTrabajoControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PuestoTrabajoControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_CuandoSearchEsNumerico_DeberiaFiltrarPorIdExacto()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);

            db.PuestosTrabajo.AddRange(
                new PuestoTrabajo { IdPuestoTrabajo = 1, Nombre = "Administrador", Descripcion = "Gerencia" },
                new PuestoTrabajo { IdPuestoTrabajo = 2, Nombre = "Recepcionista", Descripcion = "Atención" }
            );
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/puestotrabajo?search=2&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = await response.Content.ReadFromJsonAsync<PagedResponse<PuestoTrabajoResponseDto>>();
        
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.TotalRecords);
        Assert.Single(resultado.Items);
        Assert.Equal("Recepcionista", resultado.Items.First().Nombre);
    }

    [Fact]
    public async Task Create_CuandoNombreYaExiste_DeberiaRetornarBadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);
            db.PuestosTrabajo.Add(new PuestoTrabajo { Nombre = "Cocinero", Descripcion = "Área Restaurante" });
            await db.SaveChangesAsync();
        }

        var dtoDuplicado = new PuestoTrabajoCreateDto { Nombre = "cocinero", Descripcion = "Intento duplicado" };


        var response = await _client.PostAsJsonAsync("api/puestotrabajo", dtoDuplicado);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(json);
        Assert.Equal("Ya existe un puesto con ese nombre.", json["mensaje"]);
    }

    [Fact]
    public async Task Patch_CuandoNuevoNombreYaLoTieneOtroPuesto_DeberiaRetornarBadRequest()
    {
        int idPuestoAModificar;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);

            var p1 = new PuestoTrabajo { Nombre = "Salvavidas", Descripcion = "Piscinas" };
            var p2 = new PuestoTrabajo { Nombre = "Jardinero", Descripcion = "Áreas verdes" };
            
            db.PuestosTrabajo.AddRange(p1, p2);
            await db.SaveChangesAsync();

            idPuestoAModificar = p2.IdPuestoTrabajo; 
        }

        var dtoModificar = new PuestotrabajoPatchDto { Nombre = "salvavidas" };

        // Act
        var response = await _client.PatchAsJsonAsync($"api/puestotrabajo/{idPuestoAModificar}", dtoModificar);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(json);
        Assert.Equal("Ya existe otro puesto con ese nombre.", json["mensaje"]);
    }

    [Fact]
    public async Task Delete_CuandoPuestoTieneEmpleadosAsignados_DeberiaRetornarBadRequest()
    {
        int idPuestoBloqueado;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);
            db.Personas.RemoveRange(db.Personas);
            await db.SaveChangesAsync();

            var puesto = new PuestoTrabajo { Nombre = "Guía", Descripcion = "Tours" };
            db.PuestosTrabajo.Add(puesto);
            
            var persona = new Persona { Nombres = "Luis", Apellidos = "Mendoza", DPI = "7777", Telefono = "1122" };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            idPuestoBloqueado = puesto.IdPuestoTrabajo;

            db.Empleados.Add(new Empleado { IdPersona = persona.IdPersona, IdPuestoTrabajo = idPuestoBloqueado });
            await db.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/puestotrabajo/{idPuestoBloqueado}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(json);
        Assert.Equal("No se puede eliminar: hay empleados asignados a este puesto.", json["mensaje"]);
    }
}