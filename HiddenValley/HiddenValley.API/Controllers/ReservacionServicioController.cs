using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservacionServicioController : ControllerBase
    {
    private readonly IReservacionServicio _service;
    public ReservacionServicioController(IReservacionServicio service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1, [FromQuery] int size = 10, 
        [FromQuery] string? buscar = null, [FromQuery] int? idServicio = null, 
        [FromQuery] DateTime? fecha = null)
    {
        return Ok(await _service.GetPagedAsync(page, size, buscar, idServicio, fecha));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReservacionServicioCreateDto dto) =>
        await _service.CreateAsync(dto) ? Ok() : BadRequest();

    [HttpPut("{idRes}/{idSer}")]
    public async Task<IActionResult> Put(int idRes, int idSer, [FromBody] ReservacionServicioUpdateDto dto) =>
        await _service.UpdateAsync(idRes, idSer, dto) ? Ok() : NotFound();

    [HttpDelete("{idRes}/{idSer}")]
    public async Task<IActionResult> Delete(int idRes, int idSer) =>
        await _service.DeleteAsync(idRes, idSer) ? Ok() : NotFound();
}
    
}