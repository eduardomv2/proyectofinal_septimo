using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Data;
using proyectofinal_septimo.Models;

namespace proyectofinal_septimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/notas/usuario/{id}
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Notas>>> GetNotas(Guid usuarioId)
        {
            return await _context.Notas
                .Where(n => n.UsuarioId == usuarioId && n.Estatus == true && n.EsArchivada == false)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();
        }

        // POST: Crear
        [HttpPost]
        public async Task<IActionResult> CrearNota([FromBody] Notas nota)
        {
            nota.NotaId = Guid.NewGuid();
            nota.FechaCreacion = DateTime.Now; // Usamos DateTime.Now para SQL Server
            nota.Estatus = true;
            nota.EsArchivada = false;

            if (nota.TipoId == 0 || nota.TipoId == null) nota.TipoId = 1;

            _context.Notas.Add(nota);
            await _context.SaveChangesAsync();
            return Ok(nota);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarNota(Guid id, [FromBody] Notas notaEditada)
        {
            // Validamos que exista (opcional, pero buena práctica)
            // Nota: FindAsync sí funciona porque es un SELECT, el problema son los UPDATES
            var existe = await _context.Notas.AnyAsync(n => n.NotaId == id);
            if (!existe) return NotFound();

            // SQL Directo para evitar el error de Linked Server
            var sql = @"UPDATE notas 
                        SET titulo = {0}, 
                            cuerpo = {1}, 
                            usuario_modificacion = {2},
                            fecha_modificacion = GETDATE() 
                        WHERE nota_id = {3}";

            await _context.Database.ExecuteSqlRawAsync(
                sql,
                notaEditada.Titulo,
                notaEditada.Cuerpo,
                notaEditada.UsuarioId,
                id
            );

            // Devolvemos el objeto editado (manual, porque el update no retorna nada)
            notaEditada.NotaId = id;
            return Ok(notaEditada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarNota(Guid id)
        {
            // OJO: Usamos SQL directo para evitar el error de "OUTPUT clause" del Linked Server
            // Como cambiamos el tipo a INT en Postgres: 0 = Borrado, 1 = Activo

            var sql = "UPDATE notas SET estatus = 0, fecha_modificacion = GETDATE() WHERE nota_id = {0}";

            // ExecuteSqlRawAsync envía la orden directa sin adornos
            await _context.Database.ExecuteSqlRawAsync(sql, id);

            return Ok(new { message = "Nota eliminada" });
        }
    }
}