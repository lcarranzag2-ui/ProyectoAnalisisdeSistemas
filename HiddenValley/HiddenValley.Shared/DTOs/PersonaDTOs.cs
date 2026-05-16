using System;

namespace HiddenValley.Shared.DTOs;

public class PersonaCreateDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateOnly? FechaNacimiento { get; set; }
    public string DPI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Gmail { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string TipoPersona { get; set; } = "Cliente";
}

public class PersonaPatchDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string DPI { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Gmail { get; set; }
    public string? Direccion { get; set; }
}

public class PersonaResponseDto
{
    public int IdPersona { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string DPI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Gmail { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string TipoPersona { get; set; } = string.Empty;
}
public class PagedResponsePersona<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalRecords { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}