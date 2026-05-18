using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadoCabanaController(IEstadoCabanaService estadoCabanaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var estados = await estadoCabanaService.GetAllAsync();
        return Ok(estados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var estado = await estadoCabanaService.GetByIdAsync(id);
        if (estado == null) return NotFound("Estado no encontrado.");
        return Ok(estado);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EstadoCabanaDto dto)
    {
        var (success, message, id) = await estadoCabanaService.CreateAsync(dto);
        if (!success) return BadRequest(message);
        
        return CreatedAtAction(nameof(GetById), new { id }, dto);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] EstadoCabanaDto dto)
    {
        var (success, message) = await estadoCabanaService.PatchAsync(id, dto);
        if (!success) return NotFound(message);
        
        return Ok(message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await estadoCabanaService.DeleteAsync(id);
        if (!success) return BadRequest(message);
        
        return Ok(message);
    }
}