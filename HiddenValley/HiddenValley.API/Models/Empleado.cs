using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace HiddenValley.API.Models;

[Table("empleado")]
public class Empleado
{
    [Key]
    public int IdEmpleado { get; set; }

    public int IdPersona { get; set; }

    public int IdPuestoTrabajo { get; set; }

    [ForeignKey(nameof(IdPersona))]
    public Persona? Persona { get; set; }

    [ForeignKey(nameof(IdPuestoTrabajo))]
    public PuestoTrabajo? PuestoTrabajo { get; set; }

}

// DTO entrada
public class EmpleadoRequest
{
    [Required(ErrorMessage = "IdPersona es requerido.")]
    public int IdPersona { get; set; }

    [Required(ErrorMessage = "IdPuestoTrabajo es requerido.")]
    public int IdPuestoTrabajo { get; set; }
}

