using System;

namespace HiddenValley.Shared.DTOs;

public class EmpleadoCreateDTO
{
    public int IdPersona { get; set; }
    public int IdPuestoTrabajo { get; set; }
}

public class EmpleadoPatchDTO
{
    public int? IdPuestoTrabajo { get; set; }
    public string? Telefono { get; set; }
    public string? Gmail { get; set; }
}

public class EmpleadoResponseDTO
{
    public int IdEmpleado { get; set; }
    public int IdPersona { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public int IdPuestoTrabajo { get; set; }
    public string NombrePuesto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}