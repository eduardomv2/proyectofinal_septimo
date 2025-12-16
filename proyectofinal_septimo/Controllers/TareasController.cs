using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Data;
using proyectofinal_septimo.Models;
using System.Threading;

namespace proyectofinal_septimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/tareas/usuario/{id}
        // Solo trae las PENDIENTES (ID=1) y NO BORRADAS (Estatus=true)
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Tareas>>> GetTareasPendientes(Guid usuarioId)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioID == usuarioId && t.Estatus == true && t.EstatusTareaID == 1)
                .OrderBy(t => t.FechaVencimiento) // Ordenar por fecha, lo más urgente primero
                .ToListAsync();
        }

        // POST: Crear
        
        [HttpPost]
        public async Task<IActionResult> CrearTarea([FromBody] Tareas tarea)
        {
            tarea.TareaID = Guid.NewGuid();
            tarea.FechaCreacion = DateTime.Now;
            tarea.Estatus = true;
            tarea.EstatusTareaID = 1;

            // --- AGREGA ESTO ---
            // Si llega un 0, lo convertimos a NULL para que SQL no busque la ID 0
            if (tarea.CategoriaID == 0)
            {
                tarea.CategoriaID = null;
            }
            // -------------------

            if (tarea.PrioridadID == 0) tarea.PrioridadID = 2;

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();
            return Ok(tarea);
        }

        // PUT: Editar
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarTarea(Guid id, [FromBody] Tareas tareaEditada)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            tarea.Titulo = tareaEditada.Titulo;
            tarea.Descripcion = tareaEditada.Descripcion;
            tarea.FechaVencimiento = tareaEditada.FechaVencimiento;
            tarea.PrioridadID = tareaEditada.PrioridadID;
            tarea.CategoriaID = tareaEditada.CategoriaID;

            tarea.FechaModificacion = DateTime.Now;
            tarea.UsuarioModificacion = tareaEditada.UsuarioID;

            await _context.SaveChangesAsync();
            return Ok(tarea);
        }

        // POST: Completar Tarea (Check) -> EstatusTareaID = 2
        [HttpPost("completar/{id}")]
        public async Task<IActionResult> CompletarTarea(Guid id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            tarea.EstatusTareaID = 2; // 2 = COMPLETADA (Ya no saldrá en la lista)
            tarea.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Tarea completada" });
        }

        // DELETE: Eliminar (Soft Delete) -> Estatus = false
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTarea(Guid id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            tarea.Estatus = false; // Borrado lógico
            tarea.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Tarea eliminada" });
        }
    }
}