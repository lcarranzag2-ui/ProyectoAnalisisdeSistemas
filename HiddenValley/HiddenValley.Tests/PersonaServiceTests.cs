using HiddenValley.API.Models;
using HiddenValley.API.Services;
using HiddenValley.Shared.DTOs;
using HiddenValley.Tests.Helpers;

namespace HiddenValley.Tests;

public class PersonaServiceTests
{
    private static PersonaCreateDto PersonaDto(string dpi = "1234567890101") => new()
    {
        Nombres = "Juan",
        Apellidos = "Pérez",
        DPI = dpi,
        Telefono = "55551234",
        Gmail = "juan@mail.com",
        Direccion = "Jalapa"
    };

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_DpiNuevo_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_DpiNuevo_RetornaSuccess));
        var service = new PersonaService(ctx);

        var (success, msg, id) = await service.CreateAsync(PersonaDto());

        Assert.True(success);
        Assert.NotNull(id);
    }

    [Fact]
    public async Task Create_DpiDuplicado_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_DpiDuplicado_RetornaError));
        ctx.Personas.Add(new Persona { IdPersona = 1, Nombres = "Ana", Apellidos = "López", DPI = "1234567890101", Telefono = "55550000", Direccion = "Jalapa" });
        ctx.SaveChanges();
        var service = new PersonaService(ctx);

        var (success, msg, id) = await service.CreateAsync(PersonaDto("1234567890101"));

        Assert.False(success);
        Assert.Null(id);
    }

    // ── ExisteDpiAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ExisteDpi_DpiRegistrado_RetornaTrue()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(ExisteDpi_DpiRegistrado_RetornaTrue));
        ctx.Personas.Add(new Persona { IdPersona = 1, Nombres = "Ana", Apellidos = "López", DPI = "9999999999999", Telefono = "55550000", Direccion = "Jalapa" });
        ctx.SaveChanges();
        var service = new PersonaService(ctx);

        var existe = await service.ExisteDpiAsync("9999999999999");

        Assert.True(existe);
    }

    [Fact]
    public async Task ExisteDpi_DpiNoRegistrado_RetornaFalse()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(ExisteDpi_DpiNoRegistrado_RetornaFalse));
        var service = new PersonaService(ctx);

        var existe = await service.ExisteDpiAsync("0000000000000");

        Assert.False(existe);
    }

    // ── PatchAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Patch_PersonaExiste_ActualizaNombres()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_PersonaExiste_ActualizaNombres));
        ctx.Personas.Add(new Persona { IdPersona = 1, Nombres = "Viejo", Apellidos = "Apellido", Telefono = "55550000", Direccion = "Jalapa" });
        ctx.SaveChanges();
        var service = new PersonaService(ctx);

        var (success, _) = await service.PatchAsync(1, new PersonaPatchDto { Nombres = "Nuevo" });

        Assert.True(success);
        Assert.Equal("Nuevo", ctx.Personas.Find(1)!.Nombres);
    }

    [Fact]
    public async Task Patch_PersonaNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_PersonaNoExiste_RetornaError));
        var service = new PersonaService(ctx);

        var (success, _) = await service.PatchAsync(999, new PersonaPatchDto { Nombres = "X" });

        Assert.False(success);
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_PersonaSinVinculos_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Delete_PersonaSinVinculos_RetornaSuccess));
        ctx.Personas.Add(new Persona { IdPersona = 1, Nombres = "Juan", Apellidos = "Pérez", Telefono = "55550000", Direccion = "Jalapa" });
        ctx.SaveChanges();
        var service = new PersonaService(ctx);

        var (success, _) = await service.DeleteAsync(1);

        Assert.True(success);
    }

    [Fact]
    public async Task Delete_PersonaVinculadaACliente_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Delete_PersonaVinculadaACliente_RetornaError));
        ctx.Personas.Add(new Persona { IdPersona = 1, Nombres = "Juan", Apellidos = "Pérez", Telefono = "55550000", Direccion = "Jalapa" });
        ctx.Clientes.Add(new Cliente { IdCliente = 1, IdPersona = 1 });
        ctx.SaveChanges();
        var service = new PersonaService(ctx);

        var (success, _) = await service.DeleteAsync(1);

        Assert.False(success);
    }
}
