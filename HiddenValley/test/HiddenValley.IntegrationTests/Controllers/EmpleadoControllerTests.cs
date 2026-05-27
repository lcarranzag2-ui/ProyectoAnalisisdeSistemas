using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HiddenValley.IntegrationTests.Controllers;

public class EmpleadoControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public EmpleadoControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ConFiltroPuesto_DeberiaRetornarSoloEmpleadosDeEsePuesto()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);
            db.Personas.RemoveRange(db.Personas);

            var puesto1 = new PuestoTrabajo { IdPuestoTrabajo = 1, Nombre = "Administrador" };
            var puesto2 = new PuestoTrabajo { IdPuestoTrabajo = 2, Nombre = "Mantenimiento" };
            db.PuestosTrabajo.AddRange(puesto1, puesto2);

            var per1 = new Persona { IdPersona = 100, Nombres = "Carlos", Apellidos = "Mérida", Telefono = "55551111" };
            var per2 = new Persona { IdPersona = 101, Nombres = "María", Apellidos = "Tecún", Telefono = "55552222" };
            db.Personas.AddRange(per1, per2);

            db.Empleados.AddRange(
                new Empleado { IdEmpleado = 1, IdPersona = 100, IdPuestoTrabajo = 1 },
                new Empleado { IdEmpleado = 2, IdPersona = 101, IdPuestoTrabajo = 2 }
            );
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/empleado?search=Administrador&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jsonTexto = await response.Content.ReadAsStringAsync();
        Assert.Contains("Carlos Mérida", jsonTexto);
        Assert.Contains("Administrador", jsonTexto);
        Assert.DoesNotContain("María Tecún", jsonTexto);
    }

    [Fact]
    public async Task Create_FlujoCompletoPersonaYPuesto_DeberiaCrearEmpleadoExitosamente()
    {
        int idPersonaValida;
        int idPuestoValido;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);
            db.Personas.RemoveRange(db.Personas);

            var puesto = new PuestoTrabajo { Nombre = "Recepcionista" };
            var persona = new Persona { Nombres = "Keilyta", Apellidos = "Lopez", Telefono = "44445555", Gmail = "keily@hiddenvalley.com" };
            
            db.PuestosTrabajo.Add(puesto);
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            idPuestoValido = puesto.IdPuestoTrabajo;
            idPersonaValida = persona.IdPersona;
        }

        var dto = new EmpleadoCreateDTO
        {
            IdPersona = idPersonaValida,
            IdPuestoTrabajo = idPuestoValido
        };

        var response = await _client.PostAsJsonAsync("api/empleado", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var empleadoEnDb = await db.Empleados
                .Include(e => e.Persona)
                .Include(e => e.PuestoTrabajo)
                .FirstOrDefaultAsync(e => e.IdPersona == idPersonaValida);

            Assert.NotNull(empleadoEnDb);
            Assert.Equal("Recepcionista", empleadoEnDb.PuestoTrabajo!.Nombre);
            Assert.Equal("Keilyta Lopez", $"{empleadoEnDb.Persona!.Nombres} {empleadoEnDb.Persona.Apellidos}");
        }
    }

    [Fact]
    public async Task Create_CuandoPersonaYaEsEmpleado_DeberiaRetornarBadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);
            db.Personas.RemoveRange(db.Personas);

            db.PuestosTrabajo.Add(new PuestoTrabajo { IdPuestoTrabajo = 5, Nombre = "Seguridad" });
            db.Personas.Add(new Persona { IdPersona = 200, Nombres = "Juan", Apellidos = "Matus" });
            db.Empleados.Add(new Empleado { IdEmpleado = 10, IdPersona = 200, IdPuestoTrabajo = 5 });
            await db.SaveChangesAsync();
        }

        var dto = new EmpleadoCreateDTO { IdPersona = 200, IdPuestoTrabajo = 5 };
        var response = await _client.PostAsJsonAsync("api/empleado", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("Esta persona ya es un empleado registrado.", mensaje);
    }

    [Fact]
    public async Task Patch_CuandoSeCambianDatos_DeberiaActualizarPuestoYPersona()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);
            db.Personas.RemoveRange(db.Personas);

            var pInicial = new PuestoTrabajo { IdPuestoTrabajo = 10, Nombre = "Puesto Viejo" };
            var pNuevo = new PuestoTrabajo { IdPuestoTrabajo = 11, Nombre = "Puesto Nuevo" };
            db.PuestosTrabajo.AddRange(pInicial, pNuevo);

            db.Personas.Add(new Persona { IdPersona = 300, Nombres = "Static", Apellidos = "User", Telefono = "1111" });
            db.Empleados.Add(new Empleado { IdEmpleado = 50, IdPersona = 300, IdPuestoTrabajo = 10 });
            await db.SaveChangesAsync();
        }

        var dto = new EmpleadoPatchDTO
        {
            IdPuestoTrabajo = 11, 
            Telefono = "99998888"  
        };
        var response = await _client.PatchAsJsonAsync("api/empleado/50", dto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emp = await db.Empleados.Include(e => e.Persona).FirstOrDefaultAsync(e => e.IdEmpleado == 50);
            
            Assert.NotNull(emp);
            Assert.Equal(11, emp.IdPuestoTrabajo); 
            Assert.Equal("99998888", emp.Persona!.Telefono); 
        }
    }

    [Fact]
    public async Task Delete_EmpleadoExistente_DeberiaRemoverloDeLaBaseDeDatos()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);

            db.Empleados.Add(new Empleado { IdEmpleado = 80, IdPersona = 1, IdPuestoTrabajo = 1 });
            await db.SaveChangesAsync();
        }
        var response = await _client.DeleteAsync("api/empleado/80");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existe = await db.Empleados.AnyAsync(e => e.IdEmpleado == 80);
            Assert.False(existe); 
        }
    }

    [Fact]
    public async Task Create_CuandoPuestoTrabajoNoExiste_DeberiaRetornarBadRequest()
    {
        int idPersonaValida;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Personas.RemoveRange(db.Personas);

            var persona = new Persona { IdPersona = 400, Nombres = "Estuardo", Apellidos = "Diaz" };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();
            idPersonaValida = persona.IdPersona;
        }

        var dto = new EmpleadoCreateDTO
        {
            IdPersona = idPersonaValida,
            IdPuestoTrabajo = 99999 
        };
        var response = await _client.PostAsJsonAsync("api/empleado", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("El puesto de trabajo no existe.", mensaje);
    }

    [Fact]
    public async Task Patch_CuandoNuevoPuestoNoExiste_DeberiaRetornarBadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Empleados.RemoveRange(db.Empleados);
            db.PuestosTrabajo.RemoveRange(db.PuestosTrabajo);

            db.PuestosTrabajo.Add(new PuestoTrabajo { IdPuestoTrabajo = 20, Nombre = "Guía Turístico" });
            db.Personas.Add(new Persona { IdPersona = 500, Nombres = "Alejandro", Apellidos = "Guerra" });
            db.Empleados.Add(new Empleado { IdEmpleado = 12, IdPersona = 500, IdPuestoTrabajo = 20 });
            await db.SaveChangesAsync();
        }

        var dto = new EmpleadoPatchDTO
        {
            IdPuestoTrabajo = 88888 
        };

        var response = await _client.PatchAsJsonAsync("api/empleado/12", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensaje = await response.Content.ReadAsStringAsync();
        Assert.Contains("El nuevo puesto no es válido.", mensaje);
    }
}