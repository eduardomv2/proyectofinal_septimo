using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("FrecuenciaHabitos")]
public class FrecuenciaHabitos
{
    [Key]
    public int FrecuenciaId { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Column("MetaCantidad")]
    public int? MetaCantidad { get; set; }

    // Campos de auditoría
    [Required]
    public Guid UsuarioCreacion { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; }

    public Guid? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    [Required]
    public bool Estatus { get; set; }
}