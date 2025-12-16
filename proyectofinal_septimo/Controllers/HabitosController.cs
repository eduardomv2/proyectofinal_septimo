using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Data;
using proyectofinal_septimo.Models;

namespace proyectofinal_septimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HabitosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/habitos/hoy/usuario/{usuarioId}
        // Este endpoint trae SOLO los hábitos que tocan HOY + si ya fueron completados
        [HttpGet("hoy/usuario/{usuarioId}")]
        public async Task<IActionResult> GetHabitosHoy(Guid usuarioId)
        {
            var hoy = DateTime.Today;       // Fecha sin hora
            var diaSemana = hoy.DayOfWeek;  // Qué día es hoy (Monday, Tuesday...)

            // 1. Filtramos los hábitos del usuario que estén activos
            var query = _context.Habitos
                .Where(h => h.UsuarioId == usuarioId && h.Estatus == true);

            // 2. Filtramos para que solo traiga los que tocan el día de hoy
            // (Nota: Esto asume que agregaste los bool Lunes, Martes... en tu modelo Habitos)
            switch (diaSemana)
            {
                case DayOfWeek.Monday: query = query.Where(h => h.Lunes); break;
                case DayOfWeek.Tuesday: query = query.Where(h => h.Martes); break;
                case DayOfWeek.Wednesday: query = query.Where(h => h.Miercoles); break;
                case DayOfWeek.Thursday: query = query.Where(h => h.Jueves); break;
                case DayOfWeek.Friday: query = query.Where(h => h.Viernes); break;
                case DayOfWeek.Saturday: query = query.Where(h => h.Sabado); break;
                case DayOfWeek.Sunday: query = query.Where(h => h.Domingo); break;
            }

            var habitosDelDia = await query.ToListAsync();
            var resultado = new List<object>();

            foreach (var habito in habitosDelDia)
            {
                // 1. Check de hoy
                bool yaEstaCompletado = await _context.BitacoraHabitos
                    .AnyAsync(b => b.HabitoId == habito.HabitoId && b.Fecha == hoy);

                // 2. NUEVO: Contamos cuántas veces se ha cumplido en TOTAL (Racha simple)
                int rachaTotal = await _context.BitacoraHabitos
                    .CountAsync(b => b.HabitoId == habito.HabitoId);

                resultado.Add(new
                {
                    Habito = habito,
                    CompletadoHoy = yaEstaCompletado,
                    Racha = rachaTotal // <--- ENVIAMOS ESTE DATO NUEVO
                });
            }

            return Ok(resultado);
        }

        // POST (Crear)
        [HttpPost]
        public async Task<IActionResult> CrearHabito([FromBody] Habitos nuevoHabito)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (nuevoHabito.HabitoId == Guid.Empty)
                    nuevoHabito.HabitoId = Guid.NewGuid();

                nuevoHabito.FechaCreacion = DateTime.Now;
                nuevoHabito.Estatus = true; // Activo por defecto

                // Asegúrate de que el Frontend envíe Lunes=true, Martes=false, etc.
                // Si no, aquí podrías poner valores por defecto.

                if (nuevoHabito.UsuarioCreacion == Guid.Empty)
                    nuevoHabito.UsuarioCreacion = nuevoHabito.UsuarioId;

                _context.Habitos.Add(nuevoHabito);
                await _context.SaveChangesAsync();

                // Retornamos OK simple para no complicar con rutas
                return Ok(nuevoHabito);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // POST (Toggle / Check) - AHORA USA LA BITÁCORA
        [HttpPost("check/{habitoId}")]
        public async Task<IActionResult> ToggleCheck(Guid habitoId)
        {
            var hoy = DateTime.Today;

            // 1. Buscamos si ya existe el registro hoy
            var registroExistente = await _context.BitacoraHabitos
                .FirstOrDefaultAsync(b => b.HabitoId == habitoId && b.Fecha == hoy);

            if (registroExistente != null)
            {
                // CASO A: Ya estaba marcado -> Lo desmarcamos (Borramos el registro)
                _context.BitacoraHabitos.Remove(registroExistente);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Hábito desmarcado", completado = false });
            }
            else
            {
                // CASO B: No estaba marcado -> Lo marcamos (Creamos el registro)
                var nuevoRegistro = new BitacoraHabitos
                {
                    HabitoId = habitoId,
                    Fecha = hoy,
                    Completado = true
                };

                _context.BitacoraHabitos.Add(nuevoRegistro);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Hábito completado", completado = true });
            }
        }

        // PUT (Editar)
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarHabito(Guid id, [FromBody] Habitos habitoEditado)
        {
            if (id != habitoEditado.HabitoId) return BadRequest("ID mismatch");

            var habitoExistente = await _context.Habitos.FindAsync(id);
            if (habitoExistente == null) return NotFound();

            // Actualizar datos básicos
            habitoExistente.Nombre = habitoEditado.Nombre;
            habitoExistente.MetaCantidad = habitoEditado.MetaCantidad;
            habitoExistente.FrecuenciaId = habitoEditado.FrecuenciaId;
            habitoExistente.Horario = habitoEditado.Horario;

            // --- ACTUALIZAR DÍAS DE LA SEMANA ---
            // Esto permite cambiar los días si el usuario edita el hábito
            habitoExistente.Lunes = habitoEditado.Lunes;
            habitoExistente.Martes = habitoEditado.Martes;
            habitoExistente.Miercoles = habitoEditado.Miercoles;
            habitoExistente.Jueves = habitoEditado.Jueves;
            habitoExistente.Viernes = habitoEditado.Viernes;
            habitoExistente.Sabado = habitoEditado.Sabado;
            habitoExistente.Domingo = habitoEditado.Domingo;
            // ------------------------------------

            habitoExistente.FechaModificacion = DateTime.Now;
            habitoExistente.UsuarioModificacion = habitoEditado.UsuarioId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarHabito(Guid id)
        {
            var habito = await _context.Habitos.FindAsync(id);
            if (habito == null) return NotFound();

            habito.Estatus = false; // Soft Delete
            habito.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}