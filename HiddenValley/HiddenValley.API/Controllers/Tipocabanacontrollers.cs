using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoCabanaController(ITipoCabanaService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var tipo = await service.GetByIdAsync(id);
        return tipo == null ? NotFound() : Ok(tipo);
    }

    [HttpPost]
    public async Task<IActionResult> Post(TipoCabanaCreateDTO dto)
    {
        var creado = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = creado.IdTipoCabana }, creado);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, TipoCabanaCreateDTO dto)
    {
        var resultado = await service.UpdateAsync(id, dto);
        return resultado.Success ? Ok(resultado.Message) : BadRequest(resultado.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resultado = await service.DeleteAsync(id);
        return resultado.Success ? NoContent() : BadRequest(resultado.Message);
    }
}