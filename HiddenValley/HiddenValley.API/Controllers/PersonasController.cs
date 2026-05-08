using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonasController(IPersonaService personaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(await personaService.GetPagedAsync(search, page, pageSize));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PersonaCreateDto dto)
    {
        var res = await personaService.CreateAsync(dto);
        return res.Success ? Ok(res) : BadRequest(new { mensaje = res.Message });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PersonaPatchDto dto)
    {
        var res = await personaService.PatchAsync(id, dto);
        return res.Success ? Ok(new { mensaje = res.Message }) : BadRequest(new { mensaje = res.Message });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await personaService.DeleteAsync(id);
        return res.Success ? Ok(new { mensaje = res.Message }) : BadRequest(new { mensaje = res.Message });
    }

    [HttpGet("dpi/{dpi}/existe")]
    public async Task<IActionResult> VerificarDpi(string dpi) 
        => Ok(new { dpi, existe = await personaService.ExisteDpiAsync(dpi) });
}