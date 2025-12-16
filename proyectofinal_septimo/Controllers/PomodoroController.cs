using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Data;
using proyectofinal_septimo.Models;

namespace proyectofinal_septimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PomodoroController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PomodoroController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Guardar sesión terminada
        [HttpPost]
        public async Task<IActionResult> RegistrarSesion([FromBody] PomodoroSesion sesion)
        {
            sesion.SesionID = Guid.NewGuid();
            sesion.FechaRegistro = DateTime.Now;

            _context.PomodoroSesiones.Add(sesion);
            await _context.SaveChangesAsync();
            return Ok(sesion);
        }

        // GET: Estadísticas de HOY del usuario
        [HttpGet("stats/hoy/{usuarioId}")]
        public async Task<IActionResult> GetStatsHoy(Guid usuarioId)
        {
            var hoy = DateTime.Today; // 00:00 horas de hoy

            var sesionesHoy = await _context.PomodoroSesiones
                .Where(p => p.UsuarioID == usuarioId && p.FechaRegistro >= hoy && p.TipoSesion == "Focus")
                .ToListAsync();

            var totalMinutos = sesionesHoy.Sum(s => s.DuracionMinutos);
            var cantidadSesiones = sesionesHoy.Count;

            return Ok(new
            {
                Minutos = totalMinutos,
                Sesiones = cantidadSesiones
            });
        }
    }
}