using HiddenValley.API.Models;
using HiddenValley.API.Services;
using HiddenValley.Shared.DTOs;
using HiddenValley.Tests.Helpers;

namespace HiddenValley.Tests;

public class ClienteServiceTests
{
    private static void SeedPersona(HiddenValley.API.Data.ApplicationDbContext ctx, int id = 1)
    {
        ctx.Personas.Add(new Persona { IdPersona = id, Nombres = "María", Apellidos = "García", Telefono = "55559999", Direccion = "Jalapa" });
        ctx.SaveChanges();
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_PersonaExisteYNoEsCliente_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_PersonaExisteYNoEsCliente_RetornaSuccess));
        SeedPersona(ctx);
        var service = new ClienteService(ctx);

        var (success, msg, id) = await service.CreateAsync(new ClienteCreateDTO { IdPersona = 1 });

        Assert.True(success);
        Assert.NotNull(id);
    }

    [Fact]
    public async Task Create_PersonaNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_PersonaNoExiste_RetornaError));
        var service = new ClienteService(ctx);

        var (success, msg, id) = await service.CreateAsync(new ClienteCreateDTO { IdPersona = 99 });

        Assert.False(success);
    }

    [Fact]
    public async Task Create_PersonaYaEsCliente_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_PersonaYaEsCliente_RetornaError));
        SeedPersona(ctx);
        ctx.Clientes.Add(new Cliente { IdCliente = 1, IdPersona = 1 });
        ctx.SaveChanges();
        var service = new ClienteService(ctx);

        var (success, msg, id) = await service.CreateAsync(new ClienteCreateDTO { IdPersona = 1 });

        Assert.False(success);
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_ClienteSinHistorial_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Delete_ClienteSinHistorial_RetornaSuccess));
        SeedPersona(ctx);
        ctx.Clientes.Add(new Cliente { IdCliente = 1, IdPersona = 1 });
        ctx.SaveChanges();
        var service = new ClienteService(ctx);

        var (success, _) = await service.DeleteAsync(1);

        Assert.True(success);
    }

    [Fact]
    public async Task Delete_ClienteConHistorial_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Delete_ClienteConHistorial_RetornaError));
        SeedPersona(ctx);
        ctx.Clientes.Add(new Cliente { IdCliente = 1, IdPersona = 1 });
        ctx.RegistroReservacion.Add(new RegistroReservacion
        {
            Id = 1, IdCliente = 1, IdCabana = 1, IdEmpleado = 1,
            FechaEntrada = DateTime.Today.AddDays(1),
            FechaSalida = DateTime.Today.AddDays(3),
            CantidadPersonas = 2, EstadoReserva = "Recibida", TotalPagar = 400
        });
        ctx.SaveChanges();
        var service = new ClienteService(ctx);

        var (success, _) = await service.DeleteAsync(1);

        Assert.False(success);
    }

    [Fact]
    public async Task Delete_ClienteNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Delete_ClienteNoExiste_RetornaError));
        var service = new ClienteService(ctx);

        var (success, _) = await service.DeleteAsync(999);

        Assert.False(success);
    }

    // ── PatchAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Patch_ClienteExiste_ActualizaTelefono()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_ClienteExiste_ActualizaTelefono));
        SeedPersona(ctx);
        ctx.Clientes.Add(new Cliente { IdCliente = 1, IdPersona = 1 });
        ctx.SaveChanges();
        var service = new ClienteService(ctx);

        var (success, _) = await service.PatchAsync(1, new ClientePatchDTO { Telefono = "44440000" });

        Assert.True(success);
        Assert.Equal("44440000", ctx.Personas.Find(1)!.Telefono);
    }

    // ── GetByIdOrFiltroAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetByFiltro_PorTelefono_RetornaCliente()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(GetByFiltro_PorTelefono_RetornaCliente));
        SeedPersona(ctx);
        ctx.Clientes.Add(new Cliente { IdCliente = 1, IdPersona = 1 });
        ctx.SaveChanges();
        var service = new ClienteService(ctx);

        var result = await service.GetByIdOrFiltroAsync("55559999");

        Assert.NotNull(result);
        Assert.Equal(1, result.IdCliente);
    }

    [Fact]
    public async Task GetByFiltro_FiltroInexistente_RetornaNull()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(GetByFiltro_FiltroInexistente_RetornaNull));
        var service = new ClienteService(ctx);

        var result = await service.GetByIdOrFiltroAsync("00000000");

        Assert.Null(result);
    }
}
