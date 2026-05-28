using HiddenValley.API.Models;
using HiddenValley.API.Services;
using HiddenValley.Shared.DTOs;
using HiddenValley.Tests.Helpers;

namespace HiddenValley.Tests;

public class CabanaServiceTests
{
    // ── helpers ──────────────────────────────────────────────────────────────
    private static (TipoCabana tipo, EstadoCabana estado, Cabana cabana) SeedCabana(
        HiddenValley.API.Data.ApplicationDbContext ctx,
        string estadoNombre = "Disponible")
    {
        var tipo = new TipoCabana { IdTipoCabana = 1, Nombre = "Estándar", Capacidad = 4, Precio = 200 };
        var estado = new EstadoCabana { IdEstadoCabana = 1, Nombre = estadoNombre };
        var cabana = new Cabana { IdCabana = 1, IdTipoCabana = 1, IdEstadoCabana = 1 };
        ctx.TiposCabana.Add(tipo);
        ctx.EstadosCabana.Add(estado);
        ctx.Cabanas.Add(cabana);
        ctx.SaveChanges();
        return (tipo, estado, cabana);
    }

    // ── RegistrarCabanaAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task RegistrarCabana_TipoExiste_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(RegistrarCabana_TipoExiste_RetornaSuccess));
        ctx.TiposCabana.Add(new TipoCabana { IdTipoCabana = 1, Nombre = "Estándar", Capacidad = 4, Precio = 200 });
        ctx.SaveChanges();

        var service = new CabanaService(ctx);
        var (success, msg, id) = await service.RegistrarCabanaAsync(new RegistrarCabanaRequest { IdTipoCabana = 1, IdEstadoCabana = 1 });

        Assert.True(success);
        Assert.NotNull(id);
    }

    [Fact]
    public async Task RegistrarCabana_TipoNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(RegistrarCabana_TipoNoExiste_RetornaError));
        var service = new CabanaService(ctx);

        var (success, msg, id) = await service.RegistrarCabanaAsync(new RegistrarCabanaRequest { IdTipoCabana = 99, IdEstadoCabana = 1 });

        Assert.False(success);
        Assert.Null(id);
    }

    // ── EliminarCabanaAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task EliminarCabana_SinReservaciones_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(EliminarCabana_SinReservaciones_RetornaSuccess));
        SeedCabana(ctx);
        var service = new CabanaService(ctx);

        var (success, msg) = await service.EliminarCabanaAsync(1);

        Assert.True(success);
    }

    [Fact]
    public async Task EliminarCabana_ConReservaciones_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(EliminarCabana_ConReservaciones_RetornaError));
        SeedCabana(ctx);
        ctx.RegistroReservacion.Add(new RegistroReservacion
        {
            Id = 1, IdCabana = 1, IdCliente = 1, IdEmpleado = 1,
            FechaEntrada = DateTime.Today.AddDays(1),
            FechaSalida = DateTime.Today.AddDays(3),
            CantidadPersonas = 2, EstadoReserva = "Recibida", TotalPagar = 400
        });
        ctx.SaveChanges();
        var service = new CabanaService(ctx);

        var (success, msg) = await service.EliminarCabanaAsync(1);

        Assert.False(success);
    }

    [Fact]
    public async Task EliminarCabana_NoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(EliminarCabana_NoExiste_RetornaError));
        var service = new CabanaService(ctx);

        var (success, _) = await service.EliminarCabanaAsync(999);

        Assert.False(success);
    }

    // ── CambiarEstadoAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CambiarEstado_EstadoValido_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(CambiarEstado_EstadoValido_RetornaSuccess));
        SeedCabana(ctx);
        ctx.EstadosCabana.Add(new EstadoCabana { IdEstadoCabana = 2, Nombre = "Mantenimiento" });
        ctx.SaveChanges();
        var service = new CabanaService(ctx);

        var (success, msg, data) = await service.CambiarEstadoAsync(new CambiarEstadoRequest { IdCabana = 1, NuevoEstado = "Mantenimiento" });

        Assert.True(success);
    }

    [Fact]
    public async Task CambiarEstado_EstadoInvalido_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(CambiarEstado_EstadoInvalido_RetornaError));
        SeedCabana(ctx);
        var service = new CabanaService(ctx);

        var (success, msg, data) = await service.CambiarEstadoAsync(new CambiarEstadoRequest { IdCabana = 1, NuevoEstado = "EstadoFalso" });

        Assert.False(success);
    }

    // ── GetPagedAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPaged_SinFiltro_RetornaTodas()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(GetPaged_SinFiltro_RetornaTodas));
        SeedCabana(ctx);
        var service = new CabanaService(ctx);

        var result = await service.GetPagedAsync(null, 1, 10);

        Assert.Equal(1, result.TotalRecords);
    }
}
