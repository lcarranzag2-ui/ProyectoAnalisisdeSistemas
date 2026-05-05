using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models;

[Table("Persona")]
public class Persona
{
    [Key]
    public int IdPersona { get; set; }

    [Required, MaxLength(100)]
    public string Nombres { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Apellidos { get; set; } = null!;

    public DateTime? FechaNacimiento { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [MaxLength(20)]
    public string? DPI { get; set; }

    [Required, MaxLength(15)]
    public string Telefono { get; set; } = null!;

    [MaxLength(100)]
    public string? Gmail { get; set; }

    [Required]
    public string Direccion { get; set; } = null!;
}
