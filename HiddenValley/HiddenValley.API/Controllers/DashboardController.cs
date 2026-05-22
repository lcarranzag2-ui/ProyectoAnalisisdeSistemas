using Microsoft.AspNetCore.Mvc;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    // GET api/dashboard
    // retorna el resumen del dia: cabanas ocupadas, disponibles, personas esperadas y proximas reservas
    [HttpGet]
    public async Task<IActionResult> GetResumen()
    {
        var resumen = await dashboardService.GetResumenAsync();
        return Ok(resumen);
    }
}
