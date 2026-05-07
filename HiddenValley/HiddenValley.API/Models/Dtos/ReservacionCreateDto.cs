using System.ComponentModel.DataAnnotations;

namespace HiddenValley.API.Models.Dtos
{
    /// <summary>
    /// DTO para crear una reservación.
    /// El issue PROYECT-60 indica que debe recibir: Cliente, Teléfono, Fechas,
    /// Cantidad de Personas y Cabaña. El IdEmpleado es opcional (default 1).
    /// </summary>
    public class ReservacionCreateDto
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int IdCliente { get; set; }

        /// <summary>
        /// Teléfono del cliente. Se valida que coincida con el de la persona
        /// asociada al cliente (criterio del issue).
        /// </summary>
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(15)]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cabaña es obligatoria.")]
        public int IdCabana { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria.")]
        public DateTime FechaEntrada { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria.")]
        public DateTime FechaSalida { get; set; }

        [Required(ErrorMessage = "La cantidad de personas es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de personas debe ser mayor a 0.")]
        public int CantidadPersonas { get; set; }

        /// <summary>
        /// Empleado que registra la reservación. Si no se envía, se usa el default (1).
        /// </summary>
        public int? IdEmpleado { get; set; }
    }
}
