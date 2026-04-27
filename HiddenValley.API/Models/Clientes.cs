using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("Persona")]
    public class Persona
    {
        [Key]
        public int IdPersona { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string? DPI { get; set; }
        public string Telefono { get; set; } = null!;
        public string? Gmail { get; set; }
        public string Direccion { get; set; } = null!;
    }

    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }
        public int IdPersona { get; set; }
        
        [ForeignKey("IdPersona")]
        public Persona Persona { get; set; } = null!;
    }

    public class RegistroReservacion
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public string EstadoReserva { get; set; } = null!;
        public decimal TotalPagar { get; set; }
        public int IdCliente { get; set; }
        public int IdCabana { get; set; }
        public int IdEmpleado { get; set; }
        public int CantidadPersonas { get; set; }
    }
}