using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoServicioController : ControllerBase
    {
        private readonly ITipoServicioService _tipoServicioService;

        public TipoServicioController(ITipoServicioService tipoServicioService)
        {
            _tipoServicioService = tipoServicioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _tipoServicioService.GetAllAsync();
            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _tipoServicioService.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            return Ok(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoServicioCreateDto dto)
        {
            var creado = await _tipoServicioService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdTipoServicio }, creado);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] TipoServicioCreateDto dto)
        {
            var resultado = await _tipoServicioService.UpdateAsync(id, dto);
            return resultado.Success ? Ok(resultado.Message) : BadRequest(resultado.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _tipoServicioService.DeleteAsync(id);
            return resultado.Success ? NoContent() : BadRequest(resultado.Message);
        }
    }
}
