using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOs.PacientePreferencias;

    [ApiController]
    // La ruta es anidada: /api/pacientes/{id_paciente}/preferencias
    [Route("api/pacientes/{pacienteId}/preferencias")]
    [Authorize]
    public sealed class PacientePreferenciaController : ControllerBase
    {
        private readonly PacientePreferenciaRepo _repo;

        public PacientePreferenciaController(PacientePreferenciaRepo repo)
        {
            _repo = repo;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PacientePreferenciaDto>))]
        public async Task<ActionResult<IEnumerable<PacientePreferenciaDto>>> Get(int pacienteId)
        {
            if (pacienteId <= 0) return BadRequest("El ID del paciente es inválido.");

            var preferencias = await _repo.ListarPorPacienteAsync(pacienteId);
            return Ok(preferencias);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        public async Task<ActionResult> Post(int pacienteId, PacientePreferenciaCreateDto dto)
        {
            if (pacienteId != dto.PacienteId)
            {
                return BadRequest("El ID del paciente en la URL no coincide con el ID del cuerpo.");
            }

            var (ok, mensaje, codigo) = await _repo.CrearAsync(dto);

            if (ok)
            {
                return StatusCode(StatusCodes.Status201Created, mensaje);
            }

            return StatusCode(codigo ?? StatusCodes.Status400BadRequest, mensaje);
        }

        [HttpPut("{preferenciaId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Put(int pacienteId, int preferenciaId, PacientePreferenciaUpdateDto dto)
        {
            var (ok, mensaje, codigo) = await _repo.ActualizarAsync(pacienteId, preferenciaId, dto);

            if (ok)
            {
                return NoContent();
            }

            return StatusCode(codigo ?? StatusCodes.Status400BadRequest, mensaje);
        }

        [HttpDelete("{preferenciaId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(int pacienteId, int preferenciaId)
        {
            var (ok, mensaje, codigo) = await _repo.DesactivarAsync(pacienteId, preferenciaId);

            if (ok)
            {
                return NoContent();
            }

            return StatusCode(codigo ?? StatusCodes.Status400BadRequest, mensaje);
        }
    }