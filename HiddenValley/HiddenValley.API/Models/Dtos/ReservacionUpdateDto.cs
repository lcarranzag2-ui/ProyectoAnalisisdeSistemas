using System.ComponentModel.DataAnnotations;

namespace HiddenValley.API.Models.Dtos
{
    /// <summary>
    /// DTO para actualizar parcialmente una reservación (PATCH).
    /// Todos los campos son opcionales: solo se actualizan los que se envían.
    /// </summary>
    public class ReservacionUpdateDto
    {
        public DateTime? FechaEntrada { get; set; }

        public DateTime? FechaSalida { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de personas debe ser mayor a 0.")]
        public int? CantidadPersonas { get; set; }

        public int? IdCabana { get; set; }

        [MaxLength(15)]
        public string? Telefono { get; set; }
    }
}
