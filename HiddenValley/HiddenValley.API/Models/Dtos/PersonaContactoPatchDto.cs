using System.ComponentModel.DataAnnotations;

namespace HiddenValley.API.Models.Dtos
{
    public class PersonaContactoPatchDto
    {
        [MaxLength(15)]
        public string? Telefono { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Gmail { get; set; }

        public string? Direccion { get; set; }
    }
}
