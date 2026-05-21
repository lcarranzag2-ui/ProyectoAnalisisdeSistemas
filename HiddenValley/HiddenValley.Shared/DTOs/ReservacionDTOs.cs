using System.ComponentModel.DataAnnotations;

namespace HiddenValley.Shared.DTOs;

public class ReservacionCreateDto
{
    [Required(ErrorMessage = "El cliente es obligatorio.")]
    public int IdCliente { get; set; }

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

    public int? IdEmpleado { get; set; }
}

public class ReservacionPatchDto
{
    public DateTime? FechaEntrada { get; set; }
    public DateTime? FechaSalida { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad de personas debe ser mayor a 0.")]
    public int? CantidadPersonas { get; set; }

    public int? IdCabana { get; set; }

    [MaxLength(15)]
    public string? Telefono { get; set; }

    // Permite cambiar el estado: Recibida, Confirmada, Cancelada, Pagada
    public string? EstadoReserva { get; set; }
}

public class ReservacionDetalleDto
{
    public int Id { get; set; }
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public int CantidadPersonas { get; set; }
    public string EstadoReserva { get; set; } = string.Empty;
    public decimal TotalPagar { get; set; }
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public int IdCabana { get; set; }
    public string TipoCabana { get; set; } = string.Empty;
    public int CapacidadCabana { get; set; }
    public int IdEmpleado { get; set; }
}
