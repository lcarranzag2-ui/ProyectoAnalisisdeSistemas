using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiddenValley.API.Models
{
    [Table("cliente")]
    public class Cliente
    {
        [Key]
        [Column("idcliente")]
        public int IdCliente { get; set; }

        [Column("idpersona")]
        public int IdPersona { get; set; }

        [ForeignKey("IdPersona")]
        public virtual Persona? Persona { get; set; }

        public virtual ICollection<RegistroReservacion>? Reservaciones { get; set; }
    }
}
