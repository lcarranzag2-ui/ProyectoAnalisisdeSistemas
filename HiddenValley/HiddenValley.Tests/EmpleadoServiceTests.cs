using HiddenValley.API.Models;
using HiddenValley.API.Services;
using HiddenValley.Shared.DTOs;
using HiddenValley.Tests.Helpers;

namespace HiddenValley.Tests;

public class EmpleadoServiceTests
{
    private static void SeedBase(HiddenValley.API.Data.ApplicationDbContext ctx)
    {
        ctx.Personas.Add(new Persona { IdPersona = 1, Nombres = "Carlos", Apellidos = "Morales", Telefono = "55553333", Direccion = "Jalapa" });
        ctx.PuestosTrabajo.Add(new PuestoTrabajo { IdPuestoTrabajo = 1, Nombre = "Recepcionista" });
        ctx.SaveChanges();
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_DatosValidos_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_DatosValidos_RetornaSuccess));
        SeedBase(ctx);
        var service = new EmpleadoService(ctx);

        var (success, msg, id) = await service.CreateAsync(new EmpleadoCreateDTO { IdPersona = 1, IdPuestoTrabajo = 1 });

        Assert.True(success);
        Assert.NotNull(id);
    }

    [Fact]
    public async Task Create_PersonaNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_PersonaNoExiste_RetornaError));
        SeedBase(ctx);
        var service = new EmpleadoService(ctx);

        var (success, _, _) = await service.CreateAsync(new EmpleadoCreateDTO { IdPersona = 99, IdPuestoTrabajo = 1 });

        Assert.False(success);
    }

    [Fact]
    public async Task Create_PuestoNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_PuestoNoExiste_RetornaError));
        SeedBase(ctx);
        var service = new EmpleadoService(ctx);

        var (success, _, _) = await service.CreateAsync(new EmpleadoCreateDTO { IdPersona = 1, IdPuestoTrabajo = 99 });

        Assert.False(success);
    }

    [Fact]
    public async Task Create_PersonaYaEsEmpleado_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Create_PersonaYaEsEmpleado_RetornaError));
        SeedBase(ctx);
        ctx.Empleados.Add(new Empleado { IdEmpleado = 1, IdPersona = 1, IdPuestoTrabajo = 1 });
        ctx.SaveChanges();
        var service = new EmpleadoService(ctx);

        var (success, _, _) = await service.CreateAsync(new EmpleadoCreateDTO { IdPersona = 1, IdPuestoTrabajo = 1 });

        Assert.False(success);
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_EmpleadoExiste_RetornaSuccess()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Delete_EmpleadoExiste_RetornaSuccess));
        SeedBase(ctx);
        ctx.Empleados.Add(new Empleado { IdEmpleado = 1, IdPersona = 1, IdPuestoTrabajo = 1 });
        ctx.SaveChanges();
        var service = new EmpleadoService(ctx);

        var (success, _) = await service.DeleteAsync(1);

        Assert.True(success);
    }

    [Fact]
    public async Task Delete_EmpleadoNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Delete_EmpleadoNoExiste_RetornaError));
        var service = new EmpleadoService(ctx);

        var (success, _) = await service.DeleteAsync(999);

        Assert.False(success);
    }

    // ── PatchAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Patch_EmpleadoExiste_ActualizaTelefono()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_EmpleadoExiste_ActualizaTelefono));
        SeedBase(ctx);
        ctx.Empleados.Add(new Empleado { IdEmpleado = 1, IdPersona = 1, IdPuestoTrabajo = 1 });
        ctx.SaveChanges();
        var service = new EmpleadoService(ctx);

        var (success, _) = await service.PatchAsync(1, new EmpleadoPatchDTO { Telefono = "44448888" });

        Assert.True(success);
        Assert.Equal("44448888", ctx.Personas.Find(1)!.Telefono);
    }

    [Fact]
    public async Task Patch_EmpleadoNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_EmpleadoNoExiste_RetornaError));
        var service = new EmpleadoService(ctx);

        var (success, _) = await service.PatchAsync(999, new EmpleadoPatchDTO { Telefono = "44448888" });

        Assert.False(success);
    }

    [Fact]
    public async Task Patch_PuestoNuevoNoExiste_RetornaError()
    {
        using var ctx = DbContextHelper.CreateInMemory(nameof(Patch_PuestoNuevoNoExiste_RetornaError));
        SeedBase(ctx);
        ctx.Empleados.Add(new Empleado { IdEmpleado = 1, IdPersona = 1, IdPuestoTrabajo = 1 });
        ctx.SaveChanges();
        var service = new EmpleadoService(ctx);

        var (success, _) = await service.PatchAsync(1, new EmpleadoPatchDTO { IdPuestoTrabajo = 99 });

        Assert.False(success);
    }
}
