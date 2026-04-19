namespace HiddenValley.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string ContrasenaHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente";
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; } = true;
}
