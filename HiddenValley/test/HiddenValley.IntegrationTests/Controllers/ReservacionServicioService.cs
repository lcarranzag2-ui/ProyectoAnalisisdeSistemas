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

public class ReservacionServiciosControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ReservacionServiciosControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_CuandoServicioYaExisteEnReserva_DeberiaSumarCantidadYRetornarOk()
    {
        int idReservacionExistente;
        int idServicioExistente;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.ReservacionServicios.RemoveRange(db.ReservacionServicios);
            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Servicio.RemoveRange(db.Servicio);
            db.Cabanas.RemoveRange(db.Cabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);
            await db.SaveChangesAsync();

            var persona = new Persona { Nombres = "Cristian", Apellidos = "Chamo", DPI = "1111", Telefono = "12345678" };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            var cliente = new Cliente { IdPersona = persona.IdPersona };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var tipo = new TipoCabana { Nombre = "Estándar", Capacidad = 4, Precio = 250 };
            db.TiposCabana.Add(tipo);
            await db.SaveChangesAsync();

            var cabana = new Cabana { IdTipoCabana = tipo.IdTipoCabana, IdEstadoCabana = 1 };
            db.Cabanas.Add(cabana);
            await db.SaveChangesAsync();

            var servicio = new Servicio { Nombre = "Acceso Piscina Olimpica", Precio = 50 };
            db.Servicio.Add(servicio);
            await db.SaveChangesAsync();
            idServicioExistente = servicio.IdServicio;

            var reserva = new RegistroReservacion
            {
                IdCliente = cliente.IdCliente,
                IdCabana = cabana.IdCabana,
                FechaEntrada = DateTime.Now.AddDays(1),
                FechaSalida = DateTime.Now.AddDays(3),
                CantidadPersonas = 2,
                EstadoReserva = "Recibida",
                TotalPagar = 500,
                IdEmpleado = 1
            };
            db.RegistroReservacion.Add(reserva);
            await db.SaveChangesAsync();
            idReservacionExistente = reserva.Id; 
            
            db.ReservacionServicios.Add(new ReservacionServicio
            {
                IdReservacion = idReservacionExistente,
                IdServicio = idServicioExistente,
                Cantidad = 1
            });
            await db.SaveChangesAsync();
        }

        var dtoAgregarMas = new ReservacionServicioCreateDto
        {
            IdReservacion = idReservacionExistente,
            IdServicio = idServicioExistente,
            Cantidad = 2
        };

        var response = await _client.PostAsJsonAsync("api/reservacionservicio", dtoAgregarMas);

        Assert.True(response.IsSuccessStatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var registroEnDb = await db.ReservacionServicios
                .FirstOrDefaultAsync(x => x.IdReservacion == idReservacionExistente && x.IdServicio == idServicioExistente);
            
            Assert.NotNull(registroEnDb);
            Assert.Equal(3, registroEnDb.Cantidad);
        }
    }

    [Fact]
    public async Task GetPagedAsync_DeberiaTotalizarCorrectamenteLosServiciosDeLaReservacion()
    {
        int idReservacion;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.ReservacionServicios.RemoveRange(db.ReservacionServicios);
            db.RegistroReservacion.RemoveRange(db.RegistroReservacion);
            db.Servicio.RemoveRange(db.Servicio);
            db.Cabanas.RemoveRange(db.Cabanas);
            db.TiposCabana.RemoveRange(db.TiposCabana); 
            db.Clientes.RemoveRange(db.Clientes);
            db.Personas.RemoveRange(db.Personas);
            await db.SaveChangesAsync();

            var persona = new Persona { Nombres = "Maria", Apellidos = "Chamo", DPI = "4444", Telefono = "12345678" }; //
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            var cliente = new Cliente { IdPersona = persona.IdPersona };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            var tipo = new TipoCabana { Nombre = "Estándar", Capacidad = 4, Precio = 300 }; 
            db.TiposCabana.Add(tipo); //
            await db.SaveChangesAsync();

            var cabana = new Cabana { IdTipoCabana = tipo.IdTipoCabana, IdEstadoCabana = 1 }; 
            db.Cabanas.Add(cabana);
            await db.SaveChangesAsync();

            var servicioPiscina = new Servicio { Nombre = "Piscina", Precio = 50 };
            var servicioTour = new Servicio { Nombre = "Tour", Precio = 150 };
            db.Servicio.AddRange(servicioPiscina, servicioTour);
            await db.SaveChangesAsync();

            var reserva = new RegistroReservacion 
            { 
                IdCliente = cliente.IdCliente,
                IdCabana = cabana.IdCabana,
                FechaEntrada = DateTime.Now.AddDays(1),
                FechaSalida = DateTime.Now.AddDays(3),
                CantidadPersonas = 2,
                EstadoReserva = "Recibida",
                TotalPagar = 600,
                IdEmpleado = 1
            };
            db.RegistroReservacion.Add(reserva);
            await db.SaveChangesAsync();
            idReservacion = reserva.Id;

            db.ReservacionServicios.AddRange(
                new ReservacionServicio { IdReservacion = idReservacion, IdServicio = servicioPiscina.IdServicio, Cantidad = 2 },
                new ReservacionServicio { IdReservacion = idReservacion, IdServicio = servicioTour.IdServicio, Cantidad = 1 }
            );
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/reservacionservicio?page=1&size=10");
        
        response.EnsureSuccessStatusCode(); 
        var resultado = await response.Content.ReadFromJsonAsync<PagedResultReservacionServicio<ReservacionServicioReadDto>>();
        
        Assert.NotNull(resultado);
        var dtoReserva = resultado.Items.FirstOrDefault(x => x.IdReservacion == idReservacion);
        Assert.NotNull(dtoReserva);
        Assert.Equal(2, dtoReserva.Servicios.Count); 
    }
}