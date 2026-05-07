using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("registroreservacion")]
    public class RegistroReservacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("fechaentrada")]
        public DateTime FechaEntrada { get; set; }

        [Required]
        [Column("fechasalida")]
        public DateTime FechaSalida { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de personas debe ser mayor a 0.")]
        [Column("cantidadpersonas")]
        public int CantidadPersonas { get; set; }

        // Estados permitidos por la BD: 'Recibida','Confirmada','Cancelada','Pagada'
        [Required]
        [MaxLength(20)]
        [Column("estadoreserva")]
        public string EstadoReserva { get; set; } = "Recibida";

        [Required]
        [Range(0, double.MaxValue)]
        [Column("totalpagar")]
        public decimal TotalPagar { get; set; }

        [Required]
        [Column("idcliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("idcabana")]
        public int IdCabana { get; set; }

        [Required]
        [Column("idempleado")]
        public int IdEmpleado { get; set; }

        // Navegaciones (no afectan el mapeo a la BD que ya existe)
        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdCabana")]
        public virtual Cabana? Cabana { get; set; }
    }
}
