using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("ideas_diario")]
public class IdeasDiario
{
    [Key]
    [Column("idea_id")]
    public int IdeaId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Required]
    [Column("titulo")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Column("contenido")]
    public string? Contenido { get; set; }

    [Column("fecha_captura")]
    public DateTime? FechaCaptura { get; set; }

    [Column("es_favorito")]
    public bool EsFavorito { get; set; }

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