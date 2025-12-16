using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("RecordatoriosTareas")]
public class RecordatoriosTareas
{
    [Key]
    public int RecordatorioId { get; set; }

    [Required]
    public int TareaId { get; set; }

    [Required]
    public DateTime FechaRecordatorio { get; set; }

    [StringLength(500)]
    public string? Mensaje { get; set; }

    [Required]
    public bool Enviado { get; set; }

    // Campos de auditoría
    [Required]
    public Guid UsuarioCreacion { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; }

    public Guid? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    [Required]
    public bool Estatus { get; set; }

    // Propiedades de navegación
    [ForeignKey("TareaId")]
    public virtual Tareas? Tarea { get; set; }
}