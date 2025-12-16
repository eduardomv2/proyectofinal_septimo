using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("reflexiones_semanales")]
public class ReflexionesSemanales
{
    [Key]
    [Column("reflexion_id")]
    public int ReflexionId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("semana_numero")]
    public int? SemanaNro { get; set; }

    [Column("anio")]
    public int? Anio { get; set; }

    [Column("fecha_inicio")]
    public DateTime? FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    [Column("logros")]
    public string? Logros { get; set; }

    [Column("desafios")]
    public string? Desafios { get; set; }

    [Column("aprendizajes")]
    public string? Aprendizajes { get; set; }

    [Column("objetivos_proxima_semana")]
    public string? ObjetivosProximaSemana { get; set; }

    [Column("calificacion_semana")]
    public int? CalificacionSemana { get; set; }

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