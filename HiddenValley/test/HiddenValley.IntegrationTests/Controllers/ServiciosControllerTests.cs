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

public class ServiciosControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ServiciosControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_DeberiaPaginatYRetornarCabecerasCorrectas()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.ReservacionServicios.RemoveRange(db.ReservacionServicios); 
            db.Servicio.RemoveRange(db.Servicio);
            await db.SaveChangesAsync();

            db.Servicio.AddRange(
                new Servicio { Nombre = "Piscina", Descripcion = "Acceso por el dia", Precio = 50 },
                new Servicio { Nombre = "Tour Guiado", Descripcion = "Recorrido por el valle", Precio = 150 },
                new Servicio { Nombre = "Alquiler Toalla", Descripcion = "Toalla de cuerpo", Precio = 15 }
            );
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/servicios?pageNumber=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<IEnumerable<ServicioReadDto>>();
        Assert.NotNull(items);
        Assert.Equal(2, items.Count());

        Assert.True(response.Headers.Contains("X-Total-Records"));
        Assert.True(response.Headers.Contains("X-Total-Pages"));

        var totalRecords = response.Headers.GetValues("X-Total-Records").First();
        var totalPages = response.Headers.GetValues("X-Total-Pages").First();

        Assert.Equal("3", totalRecords); 
        Assert.Equal("2", totalPages);   
    }

    [Fact]
    public async Task Create_ConDatosValidos_DeberiaRegistrarServicioYRetornarOk()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Servicio.RemoveRange(db.Servicio);
            await db.SaveChangesAsync();
        }

        var nuevoDto = new ServicioCreateDto
        {
            Nombre = "Masaje Relajante",
            Descripcion = "Sesión de 45 minutos en spa",
            Precio = 200
        };
        var response = await _client.PostAsJsonAsync("api/servicios", nuevoDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cuerpo = await response.Content.ReadFromJsonAsync<ServicioCreateDto>();
        Assert.NotNull(cuerpo);
        Assert.Equal(nuevoDto.Nombre, cuerpo.Nombre);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var guardado = await db.Servicio.FirstOrDefaultAsync(s => s.Nombre == "Masaje Relajante");
            Assert.NotNull(guardado);
            Assert.Equal(200, guardado.Precio);
        }
    }

    [Fact]
    public async Task Patch_CuandoSoloSeEnviaPrecio_DeberiaModificarSoloElPrecio()
    {
        int idServicio;
        string descripcionOriginal = "Uso de canchas de tenis por hora";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Servicio.RemoveRange(db.Servicio);
            await db.SaveChangesAsync();

            var servicio = new Servicio 
            { 
                Nombre = "Canchas de Tenis", 
                Descripcion = descripcionOriginal, 
                Precio = 75 
            };
            db.Servicio.Add(servicio);
            await db.SaveChangesAsync();
            idServicio = servicio.IdServicio;
        }

        var patchDto = new UpdateServicioDto
        {
            Nombre = null,
            Descripcion = null,
            Precio = 95 
        };

        var response = await _client.PatchAsJsonAsync($"api/servicios/{idServicio}", patchDto);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var modificado = await db.Servicio.FindAsync(idServicio);
            
            Assert.NotNull(modificado);
            Assert.Equal(95, modificado.Precio); 
            Assert.Equal("Canchas de Tenis", modificado.Nombre); 
            Assert.Equal(descripcionOriginal, modificado.Descripcion); 
        }
    }

    [Fact]
    public async Task Delete_CuandoIdNoExiste_DeberiaRetornarNotFound()
    {
        int idInexistente = 99999; 

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var encontrado = await db.Servicio.FindAsync(idInexistente);
            if (encontrado != null)
            {
                db.Servicio.Remove(encontrado);
                await db.SaveChangesAsync();
            }
        }
        var response = await _client.DeleteAsync($"api/servicios/{idInexistente}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}