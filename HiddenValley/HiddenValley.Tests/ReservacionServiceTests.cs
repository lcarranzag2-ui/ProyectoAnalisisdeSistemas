using HiddenValley.API.Models;
using HiddenValley.API.Services;
using HiddenValley.Shared.DTOs;
using HiddenValley.Tests.Helpers;

namespace HiddenValley.Tests;

public class ReservacionServiceTests
{
    // ── seed helpers ──────────────────────────────────────────────────────────
    private static void SeedBase(HiddenValley.API.Data.ApplicationDbContext ctx)
    {
        // Separar en bloques para evitar conflicto con el doble DbSet de EstadoCabana
        // que existe en ApplicationDbContext (EstadosCabana + EstadosCabanas).
        ctx.Personas.Add(new Persona { IdPersona = 1, Nombres = "Luis", Apellidos = "Ramos", Telefono = "55551111", Direccion = "Jalapa" });
        ctx.Clientes.Add(new Cliente { IdCliente = 1, IdPersona = 1 });
        ctx.TiposCabana.Add(new TipoCabana { IdTipoCabana = 1, Nombre = "Estándar", Capacidad = 4, Precio = 300 });
        ctx.Cabanas.Add(new Cabana { IdCabana = 1, IdTipoCabana = 1, IdEstadoCabana = 1 });
        ctx.SaveChanges();
    }

    private static ReservacionCreateDto DtoValido() => new()
    {
        IdCliente = 1,
        Telefono = "55551111",
        IdCabana = 1,
        FechaEntrada = DateTime.UtcNow.Date.AddDays(2),
        FechaSalida = DateTime.UtcNow.Date.AddDays(4),
        CantidadPersonas = 2
    };

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_DatosValidos_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_DatosValidos_RetornaSuccess));
        SeedBase(ctx);
        var service = new ReservacionService(ctx);

        var (success, msg, id, total, noches) = await service.CreateAsync(DtoValido());

        Assert.True(success);
        Assert.NotNull(id);
        Assert.Equal(2, noches);
        Assert.Equal(600m, total);
    }

    [Fact]
    public async Task Create_FechaSalidaMenorEntrada_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_FechaSalidaMenorEntrada_RetornaError));
        SeedBase(ctx);
        var service = new ReservacionService(ctx);
        var dto = DtoValido();
        dto.FechaSalida = dto.FechaEntrada.AddDays(-1);

        var (success, msg, _, _, _) = await service.CreateAsync(dto);

        Assert.False(success);
        Assert.Contains("salida", msg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Create_ClienteNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_ClienteNoExiste_RetornaError));
        SeedBase(ctx);
        var service = new ReservacionService(ctx);
        var dto = DtoValido();
        dto.IdCliente = 99;

        var (success, _, _, _, _) = await service.CreateAsync(dto);

        Assert.False(success);
    }

    [Fact]
    public async Task Create_TelefonoNoCoincide_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_TelefonoNoCoincide_RetornaError));
        SeedBase(ctx);
        var service = new ReservacionService(ctx);
        var dto = DtoValido();
        dto.Telefono = "00000000";

        var (success, _, _, _, _) = await service.CreateAsync(dto);

        Assert.False(success);
    }

    [Fact]
    public async Task Create_CapacidadExcedida_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_CapacidadExcedida_RetornaError));
        SeedBase(ctx);
        var service = new ReservacionService(ctx);
        var dto = DtoValido();
        dto.CantidadPersonas = 10; // capacidad es 4

        var (success, _, _, _, _) = await service.CreateAsync(dto);

        Assert.False(success);
    }

    [Fact]
    public async Task Create_TraslapeEnFechas_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_TraslapeEnFechas_RetornaError));
        SeedBase(ctx);
        // reserva existente que ocupa las mismas fechas
        ctx.RegistroReservacion.Add(new RegistroReservacion
        {
            Id = 1, IdCabana = 1, IdCliente = 1, IdEmpleado = 1,
            FechaEntrada = DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = DateTime.UtcNow.Date.AddDays(5),
            CantidadPersonas = 2, EstadoReserva = "Confirmada", TotalPagar = 600
        });
        ctx.SaveChanges();

        // segundo cliente para no chocar con "ya tiene reserva activa"
        ctx.Personas.Add(new Persona { IdPersona = 2, Nombres = "Ana", Apellidos = "Paz", Telefono = "55552222", Direccion = "Jalapa" });
        ctx.Clientes.Add(new Cliente { IdCliente = 2, IdPersona = 2 });
        ctx.SaveChanges();

        var service = new ReservacionService(ctx);
        var dto = DtoValido();
        dto.IdCliente = 2;
        dto.Telefono = "55552222";

        var (success, _, _, _, _) = await service.CreateAsync(dto);

        Assert.False(success);
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReservaExiste_RetornaDto()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(GetById_ReservaExiste_RetornaDto));
        SeedBase(ctx);
        ctx.RegistroReservacion.Add(new RegistroReservacion
        {
            Id = 1, IdCabana = 1, IdCliente = 1, IdEmpleado = 1,
            FechaEntrada = DateTime.UtcNow.Date.AddDays(2),
            FechaSalida = DateTime.UtcNow.Date.AddDays(4),
            CantidadPersonas = 2, EstadoReserva = "Recibida", TotalPagar = 600
        });
        ctx.SaveChanges();
        var service = new ReservacionService(ctx);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetById_ReservaNoExiste_RetornaNull()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(GetById_ReservaNoExiste_RetornaNull));
        var service = new ReservacionService(ctx);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    // ── PatchAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Patch_EstadoCancelada_NoPuedeActualizar()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_EstadoCancelada_NoPuedeActualizar));
        SeedBase(ctx);
        ctx.RegistroReservacion.Add(new RegistroReservacion
        {
            Id = 1, IdCabana = 1, IdCliente = 1, IdEmpleado = 1,
            FechaEntrada = DateTime.UtcNow.Date.AddDays(2),
            FechaSalida = DateTime.UtcNow.Date.AddDays(4),
            CantidadPersonas = 2, EstadoReserva = "Cancelada", TotalPagar = 600
        });
        ctx.SaveChanges();
        var service = new ReservacionService(ctx);

        var (success, msg, _) = await service.PatchAsync(1, new ReservacionPatchDto { EstadoReserva = "Confirmada" });

        Assert.False(success);
    }

    [Fact]
    public async Task Patch_EstadoInvalido_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_EstadoInvalido_RetornaError));
        SeedBase(ctx);
        ctx.RegistroReservacion.Add(new RegistroReservacion
        {
            Id = 1, IdCabana = 1, IdCliente = 1, IdEmpleado = 1,
            FechaEntrada = DateTime.UtcNow.Date.AddDays(2),
            FechaSalida = DateTime.UtcNow.Date.AddDays(4),
            CantidadPersonas = 2, EstadoReserva = "Recibida", TotalPagar = 600
        });
        ctx.SaveChanges();
        var service = new ReservacionService(ctx);

        var (success, _, _) = await service.PatchAsync(1, new ReservacionPatchDto { EstadoReserva = "EstadoFalso" });

        Assert.False(success);
    }
}
