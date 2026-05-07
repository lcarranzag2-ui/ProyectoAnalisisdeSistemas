namespace HiddenValley.API.Models.Dtos
{
    /// <summary>
    /// DTO con la información completa de una reservación, pensado
    /// para el listado, detalle por id y la vista de calendario.
    /// </summary>
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
}
