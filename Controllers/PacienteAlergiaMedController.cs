using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOs.PacienteAlergiaMed;


    [ApiController]
    [Route("api/pacientes/{pacienteId}/alergias")]
    [Authorize]
    public sealed class PacienteAlergiaMedController : ControllerBase
    {
        private readonly PacienteAlergiaMedRepo _repo;

        public PacienteAlergiaMedController(PacienteAlergiaMedRepo repo)
        {
            _repo = repo;
        }

        // Helper para mapear el código del SP al resultado HTTP
        private ObjectResult ToHttp(int? codigo, string mensaje) => codigo switch
        {
            // 200 OK (Manejado por POST/PUT/DELETE si el SP devuelve una advertencia/éxito)
            200 => Ok(new { mensaje }),
            // 404 Not Found (Paciente/Medicamento no existe)
            404 => NotFound(new { mensaje }),
            // 409 Conflict (Duplicado)
            409 => Conflict(new { mensaje }),
            // 500 Internal Server Error (Error del SP/SQL)
            500 => StatusCode(500, new { mensaje }),
            // Por defecto: 400 Bad Request
            _ => BadRequest(new { mensaje })
        };


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteAlergiaMedDto>>> Get(int pacienteId)
        {
            if (pacienteId <= 0) return BadRequest("El ID del paciente es inválido.");

            // Si falla, el try-catch implícito de ASP.NET Core capturará la excepción y devolverá 500.
            var alergias = await _repo.ListarPorPacienteAsync(pacienteId);

            return Ok(alergias);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int pacienteId, [FromBody] PacienteAlergiaMedCreateDto dto)
        {
            if (pacienteId != dto.PacienteId)
            {
                return BadRequest("El ID del paciente en la URL no coincide con el ID del cuerpo.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (ok, mensaje, codigo) = await _repo.CrearAsync(dto);

            if (ok)
            {
                // Si es éxito (código 200), devuelve 201 Created (o 200 OK si el SP fue solo una advertencia 200).
                return Created(string.Empty, new { mensaje });
            }

            // Si falla, usa el helper para mapear 404, 409 o 500.
            return ToHttp(codigo, mensaje);
        }

        [HttpPut("{medicamentoId}")]
        public async Task<ActionResult> Put(int pacienteId, int medicamentoId, [FromBody] PacienteAlergiaMedUpdateDto dto)
        {
            var (ok, mensaje, codigo) = await _repo.ActualizarAsync(pacienteId, medicamentoId, dto);

            if (ok)
            {
                // Si la actualización es exitosa (código 200), devuelve 204 No Content.
                return NoContent();
            }

            return ToHttp(codigo, mensaje);
        }

        [HttpDelete("{medicamentoId}")]
        public async Task<ActionResult> Delete(int pacienteId, int medicamentoId)
        {
            var (ok, mensaje, codigo) = await _repo.DesactivarAsync(pacienteId, medicamentoId);

            if (ok)
            {
                // Si la desactivación es exitosa (código 200), devuelve 204 No Content.
                return NoContent();
            }

            return ToHttp(codigo, mensaje);
        }
    }