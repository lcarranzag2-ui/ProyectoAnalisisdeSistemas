using System.Net;
using System.Net.Http.Json;
using HiddenValley.API.Models;
using HiddenValley.API.Data;
using HiddenValley.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class CabanasControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CabanasControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task GetAll_DeberiaRetornarOkYListaPaginada()
    {
        var response = await _client.GetAsync("api/cabanas?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<object>>();
        Assert.NotNull(result);
    }

    // ENDPOINT: GET api/cabanas/disponibilidad

    [Fact]
    public async Task VerificarDisponibilidad_FechasValidas_DeberiaRetornarOk()
    {
        var inicio = DateTime.Now.ToString("yyyy-MM-dd");
        var fin = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");

        var response = await _client.GetAsync($"api/cabanas/disponibilidad?fechaInicio={inicio}&fechaFin={fin}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task VerificarDisponibilidad_FechasInvertidas_DeberiaRetornarBadRequest()
    {
        var inicio = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");
        var fin = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"api/cabanas/disponibilidad?fechaInicio={inicio}&fechaFin={fin}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ENDPOINT: DELETE api/cabanas/{id}

    [Fact]
    public async Task EliminarCabana_CuandoNoExiste_DeberiaRetornarBadRequest()
    {
        var response = await _client.DeleteAsync("api/cabanas/999");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VerificarDisponibilidad_CuandoEstaOcupada_DeberiaReflejarNoDisponible()
    {
        var fechaInicio = DateTime.Now.AddDays(10);
        var fechaFin = DateTime.Now.AddDays(15);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Cabanas.RemoveRange(db.Cabanas);

            var cabana = new Cabana { IdCabana = 50, IdTipoCabana = 1, IdEstadoCabana = 1 };
            db.Cabanas.Add(cabana);
            var reservaExistente = new RegistroReservacion
            {
                Id = 100,
                IdCabana = 50,
                FechaEntrada = DateTime.Now.AddDays(11),
                FechaSalida = DateTime.Now.AddDays(14),
                IdCliente = 1,
                CantidadPersonas = 2,
                EstadoReserva = "Confirmada",
                TotalPagar = 500
            };
            db.RegistroReservacion.Add(reservaExistente);
            await db.SaveChangesAsync();
        }

        var inicioStr = fechaInicio.ToString("yyyy-MM-dd");
        var finStr = fechaFin.ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"api/cabanas/disponibilidad?fechaInicio={inicioStr}&fechaFin={finStr}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var cabanasDisponibles = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(cabanasDisponibles);

        var jsonTexto = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("\"idCabana\":50", jsonTexto);
    }
}