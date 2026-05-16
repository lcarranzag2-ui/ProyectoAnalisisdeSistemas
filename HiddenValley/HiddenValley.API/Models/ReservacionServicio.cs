using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("reservacionservicio")]
    public class ReservacionServicio
    {
        [Key]
        [Column("idreservacion")]
        public int IdReservacion { get; set; }
        [Column("idservicio")]
        public int IdServicio { get; set; }
        [Key]
        [Column("cantidad")]
        public int Cantidad { get; set; }
        public RegistroReservacion? Reservacion { get; set; }
        public Servicio? Servicio { get; set; }
    }
}