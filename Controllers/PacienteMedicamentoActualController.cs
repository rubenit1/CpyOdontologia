using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOs.PacienteMedicamentoActual;

namespace AppOdontologia.Controllers
{
    [ApiController]
    [Route("api/pacientes/{pacienteId}/medicamentos-actuales")]
    [Authorize]
    public sealed class PacienteMedicamentoActualController : ControllerBase
    {
        private readonly PacienteMedicamentoActualRepo _repo;

        public PacienteMedicamentoActualController(PacienteMedicamentoActualRepo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PacienteMedicamentoActualDto>))]
        public async Task<ActionResult<IEnumerable<PacienteMedicamentoActualDto>>> Get(int pacienteId)
        {
            if (pacienteId <= 0) return BadRequest("El ID del paciente es inválido.");

            var medicamentos = await _repo.ListarPorPacienteAsync(pacienteId);
            return Ok(medicamentos);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        public async Task<ActionResult> Post(int pacienteId, PacienteMedicamentoActualCreateDto dto)
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


        [HttpPut("{medicamentoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Put(int pacienteId, int medicamentoId, PacienteMedicamentoActualUpdateDto dto)
        {
            var (ok, mensaje, codigo) = await _repo.ActualizarAsync(pacienteId, medicamentoId, dto);

            if (ok)
            {
                return NoContent();
            }

            return StatusCode(codigo ?? StatusCodes.Status400BadRequest, mensaje);
        }


        [HttpDelete("{medicamentoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(int pacienteId, int medicamentoId)
        {
            var (ok, mensaje, codigo) = await _repo.DesactivarAsync(pacienteId, medicamentoId);

            if (ok)
            {
                return NoContent();
            }

            return StatusCode(codigo ?? StatusCodes.Status400BadRequest, mensaje);
        }
    }
}