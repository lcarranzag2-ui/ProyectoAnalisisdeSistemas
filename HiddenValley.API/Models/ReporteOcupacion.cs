using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("registroreservacion")] 
    public class ReporteOcupacion
    {
        [Key]
        public int Id { get; set; } 
        [DataType(DataType.Date)]
        public DateTime FechaEntrada { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaSalida { get; set; }
        public int CantidadPersonas { get; set; }
        public string EstadoReserva { get; set; } = string.Empty;
        public decimal TotalPagar { get; set; }
        public int IdCliente { get; set; }
        public int IdCabana { get; set; }
        public int IdEmpleado { get; set; }
    }
}
