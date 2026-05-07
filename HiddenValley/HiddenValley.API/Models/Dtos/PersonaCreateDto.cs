using System;
using System.ComponentModel.DataAnnotations;

namespace HiddenValley.API.Models.Dtos
{
    public class PersonaCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        public DateOnly? FechaNacimiento { get; set; }

        [MaxLength(20)]
        public string? DPI { get; set; }

        [Required]
        [MaxLength(15)]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string? Gmail { get; set; }

        [Required]
        public string Direccion { get; set; } = string.Empty;
    }
}
