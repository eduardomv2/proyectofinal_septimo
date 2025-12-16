using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("mapa_etiquetas")]
public class MapaEtiquetas
{
    [Key]
    [Column("mapa_id")]
    public int MapaId { get; set; }

    [Required]
    [Column("etiqueta_id")]
    public int EtiquetaId { get; set; }

    [Column("entrada_id")]
    public int? EntradaId { get; set; }

    [Column("nota_id")]
    public int? NotaId { get; set; }

    [Column("idea_id")]
    public int? IdeaId { get; set; }

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
    [ForeignKey("EtiquetaId")]
    public virtual Etiquetas? Etiqueta { get; set; }

    //[ForeignKey("EntradaId")]
    //public virtual DiarioEntradas? Entrada { get; set; }

    [ForeignKey("NotaId")]
    public virtual Notas? Nota { get; set; }

    [ForeignKey("IdeaId")]
    public virtual IdeasDiario? Idea { get; set; }
}