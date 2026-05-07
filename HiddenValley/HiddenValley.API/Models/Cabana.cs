using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("cabana")]
    public class Cabana
    {
        [Key]
        [Column("idcabana")]
        public int IdCabana { get; set; }

        [Column("idtipocabana")]
        public int IdTipoCabana { get; set; }

        [Column("idestadocabana")]
        public int IdEstadoCabana { get; set; }

        [ForeignKey("IdTipoCabana")]
        public virtual TipoCabana? TipoCabana { get; set; }

        [ForeignKey("IdEstadoCabana")]
        public virtual EstadoCabana? EstadoCabana { get; set; }

        public virtual ICollection<BitacoraEstados>? BitacoraEstados { get; set; }
        public virtual ICollection<RegistroReservacion>? Reservaciones { get; set; }
    }
}
