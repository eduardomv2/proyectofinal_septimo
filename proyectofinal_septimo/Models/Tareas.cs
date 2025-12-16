using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("Tareas")]
public class Tareas
{
    [Key]
    public Guid TareaID { get; set; }

    public Guid UsuarioID { get; set; }

    // Asumiremos que pueden ser nulos si no seleccionas nada
    public int? CategoriaID { get; set; }

    [Required]
    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    // 1=Baja, 2=Media, 3=Alta (Ejemplo)
    public int PrioridadID { get; set; }

    // 1=Pendiente, 2=Completada
    public int EstatusTareaID { get; set; }

    // Auditoría
    public Guid UsuarioCreacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public Guid? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }

    // Soft Delete (true=Activo, false=Eliminado)
    public bool Estatus { get; set; }
}
