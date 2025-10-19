using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOs.PlanTratamiento;
using System.Security.Claims;

    [ApiController]
    [Route("api/planes-tratamiento")]
    [Authorize]
    public sealed class PlanTratamientoController : ControllerBase
    {
        private readonly PlanTratamientoRepo _repo;

        public PlanTratamientoController(PlanTratamientoRepo repo)
        {
            _repo = repo;
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            var possibleIdClaimNames = new[] {
                ClaimTypes.NameIdentifier,
                "id", "Id", "userId"
            };

            foreach (var name in possibleIdClaimNames)
            {
                var userIdClaim = User.FindFirst(name);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }

            // 2. Si no se encuentra el ID numérico, buscar el nombre de usuario
            // El token SÍ contiene el ClaimTypes.Name (o sinónimo) con el nombre de usuario.
            var usernameClaim = User.FindFirst(ClaimTypes.Name);

            if (usernameClaim == null)
            {
                usernameClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            }

            if (usernameClaim != null && !string.IsNullOrWhiteSpace(usernameClaim.Value))
            {
                return await _repo.ObtenerIdUsuarioPorNombreAsync(usernameClaim.Value);
            }

            return null;
        }

        private ActionResult ToHttp(int? codigo, string mensaje) => codigo switch
        {
            201 => StatusCode(StatusCodes.Status201Created, new { mensaje }),
            204 => NoContent(),
            400 => BadRequest(new { mensaje }),
            404 => NotFound(new { mensaje }),
            500 => StatusCode(500, new { mensaje }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Respuesta de servidor no esperada." })
        };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanTratamientoDto>>> Get(
            [FromQuery] int? pacienteId,
            [FromQuery] bool? estado)
        {
            var planes = await _repo.ListarAsync(null, pacienteId, estado);
            return Ok(planes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlanTratamientoDto>> GetIndividual(int id)
        {
            var resultado = await _repo.ListarAsync(id, null, null);
            var plan = resultado.FirstOrDefault();

            if (plan == null)
            {
                return NotFound($"Plan de Tratamiento con ID {id} no encontrado.");
            }
            return Ok(plan);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PlanTratamientoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int? creadoPorUsuarioId = await GetCurrentUserIdAsync();

            if (!creadoPorUsuarioId.HasValue)
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del usuario. Asegúrate de que el token contenga el nombre de usuario válido para la base de datos." });
            }

            var (ok, mensaje, codigo) = await _repo.CrearAsync(dto, creadoPorUsuarioId.Value);
            return ToHttp(codigo, mensaje);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] PlanTratamientoUpdateDto dto)
        {
            var (ok, mensaje, codigo) = await _repo.ActualizarAsync(id, dto);
            return ToHttp(codigo, mensaje);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var (ok, mensaje, codigo) = await _repo.AnularAsync(id);
            return ToHttp(codigo, mensaje);
        }
    }
