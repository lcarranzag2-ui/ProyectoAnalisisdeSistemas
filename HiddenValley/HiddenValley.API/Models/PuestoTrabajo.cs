using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models;

[Table("PuestoTrabajo")]
public class PuestoTrabajo
{
    [Key]
    public int IdPuestoTrabajo { get; set; }

    [Required, MaxLength(50)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }
}
