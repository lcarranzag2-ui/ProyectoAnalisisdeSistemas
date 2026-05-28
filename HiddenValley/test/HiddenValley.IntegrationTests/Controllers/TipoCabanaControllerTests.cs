using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class TipoCabanaControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public TipoCabanaControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_DeberiaRetornarTodosLosTiposDeCabana()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Cabanas.RemoveRange(db.Cabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 
            await db.SaveChangesAsync();

            db.TiposCabana.AddRange(
                new TipoCabana { Nombre = "Familiar", Precio = 500, Capacidad = 6 }, 
                new TipoCabana { Nombre = "Matrimonial", Precio = 350, Capacidad = 2 } 
            );
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/tipocabana");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<IEnumerable<TipoCabanaDTO>>();
        Assert.NotNull(items);
        Assert.Equal(2, items.Count());
    }

    [Fact]
    public async Task Create_ConDatosValidos_DeberiaRegistrarYRetornarCreatedAtAction()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Cabanas.RemoveRange(db.Cabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 
            await db.SaveChangesAsync();
        }

        var dto = new TipoCabanaCreateDTO { Nombre = "Suites", Precio = 600 };

        var response = await _client.PostAsJsonAsync("api/tipocabana", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var cuerpo = await response.Content.ReadFromJsonAsync<TipoCabanaDTO>();
        Assert.NotNull(cuerpo);
        Assert.True(cuerpo.IdTipoCabana > 0);
        Assert.Equal("Suites", cuerpo.Nombre);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Patch_CuandoIdExiste_DeberiaActualizarYRetornarOk()
    {
        int idTipo;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Cabanas.RemoveRange(db.Cabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 
            await db.SaveChangesAsync();

            var tipo = new TipoCabana { Nombre = "Original", Precio = 200, Capacidad = 2 }; 
            db.TiposCabana.Add(tipo); 
            await db.SaveChangesAsync();
            idTipo = tipo.IdTipoCabana;
        }

        var dtoPatch = new TipoCabanaCreateDTO { Nombre = "Modificado", Precio = 275 };

        var response = await _client.PatchAsJsonAsync($"api/tipocabana/{idTipo}", dtoPatch);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var modificado = await db.TiposCabana.FindAsync(idTipo); 
            Assert.NotNull(modificado);
            Assert.Equal("Modificado", modificado.Nombre);
            Assert.Equal(275, modificado.Precio);
        }
    }

    [Fact]
    public async Task Delete_CuandoTieneCabanasAsociadas_DeberiaRetornarBadRequest()
    {
        int idTipo;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Cabanas.RemoveRange(db.Cabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 
            await db.SaveChangesAsync();

            var tipo = new TipoCabana { Nombre = "No Borrable", Precio = 300, Capacidad = 4 }; 
            db.TiposCabana.Add(tipo); 
            await db.SaveChangesAsync();
            idTipo = tipo.IdTipoCabana;

            var cabana = new Cabana { IdTipoCabana = idTipo, IdEstadoCabana = 1 }; 
            db.Cabanas.Add(cabana);
            await db.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/tipocabana/{idTipo}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("No se puede eliminar: tiene cabañas asociadas.", mensaje);
    }

    [Fact]
    public async Task Delete_CuandoNoTieneCabanasAsociadas_DeberiaRetornarNoContent()
    {
        int idTipo;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Cabanas.RemoveRange(db.Cabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 
            await db.SaveChangesAsync();

            var tipo = new TipoCabana { Nombre = "Borrable", Precio = 150, Capacidad = 2 }; 
            db.TiposCabana.Add(tipo); 
            await db.SaveChangesAsync();
            idTipo = tipo.IdTipoCabana;
        }

        var response = await _client.DeleteAsync($"api/tipocabana/{idTipo}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existe = await db.TiposCabana.AnyAsync(t => t.IdTipoCabana == idTipo); 
            Assert.False(existe);
        }
    }
}