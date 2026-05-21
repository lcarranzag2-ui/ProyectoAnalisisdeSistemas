using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("tiposervicio")]
    public class TipoServicio
    {
        [Key]
        [Column("idtiposervicio")]
        public int IdTipoServicio { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        // navegacion hacia los servicios que usan este tipo
        public virtual ICollection<Servicio>? Servicios { get; set; }
    }
}
