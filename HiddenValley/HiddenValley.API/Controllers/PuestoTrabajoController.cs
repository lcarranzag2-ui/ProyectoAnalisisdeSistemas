using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PuestoTrabajoController(IPuestoTrabajoService puestoService) : ControllerBase
{
    // GET api/puestotrabajo?search=admin&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await puestoService.GetPagedAsync(search, page, pageSize));

    // POST api/puestotrabajo
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PuestoTrabajoCreateDto dto)
    {
        var res = await puestoService.CreateAsync(dto);
        return res.Success
            ? Ok(new { mensaje = res.Message, id = res.Id })
            : BadRequest(new { mensaje = res.Message });
    }

    // PATCH api/puestotrabajo/1
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PuestotrabajoPatchDto dto)
    {
        var res = await puestoService.PatchAsync(id, dto);
        return res.Success
            ? Ok(new { mensaje = res.Message })
            : BadRequest(new { mensaje = res.Message });
    }

    // DELETE api/puestotrabajo/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await puestoService.DeleteAsync(id);
        return res.Success
            ? Ok(new { mensaje = res.Message })
            : BadRequest(new { mensaje = res.Message });
    }
}
