using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;


namespace HiddenValley.API.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController(ApplicationDbContext context) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] ClienteCreateDTO dto)
        {
            var persona = await context.Personas.FindAsync(dto.IdPersona);
            if (persona == null) return NotFound("La persona no existe.");

            var existeCliente = await context.Clientes.AnyAsync(c => c.IdPersona == dto.IdPersona);
            if (existeCliente) return BadRequest("Esta persona ya es cliente.");

            var nuevoCliente = new Cliente { IdPersona = dto.IdPersona };
            context.Clientes.Add(nuevoCliente);
            await context.SaveChangesAsync();

            return Ok(new { idCliente = nuevoCliente.IdCliente, mensaje = "Cliente creado con éxito" });
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<ClienteDetalleDTO>> Buscar([FromQuery] string filtro)
        {
            var cliente = await context.Clientes
                .Include(c => c.Persona)
                .Where(c => c.Persona!.DPI == filtro || c.Persona.Telefono == filtro)
                .Select(c => new ClienteDetalleDTO(
                    c.IdCliente,
                    $"{c.Persona!.Nombres} {c.Persona.Apellidos}",
                    c.Persona.DPI ?? "N/A",
                    c.Persona.Telefono,
                    c.Persona.Gmail ?? "N/A"
                )).FirstOrDefaultAsync();

            if (cliente == null) return NotFound("Cliente no encontrado.");
            return Ok(cliente);
        }

        [HttpGet("{idCliente}/historial")]
        public async Task<ActionResult<IEnumerable<HistorialReservaDTO>>> GetHistorial(int idCliente)
        {
            var historial = await context.RegistroReservacion
                .Where(r => r.IdCliente == idCliente)
                .OrderByDescending(r => r.FechaEntrada)
                .Select(r => new HistorialReservaDTO(
                    r.Id, r.FechaEntrada, r.FechaSalida, r.EstadoReserva, r.TotalPagar
                )).ToListAsync();

            return Ok(historial);
        }
}
