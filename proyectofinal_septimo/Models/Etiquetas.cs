using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("etiquetas")]
public class Etiquetas
{
    [Key]
    [Column("etiqueta_id")]
    public int EtiquetaId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Required]
    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Column("color")]
    [StringLength(7)]
    public string? Color { get; set; }

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

    // Propiedades de navegación
    [ForeignKey("UsuarioId")]
    public virtual UsuariosReplica? Usuario { get; set; }
}