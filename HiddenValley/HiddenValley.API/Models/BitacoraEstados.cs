using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("bitacoraestados")]
    public class BitacoraEstados
    {
        [Key]
        [Column("idbitacoraestado")]
        public int IdBitacoraEstado { get; set; }
        
        [Column("idestadoanterior")]
        public int IdEstadoAnterior { get; set; }
        
        [Column("idestadonuevo")]
        public int IdEstadoNuevo { get; set; }
        
        [Column("fechacambio")]
        public DateTime FechaCambio { get; set; }
        
        [Column("idcabana")]
        public int IdCabana { get; set; }
        
        [Column("idempleado")]
        public int IdEmpleado { get; set; }
        
        [ForeignKey("IdEstadoAnterior")]
        public virtual EstadoCabana? EstadoAnterior { get; set; }
        
        [ForeignKey("IdEstadoNuevo")]
        public virtual EstadoCabana? EstadoNuevo { get; set; }
        
        [ForeignKey("IdCabana")]
        public virtual Cabana? Cabana { get; set; }
    }
}