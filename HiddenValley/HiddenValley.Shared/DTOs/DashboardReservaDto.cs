namespace HiddenValley.Shared.DTOs;

// representa cada reserva que se va a mostrar en el panel de proximas reservaciones
public class DashboardReservaDto
{
    public int Id { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string TipoCabana { get; set; } = string.Empty;
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public int CantidadPersonas { get; set; }
    public string EstadoReserva { get; set; } = string.Empty;
}
