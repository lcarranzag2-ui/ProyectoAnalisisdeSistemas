namespace HiddenValley.Domain.Entities;

public class Cabana
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public int NumeroHabitaciones { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public string EstadoDisponibilidad { get; set; } = "Disponible";
    public DateTime FechaRegistro { get; set; }
    public bool Activa { get; set; } = true;
}
