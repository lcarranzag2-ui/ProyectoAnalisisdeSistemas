using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("servicio")]
    public class Servicio
    {
        [Key]
        [Column("idservicio")]
        public int IdServicio {get; set;}
        [Column("nombre")]
        public string? Nombre {get; set;}
        [Column("descripcion")]
        public string? Descripcion {get; set;}
        [Column("precio")]
        public decimal Precio {get; set;}
    }
}