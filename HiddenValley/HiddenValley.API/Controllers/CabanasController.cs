using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CabanasController(ApplicationDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllCabanas()
    {
        var cabanas = await context.Cabanas
            .Include(c => c.TipoCabana)
            .Include(c => c.EstadoCabana)
            .Select(c => new
            {
                c.IdCabana,
                TipoCabana = c.TipoCabana != null ? c.TipoCabana.Nombre : "Desconocido",
                Capacidad = c.TipoCabana != null ? c.TipoCabana.Capacidad : 0,
                Precio = c.TipoCabana != null ? c.TipoCabana.Precio : 0,
                EstadoActual = c.EstadoCabana != null ? c.EstadoCabana.Nombre : "Desconocido"
            })
            .ToListAsync();

        return Ok(cabanas);
    }

    [HttpGet("disponibilidad")]
    public async Task<IActionResult> VerificarDisponibilidad([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {

        if (fechaInicio >= fechaFin)
            return BadRequest("La fecha de inicio debe ser menor a la fecha de fin");

        var cabanasOcupadas = await context.RegistroReservacion
            .Where(r => r.FechaEntrada < fechaFin && r.FechaSalida > fechaInicio)
            .Select(r => r.IdCabana)
            .Distinct()
            .ToListAsync();

        var todasCabanas = await context.Cabanas
            .Include(c => c.TipoCabana)
            .Include(c => c.EstadoCabana)
            .Select(c => new
            {
                c.IdCabana,
                TipoCabana = c.TipoCabana != null ? c.TipoCabana.Nombre : "Desconocido",
                Capacidad = c.TipoCabana != null ? c.TipoCabana.Capacidad : 0,
                Precio = c.TipoCabana != null ? c.TipoCabana.Precio : 0,
                EstadoActual = c.EstadoCabana != null ? c.EstadoCabana.Nombre : "Desconocido",
                EstaDisponible = !cabanasOcupadas.Contains(c.IdCabana) && 
                                 (c.EstadoCabana != null && c.EstadoCabana.Nombre == "Disponible")
            })
            .ToListAsync();

        return Ok(todasCabanas);
    }

    [HttpPut("cambiar-estado")]
    public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoRequest request)
    {
        var cabana = await context.Cabanas
            .Include(c => c.EstadoCabana)
            .FirstOrDefaultAsync(c => c.IdCabana == request.IdCabana);
            
        if (cabana == null)
            return NotFound("La cabaña no existe.");

        var nuevoEstado = await context.EstadosCabana
            .FirstOrDefaultAsync(e => e.Nombre == request.NuevoEstado);
            
        if (nuevoEstado == null)
            return BadRequest("Estado no válido. Estados permitidos: Disponible, Ocupada, En limpieza");

        var estadoAnteriorId = cabana.IdEstadoCabana;

        cabana.IdEstadoCabana = nuevoEstado.IdEstadoCabana;
        await context.SaveChangesAsync();

        var bitacora = new BitacoraEstados
        {
            IdEstadoAnterior = estadoAnteriorId,
            IdEstadoNuevo = nuevoEstado.IdEstadoCabana,
            IdCabana = request.IdCabana,
            IdEmpleado = request.IdEmpleado,
            FechaCambio = DateTime.UtcNow
        };

        context.BitacoraEstados.Add(bitacora);
        await context.SaveChangesAsync();

        return Ok(new { mensaje = "Estado actualizado correctamente", estadoAnterior = cabana.EstadoCabana?.Nombre, estadoNuevo = request.NuevoEstado });
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarCabana([FromBody] RegistrarCabanaRequest request)
    {

        var tipoCabana = await context.TiposCabana.FindAsync(request.IdTipoCabana);
        if (tipoCabana == null)
            return NotFound("El tipo de cabaña no existe.");

        var estadoCabana = await context.EstadosCabana.FindAsync(request.IdEstadoCabana);
        if (estadoCabana == null)
            return NotFound("El estado de cabaña no existe.");

        var nuevaCabana = new Cabana
        {
            IdTipoCabana = request.IdTipoCabana,
            IdEstadoCabana = request.IdEstadoCabana
        };

        context.Cabanas.Add(nuevaCabana);
        await context.SaveChangesAsync();

        return Ok(new { idCabana = nuevaCabana.IdCabana, mensaje = "Cabaña registrada con éxito" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarCabana(int id)
    {
        var cabana = await context.Cabanas.FindAsync(id);
        if (cabana == null)
            return NotFound("La cabaña no existe.");

        var tieneReservaciones = await context.RegistroReservacion.AnyAsync(r => r.IdCabana == id);
        if (tieneReservaciones)
            return BadRequest("No se puede eliminar la cabaña porque tiene reservaciones asociadas.");

        context.Cabanas.Remove(cabana);
        await context.SaveChangesAsync();

        return Ok(new { mensaje = "Cabaña eliminada con éxito" });
    }
}

public class CambiarEstadoRequest
{
    public int IdCabana { get; set; }
    public string NuevoEstado { get; set; } = string.Empty;
    public int IdEmpleado { get; set; }
}

public class RegistrarCabanaRequest
{
    public int IdTipoCabana { get; set; }
    public int IdEstadoCabana { get; set; }
}