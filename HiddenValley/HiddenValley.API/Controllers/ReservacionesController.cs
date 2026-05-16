using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservacionesController(IReservacionService reservacionService) : ControllerBase
{
    // GET api/reservaciones?search=&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await reservacionService.GetPagedAsync(search, page, pageSize));

    // GET api/reservaciones/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var res = await reservacionService.GetByIdAsync(id);
        return res is null
            ? NotFound(new { mensaje = $"No existe la reservación con id {id}." })
            : Ok(res);
    }

    // POST api/reservaciones
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservacionCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var res = await reservacionService.CreateAsync(dto);
        return res.Success
            ? CreatedAtAction(nameof(GetById), new { id = res.Id }, new
              {
                  id         = res.Id,
                  mensaje    = res.Message,
                  totalPagar = res.Total,
                  noches     = res.Noches
              })
            : BadRequest(new { mensaje = res.Message });
    }

    // PATCH api/reservaciones/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] ReservacionPatchDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var res = await reservacionService.PatchAsync(id, dto);
        return res.Success
            ? Ok(new { mensaje = res.Message, reservacion = res.Data })
            : BadRequest(new { mensaje = res.Message });
    }
}
