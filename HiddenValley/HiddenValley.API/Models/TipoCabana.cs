using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("tipocabana")]
    public class TipoCabana
    {
        [Key]
        [Column("idtipocabana")]
        public int IdTipoCabana { get; set; }
        
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        
        [Column("capacidad")]
        public int Capacidad { get; set; }
        
        [Column("precio")]
        public decimal Precio { get; set; }
        
        public virtual ICollection<Cabana>? Cabanas { get; set; }
    }
}