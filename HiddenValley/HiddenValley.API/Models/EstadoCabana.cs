using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("estadocabana")]
    public class EstadoCabana
    {
        [Key]
        [Column("idestadocabana")]
        public int IdEstadoCabana { get; set; }
        
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        
        public virtual ICollection<Cabana>? Cabanas { get; set; }
        public virtual ICollection<BitacoraEstados>? BitacoraEstadosAnterior { get; set; }
        public virtual ICollection<BitacoraEstados>? BitacoraEstadosNuevo { get; set; }
    }
}