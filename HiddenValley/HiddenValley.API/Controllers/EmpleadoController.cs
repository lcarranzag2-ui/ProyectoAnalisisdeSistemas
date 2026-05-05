using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadoController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EmpleadoController(ApplicationDbContext db) => _db = db;

    /// <summary>GET api/empleado — Lista todos los empleados</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var lista = await _db.Empleados
            .Include(e => e.Persona)
            .Include(e => e.PuestoTrabajo)
            .Select(e => new EmpleadoResponse
            {
                IdEmpleado     = e.IdEmpleado,
                IdPersona      = e.IdPersona,
                NombreCompleto = e.Persona!.Nombres + " " + e.Persona.Apellidos,
                IdPuestoTrabajo = e.IdPuestoTrabajo,
                NombrePuesto   = e.PuestoTrabajo!.Nombre
            })
            .ToListAsync();

        return Ok(lista);
    }

    /// <summary>POST api/empleado — Registra un nuevo empleado</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmpleadoRequest request)
    {
        // Validar que el puesto exista (criterio obligatorio)
        var puestoExiste = await _db.PuestosTrabajo
            .AnyAsync(p => p.IdPuestoTrabajo == request.IdPuestoTrabajo);

        if (!puestoExiste)
            return BadRequest(new { message = "El puesto de trabajo especificado no existe. Seleccione un puesto válido." });

        // Validar que la persona exista
        var personaExiste = await _db.Personas
            .AnyAsync(p => p.IdPersona == request.IdPersona);

        if (!personaExiste)
            return BadRequest(new { message = "La persona especificada no existe." });

        // Validar que la persona no esté ya registrada como empleado
        var yaRegistrado = await _db.Empleados
            .AnyAsync(e => e.IdPersona == request.IdPersona);

        if (yaRegistrado)
            return Conflict(new { message = "Esta persona ya está registrada como empleado." });

        var empleado = new Empleado
        {
            IdPersona       = request.IdPersona,
            IdPuestoTrabajo = request.IdPuestoTrabajo
        };

        _db.Empleados.Add(empleado);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = empleado.IdEmpleado },
            new { empleado.IdEmpleado, message = "Empleado registrado correctamente." });
    }
}
