using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.API.Models.Dtos;

namespace HiddenValley.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PersonasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/personas
        [HttpPost]
        public async Task<ActionResult<Persona>> CrearPersona([FromBody] PersonaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrWhiteSpace(dto.DPI) &&
                await _context.Personas.AnyAsync(p => p.DPI == dto.DPI))
            {
                return Conflict(new { mensaje = "Ya existe una persona con ese DPI." });
            }

            if (!string.IsNullOrWhiteSpace(dto.Gmail) &&
                await _context.Personas.AnyAsync(p => p.Gmail == dto.Gmail))
            {
                return Conflict(new { mensaje = "Ya existe una persona con ese Gmail." });
            }

            var persona = new Persona
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                FechaNacimiento = dto.FechaNacimiento,
                DPI = dto.DPI,
                Telefono = dto.Telefono,
                Gmail = dto.Gmail,
                Direccion = dto.Direccion
            };

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(VerificarDpi), new { dpi = persona.DPI }, persona);
        }

        // GET: api/personas/dpi/{dpi}/existe
        [HttpGet("dpi/{dpi}/existe")]
        public async Task<ActionResult<object>> VerificarDpi(string dpi)
        {
            if (string.IsNullOrWhiteSpace(dpi))
                return BadRequest(new { mensaje = "El DPI es obligatorio." });

            var existe = await _context.Personas.AnyAsync(p => p.DPI == dpi);
            return Ok(new { dpi, existe });
        }

        // PATCH: api/personas/{id}/contacto
        [HttpPatch("{id}/contacto")]
        public async Task<ActionResult<Persona>> ActualizarContacto(int id, [FromBody] PersonaContactoPatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
                return NotFound(new { mensaje = $"No existe persona con Id {id}." });

            if (dto.Gmail != null && dto.Gmail != persona.Gmail &&
                await _context.Personas.AnyAsync(p => p.Gmail == dto.Gmail))
            {
                return Conflict(new { mensaje = "Ya existe una persona con ese Gmail." });
            }

            if (dto.Telefono != null)
                persona.Telefono = dto.Telefono;

            if (dto.Gmail != null)
                persona.Gmail = dto.Gmail;

            if (dto.Direccion != null)
                persona.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();
            return Ok(persona);
        }
    }
}
