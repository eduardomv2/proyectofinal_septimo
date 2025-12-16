using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("Subtareas")]
public class Subtareas
{
    [Key]
    public int SubtareaId { get; set; }

    [Required]
    public int TareaId { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public bool Completado { get; set; }

    public int? Orden { get; set; }

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