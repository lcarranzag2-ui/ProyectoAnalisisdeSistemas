using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController(IClienteService clienteService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await clienteService.GetPagedAsync(search, page, pageSize));
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string filtro)
    {
        var cliente = await clienteService.GetByIdOrFiltroAsync(filtro);
        return cliente != null ? Ok(cliente) : NotFound("Cliente no encontrado.");
    }

    [HttpGet("{id}/historial")]
    public async Task<IActionResult> GetHistorial(int id)
    {
        return Ok(await clienteService.GetHistorialAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClienteCreateDTO dto)
    {
        var result = await clienteService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] ClientePatchDTO dto)
    {
        var result = await clienteService.PatchAsync(id, dto);
        return result.Success ? Ok(new { mensaje = result.Message }) : NotFound(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await clienteService.DeleteAsync(id);
        return result.Success ? Ok(new { mensaje = result.Message }) : BadRequest(result.Message);
    }
}