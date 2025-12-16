using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("archivos_adjuntos")]
public class ArchivosAdjuntos
{
    [Key]
    [Column("archivo_id")]
    public int ArchivoId { get; set; }

    [Column("entrada_id")]
    public int? EntradaId { get; set; }

    [Column("nota_id")]
    public int? NotaId { get; set; }

    [Required]
    [Column("nombre_archivo")]
    [StringLength(255)]
    public string NombreArchivo { get; set; } = string.Empty;

    [Required]
    [Column("ruta_archivo")]
    [StringLength(500)]
    public string RutaArchivo { get; set; } = string.Empty;

    [Column("tipo_mime")]
    [StringLength(100)]
    public string? TipoMime { get; set; }

    [Column("tamano_bytes")]
    public long? TamanoBytes { get; set; }

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
    //[ForeignKey("EntradaId")]
    //public virtual DiarioEntradas? Entrada { get; set; }

    [ForeignKey("NotaId")]
    public virtual Notas? Nota { get; set; }
}