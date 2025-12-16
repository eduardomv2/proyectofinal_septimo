using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("tipos_contenido")]
public class TiposContenido
{
    [Key]
    [Column("tipo_id")]
    public int TipoId { get; set; }

    [Required]
    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Column("descripcion")]
    [StringLength(500)]
    public string? Descripcion { get; set; }

    // Campos de auditoría
    [Required]
    [Column("usuario_creacion")]
    public Guid UsuarioCreacion { get; set; }

    [Required]
    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; }

    [Column("usuario_modificacion")]
    public Guid? UsuarioModificacion { get; set; }

    [Column("fecha_modificacion")]
    public DateTime? FechaModificacion { get; set; }

    [Required]
    [Column("estatus")]
    public bool Estatus { get; set; }
}