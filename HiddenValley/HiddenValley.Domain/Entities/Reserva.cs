namespace HiddenValley.Domain.Entities;

public class Reserva
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int CabanaId { get; set; }
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public int NumeroHuespedes { get; set; }
    public decimal MontoTotal { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime FechaCreacion { get; set; }
    public string? Observaciones { get; set; }
}
