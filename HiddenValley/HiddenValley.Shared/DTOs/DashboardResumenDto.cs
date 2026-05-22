namespace HiddenValley.Shared.DTOs;

// dto principal que agrupa todas las metricas operativas del dia para la pantalla de inicio
public class DashboardResumenDto
{
    // cuantas cabanas tienen reserva activa hoy
    public int CabanasOcupadasHoy { get; set; }

    // cuantas cabanas tienen estado "Disponible" y no estan ocupadas hoy
    public int CabanasDisponibles { get; set; }

    // suma de cantidadpersonas de las reservas cuyo check-in es hoy
    public int PersonasEsperadasHoy { get; set; }

    // las proximas 5 reservaciones ordenadas por fecha de entrada
    public List<DashboardReservaDto> ProximasReservas { get; set; } = new();
}
