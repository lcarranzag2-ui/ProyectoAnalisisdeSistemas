namespace HiddenValley.Shared.DTOs;

public class PuestoTrabajoCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class PuestotrabajoPatchDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
}

public class PuestoTrabajoResponseDto
{
    public int IdPuestoTrabajo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
