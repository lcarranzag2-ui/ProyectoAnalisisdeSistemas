using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("persona")]
    public class Persona
    {
        [Key]
        [Column("idpersona")]
        public int IdPersona { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Column("fechanacimiento")]
        public DateOnly? FechaNacimiento { get; set; }

        [Column("fecharegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        [Column("dpi")]
        public string? DPI { get; set; }

        [Required]
        [MaxLength(15)]
        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("gmail")]
        public string? Gmail { get; set; }

        [Required]
        [Column("direccion")]
        public string Direccion { get; set; } = string.Empty;
    }
}
