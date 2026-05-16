namespace HiddenValley.Shared.DTOs
{
    public class ReservacionServicioCreateDto
    {
        public int IdReservacion { get; set; }
        public int IdServicio { get; set; }
        public int Cantidad { get; set; }
    }

    public class ReservacionServicioUpdateDto
    {
        public int Cantidad { get; set; }
    }

    public class ReservacionServicioReadDto
    {
        public int IdReservacion { get; set; }
        public int IdServicio { get; set; }
        public string? NombreServicio { get; set; }
        public string? NombreCliente { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaEntrada { get; set; }
        public string? EstadoReserva { get; set; }
    }

    public class PagedResultReservacionServicio<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
    }
}