using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("usuarios_replica")]
public class UsuariosReplica
{
    [Key]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Required]
    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Column("apellido")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Column("fecha_registro")]
    public DateTime? FechaRegistro { get; set; }

    [Required]
    [Column("estatus")]
    public bool Estatus { get; set; }

    [Column("ultima_sincronizacion")]
    public DateTime? UltimaSincronizacion { get; set; }
}