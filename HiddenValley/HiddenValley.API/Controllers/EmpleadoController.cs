using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Services;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadoController(IEmpleadoService empleadoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await empleadoService.GetPagedAsync(search, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmpleadoCreateDTO dto)
    {
        var result = await empleadoService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] EmpleadoPatchDTO dto)
    {
        var result = await empleadoService.PatchAsync(id, dto);
        return result.Success ? Ok(new { message = result.Message }) : BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await empleadoService.DeleteAsync(id);
        return result.Success ? Ok(new { message = result.Message }) : BadRequest(result.Message);
    }
}