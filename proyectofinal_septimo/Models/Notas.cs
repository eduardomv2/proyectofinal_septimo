using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models
{
    // Asumo que tu tabla en BD se llama "notas" (en minúsculas) por los nombres de columnas que pasaste.
    // Si en SQL Server se llama "Notas", cambia esto a [Table("Notas")]
    [Table("notas")]
    public class Notas
    {
        [Key]
        [Column("nota_id")]
        public Guid NotaId { get; set; }

        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Column("titulo")]
        public string? Titulo { get; set; }

        [Column("cuerpo")]
        public string? Cuerpo { get; set; }

        [Column("tipo_id")]
        public int? TipoId { get; set; } // Por defecto 1

        [Column("es_archivada")]
        public bool EsArchivada { get; set; }

        [Column("estatus")]
        public bool Estatus { get; set; }

        // Auditoría
        [Column("usuario_creacion")]
        public Guid UsuarioCreacion { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("usuario_modificacion")]
        public Guid? UsuarioModificacion { get; set; }

        [Column("fecha_modificacion")]
        public DateTime? FechaModificacion { get; set; }
    }
}