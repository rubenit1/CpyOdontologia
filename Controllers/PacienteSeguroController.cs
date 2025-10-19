using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOs.PacienteSeguro;

    [ApiController]
    [Route("api/paciente-seguros")] 
    [Authorize]
    public sealed class PacienteSeguroController : ControllerBase
    {
        private readonly PacienteSeguroRepo _repo;

        public PacienteSeguroController(PacienteSeguroRepo repo)
        {
            _repo = repo;
        }
        private ActionResult ToHttp(int? codigo, string mensaje) => codigo switch
        {
            201 => StatusCode(StatusCodes.Status201Created, new { mensaje }),
            204 => NoContent(),
            404 => NotFound(new { mensaje }),
            500 => StatusCode(500, new { mensaje }),
            _ => BadRequest(new { mensaje })
        };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteSeguroDto>>> Get([FromQuery] int? pacienteId)
        {
            var seguros = await _repo.ListarAsync(pacienteId);
            return Ok(seguros);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PacienteSeguroDto>> GetIndividual(int id)
        {
            var seguro = await _repo.LeerIndividualAsync(id);
            if (seguro == null)
            {
                return NotFound($"Seguro con ID {id} no encontrado.");
            }
            return Ok(seguro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PacienteSeguroCreateDto dto)
        {
            var (ok, mensaje, codigo) = await _repo.CrearAsync(dto);

            if (ok)
            {

                return ToHttp(codigo, mensaje);
            }

            return ToHttp(codigo, mensaje);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, PacienteSeguroUpdateDto dto)
        {
            var (ok, mensaje, codigo) = await _repo.ActualizarAsync(id, dto);

            if (ok)
            {
                return ToHttp(codigo, mensaje);
            }

            return ToHttp(codigo, mensaje);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var (ok, mensaje, codigo) = await _repo.DesactivarAsync(id);

            if (ok)
            {

                return ToHttp(codigo, mensaje);
            }

            return ToHttp(codigo, mensaje);
        }
    }