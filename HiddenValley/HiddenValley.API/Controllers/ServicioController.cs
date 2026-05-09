using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly IServicioService _servicioService;

        public ServiciosController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? nombre = null, 
            [FromQuery] int? id = null)
        {
            var resultados = await _servicioService.GetPagedAsync(pageNumber, pageSize, nombre, id);
            return Ok(resultados);
        }


        [HttpPost]
        public async Task<ActionResult<ServicioCreateDto>> Create(ServicioCreateDto servicioDto)
        {
            var creado = await _servicioService.CreateServicioAsync(servicioDto);
            return Ok(creado);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateServicioDto servicioDto)
        {
            var actualizado = await _servicioService.PatchAsync(id, servicioDto);
            if (!actualizado) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _servicioService.DeleteServicioAsync(id);
            if (!resultado) return NotFound();
            return NoContent();
        }
    }
}