using AppOdontologia.Models;
using AppOdontologia.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AppOdontologia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IConfiguration _cfg;

        public AuthController(JwtService jwtService, IConfiguration cfg)
        {
            _jwtService = jwtService;
            _cfg = cfg;
        }

        // Simulación de usuarios
        private readonly List<Usuario> usuarios = new()
        {
            new Usuario { Id = 1, UsuarioNombre = "Usuario1", Password = "password1$2025", Rol = "Admin" }
        };

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.UsuarioNombre) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Solicitud inválida.");

            // 1) Intento con la lista en memoria
            var usuario = usuarios.SingleOrDefault(u =>
                u.UsuarioNombre.Equals(request.UsuarioNombre, StringComparison.OrdinalIgnoreCase)
                && u.Password == request.Password);

            // 2) Si no está en memoria, consultamos la BD mediante el SP
            if (usuario is null)
            {
                try
                {
                    var cs = _cfg.GetConnectionString("DefaultConnection");
                    using var con = new SqlConnection(cs);

                    usuario = await con.QuerySingleOrDefaultAsync<Usuario>(
                        "dbo.sp_auth_login",
                        new
                        {
                            UsuarioNombre = request.UsuarioNombre, // usuario O email
                            Password = request.Password          // texto plano por ahora
                        },
                        commandType: CommandType.StoredProcedure
                    );
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error interno.");
                }
            }

            // 3) Generar token si hay usuario
            if (usuario != null)
            {
                var token = _jwtService.GenerarToken(usuario);
                return Ok(new { Token = token }); // mantenemos el contrato de respuesta
            }

            return Unauthorized("Credenciales inválidas");
        }
    }
}