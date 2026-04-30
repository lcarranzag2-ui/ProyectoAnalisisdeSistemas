using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.API.Models.Dtos;

namespace HiddenValley.API.Controllers;

/// <summary>
/// Módulo de Registro de Reservación — PROYECT-60.
/// Endpoints para crear, consultar, listar, actualizar, cancelar y eliminar reservaciones,
/// con todas las validaciones automáticas que pide el criterio de aceptación del issue.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReservacionesController(ApplicationDbContext context) : ControllerBase
{
    // Estados válidos según el CHECK constraint de la tabla registroreservacion
    private const string EstadoRecibida   = "Recibida";
    private const string EstadoConfirmada = "Confirmada";
    private const string EstadoCancelada  = "Cancelada";
    private const string EstadoPagada     = "Pagada";

    // Empleado por defecto cuando el endpoint no recibe IdEmpleado.
    private const int EmpleadoDefault = 1;

    // ---------------------------------------------------------------------
    // POST: api/reservaciones
    // Crea una reservación validando disponibilidad, capacidad y datos.
    // ---------------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> CrearReservacion([FromBody] ReservacionCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validar rango de fechas
        var errorFechas = ValidarFechas(dto.FechaEntrada, dto.FechaSalida, esCreacion: true);
        if (errorFechas != null) return BadRequest(new { mensaje = errorFechas });

        // Validar cantidad de personas (defensa adicional al [Range])
        if (dto.CantidadPersonas <= 0)
            return BadRequest(new { mensaje = "La cantidad de personas debe ser mayor a 0." });

        // Validar que el cliente exista
        var cliente = await context.Clientes
            .Include(c => c.Persona)
            .FirstOrDefaultAsync(c => c.IdCliente == dto.IdCliente);
        if (cliente == null)
            return NotFound(new { mensaje = $"El cliente con id {dto.IdCliente} no existe." });

        // Validar que el teléfono coincida con el de la persona del cliente
        if (cliente.Persona == null || cliente.Persona.Telefono != dto.Telefono)
            return BadRequest(new { mensaje = "El teléfono no coincide con el cliente proporcionado." });

        // Validar que el cliente no tenga otra reserva activa
        var clienteTieneReservaActiva = await context.RegistroReservacion
            .AnyAsync(r => r.IdCliente == dto.IdCliente && r.EstadoReserva != EstadoCancelada);
        if (clienteTieneReservaActiva)
            return Conflict(new { mensaje = "El cliente ya tiene otra reserva activa." });

        // Validar que la cabaña exista (incluyendo su tipo para conocer la capacidad y precio)
        var cabana = await context.Cabanas
            .Include(c => c.TipoCabana)
            .FirstOrDefaultAsync(c => c.IdCabana == dto.IdCabana);
        if (cabana == null)
            return NotFound(new { mensaje = $"La cabaña con id {dto.IdCabana} no existe." });
        if (cabana.TipoCabana == null)
            return BadRequest(new { mensaje = "La cabaña no tiene un tipo asignado." });

        // Validar capacidad
        if (dto.CantidadPersonas > cabana.TipoCabana.Capacidad)
            return BadRequest(new
            {
                mensaje = $"La capacidad de la cabaña es {cabana.TipoCabana.Capacidad} personas y se solicitaron {dto.CantidadPersonas}."
            });

        // Validar disponibilidad (que la cabaña no tenga otra reserva activa que se traslape con esas fechas)
        var hayTraslape = await context.RegistroReservacion.AnyAsync(r =>
            r.IdCabana == dto.IdCabana &&
            r.EstadoReserva != EstadoCancelada &&
            r.FechaEntrada < dto.FechaSalida &&
            r.FechaSalida > dto.FechaEntrada);
        if (hayTraslape)
            return Conflict(new { mensaje = "La cabaña ya tiene una reserva en esas fechas." });

        // Calcular total a pagar (noches * precio del tipo de cabaña)
        var noches = Math.Max(1, (dto.FechaSalida.Date - dto.FechaEntrada.Date).Days);
        var total = noches * cabana.TipoCabana.Precio;

        var nueva = new RegistroReservacion
        {
            FechaEntrada = dto.FechaEntrada.Date,
            FechaSalida = dto.FechaSalida.Date,
            CantidadPersonas = dto.CantidadPersonas,
            EstadoReserva = EstadoRecibida,
            TotalPagar = total,
            IdCliente = dto.IdCliente,
            IdCabana = dto.IdCabana,
            IdEmpleado = dto.IdEmpleado ?? EmpleadoDefault
        };

        context.RegistroReservacion.Add(nueva);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservacionPorId), new { id = nueva.Id }, new
        {
            id = nueva.Id,
            mensaje = "Reservación creada con éxito",
            totalPagar = nueva.TotalPagar,
            noches
        });
    }

    // ---------------------------------------------------------------------
    // GET: api/reservaciones?nombre=&telefono=
    // Lista todas las reservaciones, con filtros opcionales por nombre y teléfono.
    // ---------------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservacionDetalleDto>>> ListarReservaciones(
        [FromQuery] string? nombre,
        [FromQuery] string? telefono)
    {
        var query = context.RegistroReservacion
            .Include(r => r.Cliente)!.ThenInclude(c => c!.Persona)
            .Include(r => r.Cabana)!.ThenInclude(c => c!.TipoCabana)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var n = nombre.Trim().ToLower();
            query = query.Where(r =>
                r.Cliente != null && r.Cliente.Persona != null &&
                ((r.Cliente.Persona.Nombres + " " + r.Cliente.Persona.Apellidos).ToLower().Contains(n)));
        }

        if (!string.IsNullOrWhiteSpace(telefono))
        {
            var t = telefono.Trim();
            query = query.Where(r =>
                r.Cliente != null && r.Cliente.Persona != null &&
                r.Cliente.Persona.Telefono.Contains(t));
        }

        var entidades = await query
            .OrderByDescending(r => r.FechaEntrada)
            .ToListAsync();

        var lista = entidades.Select(MapearADetalle).ToList();
        return Ok(lista);
    }

    // ---------------------------------------------------------------------
    // GET: api/reservaciones/{id}
    // Detalle de una reservación específica (pensado para el calendario).
    // ---------------------------------------------------------------------
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservacionDetalleDto>> GetReservacionPorId(int id)
    {
        var reserva = await context.RegistroReservacion
            .Include(r => r.Cliente)!.ThenInclude(c => c!.Persona)
            .Include(r => r.Cabana)!.ThenInclude(c => c!.TipoCabana)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
            return NotFound(new { mensaje = $"No existe la reservación con id {id}." });

        return Ok(MapearADetalle(reserva));
    }

    // ---------------------------------------------------------------------
    // PATCH: api/reservaciones/{id}
    // Actualiza parcialmente los datos de una reservación.
    // Solo se actualizan los campos enviados en el body.
    // ---------------------------------------------------------------------
    [HttpPatch("{id}")]
    public async Task<IActionResult> ActualizarReservacion(int id, [FromBody] ReservacionUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reserva = await context.RegistroReservacion
            .Include(r => r.Cliente)!.ThenInclude(c => c!.Persona)
            .Include(r => r.Cabana)!.ThenInclude(c => c!.TipoCabana)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
            return NotFound(new { mensaje = $"No existe la reservación con id {id}." });

        if (reserva.EstadoReserva == EstadoCancelada)
            return BadRequest(new { mensaje = "No se puede actualizar una reservación cancelada." });

        // Aplicar cambios solo en los campos enviados, conservando los originales en el resto
        var nuevaFechaEntrada = dto.FechaEntrada ?? reserva.FechaEntrada;
        var nuevaFechaSalida  = dto.FechaSalida  ?? reserva.FechaSalida;
        var nuevaCantidad     = dto.CantidadPersonas ?? reserva.CantidadPersonas;
        var nuevaIdCabana     = dto.IdCabana ?? reserva.IdCabana;

        // Validar fechas (no exigimos que la entrada sea futura porque puede que se actualice una ya iniciada)
        var errorFechas = ValidarFechas(nuevaFechaEntrada, nuevaFechaSalida, esCreacion: false);
        if (errorFechas != null) return BadRequest(new { mensaje = errorFechas });

        if (nuevaCantidad <= 0)
            return BadRequest(new { mensaje = "La cantidad de personas debe ser mayor a 0." });

        // Si cambió la cabaña, validar que exista y obtener su tipo
        Cabana? cabanaDestino = reserva.Cabana;
        if (dto.IdCabana.HasValue && dto.IdCabana.Value != reserva.IdCabana)
        {
            cabanaDestino = await context.Cabanas
                .Include(c => c.TipoCabana)
                .FirstOrDefaultAsync(c => c.IdCabana == dto.IdCabana.Value);
            if (cabanaDestino == null)
                return NotFound(new { mensaje = $"La cabaña con id {dto.IdCabana.Value} no existe." });
        }

        if (cabanaDestino?.TipoCabana == null)
            return BadRequest(new { mensaje = "La cabaña no tiene un tipo asignado." });

        // Validar capacidad
        if (nuevaCantidad > cabanaDestino.TipoCabana.Capacidad)
            return BadRequest(new
            {
                mensaje = $"La capacidad de la cabaña es {cabanaDestino.TipoCabana.Capacidad} personas y se solicitaron {nuevaCantidad}."
            });

        // Validar traslape (excluyendo la propia reservación y las canceladas)
        var hayTraslape = await context.RegistroReservacion.AnyAsync(r =>
            r.Id != id &&
            r.IdCabana == nuevaIdCabana &&
            r.EstadoReserva != EstadoCancelada &&
            r.FechaEntrada < nuevaFechaSalida &&
            r.FechaSalida > nuevaFechaEntrada);
        if (hayTraslape)
            return Conflict(new { mensaje = "La cabaña ya tiene una reserva en esas fechas." });

        // Si cambió el teléfono, validar que coincida con la persona del cliente
        if (dto.Telefono != null)
        {
            if (reserva.Cliente?.Persona == null || reserva.Cliente.Persona.Telefono != dto.Telefono)
                return BadRequest(new { mensaje = "El teléfono no coincide con el cliente de la reservación." });
        }

        // Aplicar
        reserva.FechaEntrada = nuevaFechaEntrada.Date;
        reserva.FechaSalida  = nuevaFechaSalida.Date;
        reserva.CantidadPersonas = nuevaCantidad;
        reserva.IdCabana = nuevaIdCabana;

        // Recalcular total con la cabaña/fechas actualizadas
        var noches = Math.Max(1, (reserva.FechaSalida.Date - reserva.FechaEntrada.Date).Days);
        reserva.TotalPagar = noches * cabanaDestino.TipoCabana.Precio;

        await context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Reservación actualizada con éxito",
            reservacion = MapearADetalle(reserva)
        });
    }

    // ---------------------------------------------------------------------
    // PUT: api/reservaciones/{id}/cancelar
    // Cancela una reservación (cambia el estado a 'Cancelada').
    // ---------------------------------------------------------------------
    [HttpPut("{id}/cancelar")]
    public async Task<IActionResult> CancelarReservacion(int id)
    {
        var reserva = await context.RegistroReservacion.FindAsync(id);
        if (reserva == null)
            return NotFound(new { mensaje = $"No existe la reservación con id {id}." });

        if (reserva.EstadoReserva == EstadoCancelada)
            return BadRequest(new { mensaje = "La reservación ya está cancelada." });

        if (reserva.EstadoReserva == EstadoPagada)
            return BadRequest(new { mensaje = "No se puede cancelar una reservación que ya fue pagada." });

        reserva.EstadoReserva = EstadoCancelada;
        await context.SaveChangesAsync();

        return Ok(new { mensaje = "Reservación cancelada con éxito", id = reserva.Id });
    }

    // ---------------------------------------------------------------------
    // DELETE: api/reservaciones/{id}
    // Elimina físicamente una reservación. Solo se permite si está en
    // estado 'Recibida' o 'Cancelada' (no se borran reservas confirmadas/pagadas).
    // ---------------------------------------------------------------------
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarReservacion(int id)
    {
        var reserva = await context.RegistroReservacion.FindAsync(id);
        if (reserva == null)
            return NotFound(new { mensaje = $"No existe la reservación con id {id}." });

        if (reserva.EstadoReserva != EstadoRecibida && reserva.EstadoReserva != EstadoCancelada)
            return BadRequest(new
            {
                mensaje = $"No se puede eliminar una reservación en estado '{reserva.EstadoReserva}'. Cancélala primero."
            });

        context.RegistroReservacion.Remove(reserva);
        await context.SaveChangesAsync();

        return Ok(new { mensaje = "Reservación eliminada con éxito", id });
    }

    // =====================================================================
    // Helpers
    // =====================================================================

    private static string? ValidarFechas(DateTime entrada, DateTime salida, bool esCreacion)
    {
        if (entrada == default || salida == default)
            return "Las fechas son obligatorias.";

        if (salida <= entrada)
            return "La fecha de salida debe ser mayor a la fecha de entrada.";

        if (esCreacion && entrada.Date < DateTime.UtcNow.Date)
            return "La fecha de entrada no puede ser anterior a hoy.";

        return null;
    }

    /// <summary>Mapea una entidad RegistroReservacion al DTO de detalle (en memoria).</summary>
    private static ReservacionDetalleDto MapearADetalle(RegistroReservacion r) => new()
    {
        Id = r.Id,
        FechaEntrada = r.FechaEntrada,
        FechaSalida = r.FechaSalida,
        CantidadPersonas = r.CantidadPersonas,
        EstadoReserva = r.EstadoReserva,
        TotalPagar = r.TotalPagar,
        IdCliente = r.IdCliente,
        NombreCliente = r.Cliente != null && r.Cliente.Persona != null
            ? r.Cliente.Persona.Nombres + " " + r.Cliente.Persona.Apellidos
            : "Desconocido",
        TelefonoCliente = r.Cliente != null && r.Cliente.Persona != null
            ? r.Cliente.Persona.Telefono
            : string.Empty,
        IdCabana = r.IdCabana,
        TipoCabana = r.Cabana != null && r.Cabana.TipoCabana != null
            ? r.Cabana.TipoCabana.Nombre
            : "Desconocido",
        CapacidadCabana = r.Cabana != null && r.Cabana.TipoCabana != null
            ? r.Cabana.TipoCabana.Capacidad
            : 0,
        IdEmpleado = r.IdEmpleado
    };
}
