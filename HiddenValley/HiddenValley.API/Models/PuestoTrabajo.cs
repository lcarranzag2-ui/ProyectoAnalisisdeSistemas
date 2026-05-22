using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models;

[Table("puestotrabajo")]
public class PuestoTrabajo
{
    [Key]
    [Column("idpuestotrabajo")]
    public int IdPuestoTrabajo { get; set; }

    [Required, MaxLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }
}
