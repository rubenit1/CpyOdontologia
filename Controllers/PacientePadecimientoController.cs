using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs.PacientePadecimiento;

namespace AppOdontologia.Controllers
{
    [ApiController]
    // La ruta es anidada: /api/pacientes/{id_paciente}/padecimientos
    [Route("api/pacientes/{pacienteId}/padecimientos")]
    [Authorize]
    public sealed class PacientePadecimientoController : ControllerBase
    {
        private readonly PacientePadecimientoRepo _repo;

        public PacientePadecimientoController(PacientePadecimientoRepo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PacientePadecimientoDto>))]
        public async Task<ActionResult<IEnumerable<PacientePadecimientoDto>>> Get(int pacienteId)
        {
            if (pacienteId <= 0) return BadRequest("El ID del paciente es inválido.");

            var padecimientos = await _repo.ListarPorPacienteAsync(pacienteId);
            return Ok(padecimientos);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        public async Task<ActionResult> Post(int pacienteId, PacientePadecimientoCreateDto dto)
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

        [HttpPut("{padecimientoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Put(int pacienteId, int padecimientoId, PacientePadecimientoUpdateDto dto)
        {
            var (ok, mensaje, codigo) = await _repo.ActualizarAsync(pacienteId, padecimientoId, dto);

            if (ok)
            {
                return NoContent();
            }

            return StatusCode(codigo ?? StatusCodes.Status400BadRequest, mensaje);
        }


        [HttpDelete("{padecimientoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(int pacienteId, int padecimientoId)
        {
            var (ok, mensaje, codigo) = await _repo.DesactivarAsync(pacienteId, padecimientoId);

            if (ok)
            {
                return NoContent();
            }

            return StatusCode(codigo ?? StatusCodes.Status400BadRequest, mensaje);
        }
    }
}