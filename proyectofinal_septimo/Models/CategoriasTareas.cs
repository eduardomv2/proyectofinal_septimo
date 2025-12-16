using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("CategoriasTareas")]
public class CategoriasTareas
{
    [Key]
    public int CategoriaId { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

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