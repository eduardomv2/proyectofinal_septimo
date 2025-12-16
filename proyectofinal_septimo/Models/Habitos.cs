using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models;

[Table("Habitos")]
public class Habitos
{
    [Key]
    [Column("HabitoID")]
    public Guid HabitoId { get; set; }

    [Required]
    [Column("UsuarioID")]
    public Guid UsuarioId { get; set; }

    [Required]
    [StringLength(200)]
    [Column("Titulo")]
    public string Nombre { get; set; } = string.Empty;

    [Column("MetaCantidad")]
    public int? MetaCantidad { get; set; }

    [Required]
    [Column("FrecuenciaID")]
    public int FrecuenciaId { get; set; }

    [Column("FechaUltimoCompletado")]
    public DateTime? FechaUltimoCompletado { get; set; }

    [Required]
    [Column("UsuarioCreacion")]
    public Guid UsuarioCreacion { get; set; }

    [Required]
    [Column("FechaCreacion")]
    public DateTime FechaCreacion { get; set; }

    [Column("UsuarioModificacion")]
    public Guid? UsuarioModificacion { get; set; }

    [Column("FechaModificacion")]
    public DateTime? FechaModificacion { get; set; }

    [Column("Horario")]
    public TimeSpan? Horario { get; set; }

    // --- AQUÍ ESTABA EL ERROR, YA ESTÁ CORREGIDO ---
    // Los días no necesitan [Column] si se llaman igual en la BD
    public bool Lunes { get; set; }
    public bool Martes { get; set; }
    public bool Miercoles { get; set; }
    public bool Jueves { get; set; }
    public bool Viernes { get; set; }
    public bool Sabado { get; set; }
    public bool Domingo { get; set; }

    // --- MOVÍ LOS ATRIBUTOS AQUÍ ---
    [Required]
    [Column("Estatus")] // Ahora sí, este atributo está sobre su propiedad correcta
    public bool Estatus { get; set; }
}