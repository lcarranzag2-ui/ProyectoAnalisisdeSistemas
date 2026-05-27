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

public class DashboardControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public DashboardControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetResumen_DeberiaCalcularMetricasYRetornarOk()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.ReservacionServicios.RemoveRange(db.ReservacionServicios);
            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Cabanas.RemoveRange(db.Cabanas);
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana);
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);
            await db.SaveChangesAsync();

            var persona = new Persona { Nombres = "Juan", Apellidos = "Perez", DPI = "123", Telefono = "123" };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            var cliente = new Cliente { IdPersona = persona.IdPersona };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var estadoDisp = new EstadoCabana { IdEstadoCabana = 1, Nombre = "Disponible" };
            db.EstadosCabanas.Add(estadoDisp);
            await db.SaveChangesAsync();

            var tipo = new TipoCabana { Nombre = "Individual", Precio = 200, Capacidad = 2 };
            db.TiposCabana.Add(tipo);
            await db.SaveChangesAsync();

            var cabana1 = new Cabana { IdTipoCabana = tipo.IdTipoCabana, IdEstadoCabana = estadoDisp.IdEstadoCabana };
            var cabana2 = new Cabana { IdTipoCabana = tipo.IdTipoCabana, IdEstadoCabana = estadoDisp.IdEstadoCabana };
            db.Cabanas.AddRange(cabana1, cabana2);
            await db.SaveChangesAsync();

            var reservaOcupada = new RegistroReservacion
            {
                IdCliente = cliente.IdCliente,
                IdCabana = cabana1.IdCabana,
                FechaEntrada = DateTime.Today,
                FechaSalida = DateTime.Today.AddDays(2),
                CantidadPersonas = 3,
                EstadoReserva = "Activa",
                TotalPagar = 400,
                IdEmpleado = 1
            };
            db.RegistroReservacion.Add(reservaOcupada);
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resumen = await response.Content.ReadFromJsonAsync<DashboardResumenDto>();
        Assert.NotNull(resumen);
        Assert.Equal(1, resumen.CabanasOcupadasHoy);
        Assert.Equal(1, resumen.CabanasDisponibles);
        Assert.Equal(3, resumen.PersonasEsperadasHoy);
        Assert.Single(resumen.ProximasReservas);
        Assert.Equal("Juan Perez", resumen.ProximasReservas.First().NombreCliente);
    }

    [Fact]
    public async Task GetResumen_CuandoHayReservasCanceladas_DeberiaIgnorarlas()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.ReservacionServicios.RemoveRange(db.ReservacionServicios);
            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Cabanas.RemoveRange(db.Cabanas);
            db.EstadosCabanas.RemoveRange(db.EstadosCabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana);
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);
            await db.SaveChangesAsync();

            var persona = new Persona { Nombres = "Luis", Apellidos = "Gomez", DPI = "456", Telefono = "456" };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            var cliente = new Cliente { IdPersona = persona.IdPersona };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var estadoDisp = new EstadoCabana { IdEstadoCabana = 1, Nombre = "Disponible" };
            db.EstadosCabanas.Add(estadoDisp);
            await db.SaveChangesAsync();

            var tipo = new TipoCabana { Nombre = "Doble", Precio = 300, Capacidad = 4 };
            db.TiposCabana.Add(tipo);
            await db.SaveChangesAsync();

            var cabana = new Cabana { IdTipoCabana = tipo.IdTipoCabana, IdEstadoCabana = estadoDisp.IdEstadoCabana };
            db.Cabanas.Add(cabana);
            await db.SaveChangesAsync();

            var reservaCancelada = new RegistroReservacion
            {
                IdCliente = cliente.IdCliente,
                IdCabana = cabana.IdCabana,
                FechaEntrada = DateTime.Today,
                FechaSalida = DateTime.Today.AddDays(1),
                CantidadPersonas = 4,
                EstadoReserva = "Cancelada",
                TotalPagar = 300,
                IdEmpleado = 1
            };
            db.RegistroReservacion.Add(reservaCancelada);
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resumen = await response.Content.ReadFromJsonAsync<DashboardResumenDto>();
        Assert.NotNull(resumen);
        Assert.Equal(0, resumen.CabanasOcupadasHoy);
        Assert.Equal(1, resumen.CabanasDisponibles);
        Assert.Equal(0, resumen.PersonasEsperadasHoy);
        Assert.Empty(resumen.ProximasReservas);
    }
}