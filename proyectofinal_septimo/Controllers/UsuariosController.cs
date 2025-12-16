using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Data;
using proyectofinal_septimo.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;


namespace proyectofinal_septimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/usuarios (REGISTRO) - ORIGINAL
        [HttpPost]
        public async Task<ActionResult<Usuarios>> PostUsuario(Usuarios usuario)
        {
            if (ContieneNumeros(usuario.Nombre) ||
                ContieneNumeros(usuario.ApellidoPaterno) ||
                (usuario.ApellidoMaterno != null && ContieneNumeros(usuario.ApellidoMaterno)))
            {
                return BadRequest(new { message = "Los nombres y apellidos no pueden contener números." });
            }

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
            {
                return BadRequest(new { message = "El correo electrónico ya está registrado." });
            }

            if (string.IsNullOrEmpty(usuario.PasswordHash))
            {
                return BadRequest(new { message = "La contraseña es obligatoria." });
            }
            usuario.PasswordHash = EncriptarPassword(usuario.PasswordHash);

            if (usuario.UsuarioID == Guid.Empty) usuario.UsuarioID = Guid.NewGuid();
            usuario.UsuarioCreacion = usuario.UsuarioID;
            usuario.FechaCreacion = DateTime.Now;
            usuario.Estatus = true;

            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }

            return CreatedAtAction("GetUsuarios", new { id = usuario.UsuarioID }, usuario);
        }

        // POST: api/usuarios/login (LOGIN) - ORIGINAL
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var passwordHasheado = EncriptarPassword(request.Password);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == passwordHasheado);

            if (usuario == null)
            {
                return Unauthorized(new { message = "Correo o contraseña incorrectos" });
            }

            return Ok(new
            {
                message = "Login exitoso",
                usuarioId = usuario.UsuarioID,
                nombre = usuario.Nombre,
                email = usuario.Email
            });
        }

        // GET: api/usuarios - ORIGINAL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // --- GOOGLE LOGIN (ORIGINAL SIN CAMBIOS) ---
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.TokenId))
                {
                    return BadRequest(new { message = "El frontend envió un token vacío." });
                }

                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(request.TokenId);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Token inválido o expirado: " + ex.Message });
                }

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (usuario == null)
                {
                    usuario = new Usuarios
                    {
                        UsuarioID = Guid.NewGuid(),
                        Nombre = payload.GivenName ?? "Usuario",
                        ApellidoPaterno = payload.FamilyName ?? "Google",
                        Email = payload.Email,
                        PasswordHash = Guid.NewGuid().ToString(),
                        FechaCreacion = DateTime.Now,
                        Estatus = true
                    };

                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    message = "Login exitoso",
                    usuarioId = usuario.UsuarioID,
                    nombre = usuario.Nombre,
                    email = usuario.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ERROR CRITICO EN SERVIDOR: " + ex.ToString() });
            }
        }

        // --- NUEVO MÉTODO PARA ANGULAR (PAGINACIÓN) ---
        // Este es el único agregado nuevo para soportar los 100,000 registros
        [HttpGet("paginado")]
        public async Task<IActionResult> GetUsuariosPaginados([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var skip = (page - 1) * pageSize;
            var totalRegistros = await _context.Usuarios.CountAsync();

            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Nombre)
                .Skip(skip)
                .Take(pageSize)
                .Select(u => new {
                    u.UsuarioID,
                    NombreCompleto = u.Nombre + " " + u.ApellidoPaterno + " " + (u.ApellidoMaterno ?? ""),
                    u.Email,
                    u.FechaNacimiento,
                    u.Estatus
                })
                .ToListAsync();

            return Ok(new
            {
                Total = totalRegistros,
                Page = page,
                PageSize = pageSize,
                Data = usuarios
            });
        }


        [HttpGet("exportar")]
        public async Task<IActionResult> ExportarUsuarios(
    [FromQuery] string formato,
    [FromQuery] DateTime? fechaInicio,
    [FromQuery] DateTime? fechaFin)
        {
            try
            {
                // 1. Timeout de seguridad (5 minutos)
                _context.Database.SetCommandTimeout(300);

                var query = _context.Usuarios.AsNoTracking().AsQueryable();

                if (fechaInicio.HasValue)
                    query = query.Where(u => u.FechaCreacion >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(u => u.FechaCreacion <= fechaFin.Value.AddDays(1).AddTicks(-1));

                // 2. Obtener datos
                var datos = await query
                    .OrderByDescending(u => u.FechaCreacion)
                    .Select(u => new UsuarioExport
                    {
                        Nombre = u.Nombre,
                        Apellido = u.ApellidoPaterno,
                        Email = u.Email,
                        FechaRegistro = u.FechaCreacion,
                        Estatus = u.Estatus ? "Activo" : "Inactivo"
                    })
                    .ToListAsync();

                // 3. Validación de vacío
                if (datos == null || datos.Count == 0)
                {
                    return NotFound(new { message = "No se encontraron registros en el rango de fechas seleccionado." });
                }

                byte[] archivoBytes;
                string mimeType;
                string nombreArchivo;

                switch (formato.ToLower())
                {
                    case "json":
                        var json = System.Text.Json.JsonSerializer.Serialize(datos);
                        archivoBytes = Encoding.UTF8.GetBytes(json);
                        mimeType = "application/json";
                        nombreArchivo = "usuarios.json";
                        break;

                    case "xml":
                        var xmlSerializer = new XmlSerializer(typeof(List<UsuarioExport>));
                        using (var ms = new MemoryStream())
                        {
                            xmlSerializer.Serialize(ms, datos);
                            archivoBytes = ms.ToArray();
                        }
                        mimeType = "application/xml";
                        nombreArchivo = "usuarios.xml";
                        break;

                    case "csv":
                        var sb = new StringBuilder();
                        sb.AppendLine("Nombre,Apellido,Email,Fecha Registro,Estatus");
                        foreach (var u in datos)
                        {
                            sb.AppendLine($"{u.Nombre},{u.Apellido},{u.Email},{u.FechaRegistro:yyyy-MM-dd},{u.Estatus}");
                        }
                        archivoBytes = Encoding.UTF8.GetBytes(sb.ToString());
                        mimeType = "text/csv";
                        nombreArchivo = "usuarios.csv";
                        break;

                    case "pdf":
                        using (var ms = new MemoryStream())
                        {
                            var writer = new PdfWriter(ms);
                            var pdf = new PdfDocument(writer);
                            var document = new Document(pdf);

                            // Título
                            document.Add(new Paragraph("Reporte de Usuarios").SetFontSize(20));
                            document.Add(new Paragraph($"Generado: {DateTime.Now}"));

                            // Tabla (Usamos porcentajes para asegurar ancho correcto)
                            // Esto requiere: using iText.Layout.Properties;
                            var table = new Table(4);
                            table.SetWidth(UnitValue.CreatePercentValue(100));

                            // --- ENCABEZADOS (FORMA EXPLÍCITA) ---
                            // Creamos la Celda -> Luego el Párrafo -> Luego el Texto
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Nombre")));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Email")));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha")));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Estado")));

                            // --- DATOS ---
                            foreach (var u in datos)
                            {
                                // Validamos nulos explícitamente antes de crear el párrafo
                                string nombre = (u.Nombre ?? "") + " " + (u.Apellido ?? "");
                                string email = u.Email ?? "Sin Email";
                                string fecha = u.FechaRegistro.ToString("yyyy-MM-dd");
                                string estatus = u.Estatus ?? "N/A";

                                table.AddCell(new Cell().Add(new Paragraph(nombre)));
                                table.AddCell(new Cell().Add(new Paragraph(email)));
                                table.AddCell(new Cell().Add(new Paragraph(fecha)));
                                table.AddCell(new Cell().Add(new Paragraph(estatus)));
                            }

                            document.Add(table);
                            document.Close();
                            archivoBytes = ms.ToArray();
                        }
                        mimeType = "application/pdf";
                        nombreArchivo = "usuarios.pdf";
                        break;

                    default:
                        return BadRequest("Formato no soportado");
                }

                return File(archivoBytes, mimeType, nombreArchivo);
            }
            catch (Exception ex)
            {
               

                // Esto imprimirá el error real en la ventana "Salida" (Output) de Visual Studio
                System.Diagnostics.Debug.WriteLine("ERROR PDF: " + ex.ToString());

                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // --- FUNCIONES PRIVADAS / CLASES AUXILIARES (ORIGINALES) ---

        private string EncriptarPassword(string textoPlano)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(textoPlano));
                var sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private bool ContieneNumeros(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return false;
            return Regex.IsMatch(texto, @"\d");
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class GoogleLoginRequest
        {
            [JsonPropertyName("tokenId")]
            public string TokenId { get; set; }
        }
    }

    public class UsuarioExport
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Estatus { get; set; } // Lo cambié a string para que en el XML salga "Activo" directo
    }
}