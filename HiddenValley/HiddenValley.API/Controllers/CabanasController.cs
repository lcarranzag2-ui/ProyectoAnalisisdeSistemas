using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Services;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CabanasController(ICabanaService cabanaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var response = await cabanaService.GetPagedAsync(search, page, pageSize);
        return Ok(response);
    }

    [HttpGet("disponibilidad")]
    public async Task<IActionResult> VerificarDisponibilidad([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        if (fechaInicio >= fechaFin)
            return BadRequest("La fecha de inicio debe ser menor a la fecha de fin");

        var result = await cabanaService.GetDisponibilidadAsync(fechaInicio, fechaFin);
        return Ok(result);
    }

    [HttpPut("cambiar-estado")]
    public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoRequest request)
    {
        var result = await cabanaService.CambiarEstadoAsync(request);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarCabana([FromBody] RegistrarCabanaRequest request)
    {
        var result = await cabanaService.RegistrarCabanaAsync(request);
        return result.Success ? Ok(new { id = result.Id, mensaje = result.Message }) : NotFound(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarCabana(int id)
    {
        var result = await cabanaService.EliminarCabanaAsync(id);
        return result.Success ? Ok(new { mensaje = result.Message }) : BadRequest(result.Message);
    }
}