using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Tipocabanacontrollers : ControllerBase
{
    private readonly AppDbContext _context;

    public Tipocabanacontrollers(AppDbContext context)
    {
        _context = context;
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var tipos = await _context.TipoCabanas.ToListAsync();

        return Ok(tipos);
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Post(TipoCabana tipoCabana)
    {
        _context.TipoCabanas.Add(tipoCabana);

        await _context.SaveChangesAsync();

        return Ok(tipoCabana);
    }

    // PATCH
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, TipoCabana model)
    {
        var tipo = await _context.TipoCabanas.FindAsync(id);

        if (tipo == null)
            return NotFound();

        tipo.Nombre = model.Nombre;
        tipo.Precio = model.Precio;

        await _context.SaveChangesAsync();

        return Ok(tipo);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tipo = await _context.TipoCabanas.FindAsync(id);

        if (tipo == null)
            return NotFound();

        _context.TipoCabanas.Remove(tipo);

        await _context.SaveChangesAsync();

        return Ok();
    }
}