using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly IServicioService _servicioService;
        private readonly ApplicationDbContext _context; 
        public ServiciosController(IServicioService servicioService, ApplicationDbContext context)
        {
            _servicioService = servicioService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? nombre = null, 
            [FromQuery] int? id = null)
        {
            var resultados = await _servicioService.GetPagedAsync(pageNumber, pageSize, nombre, id);
            
            var query = _context.Servicio.AsQueryable();
            if (id.HasValue) query = query.Where(s => s.IdServicio == id.Value);
            else if (!string.IsNullOrWhiteSpace(nombre)) query = query.Where(s => s.Nombre != null && s.Nombre.ToLower().Contains(nombre.ToLower()));
            
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            Response.Headers.Add("X-Total-Records", totalRecords.ToString());
            Response.Headers.Add("X-Total-Pages", totalPages.ToString());
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Records, X-Total-Pages"); // Permite que Blazor los lea

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