using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectofinal_septimo.Models
{
    [Table("PomodoroSesiones")]
    public class PomodoroSesion
    {
        [Key]
        public Guid SesionID { get; set; }

        public Guid UsuarioID { get; set; }

        public Guid? TareaID { get; set; } // Puede ser nulo

        public int DuracionMinutos { get; set; } // Ej: 25

        public string TipoSesion { get; set; } = "Focus"; // Focus, Break

        public DateTime FechaRegistro { get; set; }
    }
}