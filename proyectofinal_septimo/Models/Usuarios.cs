using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models
{
    [Table("Usuarios")]
    public class Usuarios
    {
        [Key]
        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }      
        [Required]
        [StringLength(100)]
        public string ApellidoPaterno { get; set; }

        [StringLength(100)]
        public string? ApellidoMaterno { get; set; }
        // -----------------------------

        public DateTime? FechaNacimiento { get; set; }
        public string? FotoPerfilURL { get; set; }

        // Auditoría
        public Guid? UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Guid? UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Estatus { get; set; }
    }
}