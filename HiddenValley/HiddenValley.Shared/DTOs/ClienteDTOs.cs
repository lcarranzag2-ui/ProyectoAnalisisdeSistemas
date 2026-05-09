using System;

namespace HiddenValley.Shared.DTOs;

public class ClienteCreateDTO
{
    public int IdPersona { get; set; }
}

public class ClientePatchDTO
{
    public string? Gmail { get; set; }
    public string? Telefono { get; set; }
}

public class ClienteDetalleDTO
{
    public int IdCliente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string DPI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class HistorialReservaDTO
{
    public int Id { get; set; }
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
}