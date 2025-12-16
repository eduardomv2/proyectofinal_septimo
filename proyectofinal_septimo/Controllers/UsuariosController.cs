using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Data;
using proyectofinal_septimo.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
}