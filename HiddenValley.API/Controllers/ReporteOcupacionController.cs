using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models; 
namespace HiddenValley.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcupacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OcupacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroReservacion>>> Get()
        {
            return await _context.RegistroReservacion.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistroReservacion>> GetId(int id)
        {
            var reserva = await _context.RegistroReservacion.FindAsync(id);
            if (reserva == null) return NotFound("Reserva no encontrada.");
            return reserva;
        }

        [HttpPost]
        public async Task<ActionResult<RegistroReservacion>> Post(RegistroReservacion reserva)
        {
            reserva.FechaEntrada = DateTime.SpecifyKind(reserva.FechaEntrada, DateTimeKind.Utc);
            reserva.FechaSalida = DateTime.SpecifyKind(reserva.FechaSalida, DateTimeKind.Utc);

            _context.RegistroReservacion.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { id = reserva.Id }, reserva);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] string nuevoEstado)
        {
            var reserva = await _context.RegistroReservacion.FindAsync(id);
            
            if (reserva == null) 
            {
                return NotFound($"No existe la reserva con ID {id}");
            }
            string estadoLimpio = nuevoEstado.Trim();
            
            estadoLimpio = char.ToUpper(estadoLimpio[0]) + estadoLimpio.Substring(1).ToLower();

            try 
            {
                reserva.EstadoReserva = estadoLimpio;
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { 
                    Error = "La base de datos rechazó el estado.",
                    Sugerencia = "Revisa si el estado debe ser 'Pagada', 'Pendiente' o 'Cancelada' (respeta mayúsculas)."
                });
            }
        }

        [HttpGet("reporte")]
        public async Task<IActionResult> GetReporte([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            var desdeUtc = DateTime.SpecifyKind(desde, DateTimeKind.Utc);
            var hastaUtc = DateTime.SpecifyKind(hasta, DateTimeKind.Utc);

            var reservas = await _context.RegistroReservacion
                .Where(r => r.FechaEntrada <= hastaUtc && r.FechaSalida >= desdeUtc)
                .ToListAsync();

            return Ok(new
            {
                TotalReservaciones = reservas.Count, 
                SumaPersonasHospedadas = reservas.Sum(r => r.CantidadPersonas), 
                Data = reservas
            });
        }
    }
}