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

    // =========================================================================
    // ENDPOINT: GET api/cabanas (GetAll)
    // =========================================================================

    [Fact]
    public async Task GetAll_DeberiaRetornarOkYListaPaginada()
    {
        // Act
        var response = await _client.GetAsync("api/cabanas?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<object>>();
        Assert.NotNull(result);
    }

    // =========================================================================
    // ENDPOINT: GET api/cabanas/disponibilidad
    // =========================================================================

    [Fact]
    public async Task VerificarDisponibilidad_FechasValidas_DeberiaRetornarOk()
    {
        // Arrange
        var inicio = DateTime.Now.ToString("yyyy-MM-dd");
        var fin = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");

        // Act
        var response = await _client.GetAsync($"api/cabanas/disponibilidad?fechaInicio={inicio}&fechaFin={fin}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task VerificarDisponibilidad_FechasInvertidas_DeberiaRetornarBadRequest()
    {
        // Arrange
        var inicio = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");
        var fin = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");

        // Act
        var response = await _client.GetAsync($"api/cabanas/disponibilidad?fechaInicio={inicio}&fechaFin={fin}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // =========================================================================
    // ENDPOINT: POST api/cabanas/registrar
    // =========================================================================

    [Fact]
    public async Task RegistrarCabana_CuandoTipoNoExiste_DeberiaRetornarNotFound()
    {
        // Arrange: Enviamos un ID que sabemos que no existe en la DB vacía
        var request = new RegistrarCabanaRequest { IdTipoCabana = 999, IdEstadoCabana = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("api/cabanas/registrar", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // =========================================================================
    // ENDPOINT: DELETE api/cabanas/{id}
    // =========================================================================

    [Fact]
    public async Task EliminarCabana_CuandoNoExiste_DeberiaRetornarBadRequest()
    {
        // Act: Intentar borrar la cabaña con ID 999
        var response = await _client.DeleteAsync("api/cabanas/999");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}