using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("BitacoraHabitos")]
public class BitacoraHabitos
{
    [Key]
    public int BitacoraID { get; set; } // O Guid, según como lo tengas en SQL
    public Guid HabitoId { get; set; }
    public DateTime Fecha { get; set; }
    public bool Completado { get; set; }
}