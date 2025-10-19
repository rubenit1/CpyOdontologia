using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOs.Tratamiento;

    [ApiController]
    [Route("api/tratamientos")]
    [Authorize]
    public sealed class TratamientoController : ControllerBase
    {
        private readonly TratamientoRepo _repo;

        public TratamientoController(TratamientoRepo repo)
        {
            _repo = repo;
        }

        private ActionResult ToHttp(int? codigo, string mensaje) => codigo switch
        {
            201 => StatusCode(StatusCodes.Status201Created, new { mensaje }),
            204 => NoContent(),
            400 => BadRequest(new { mensaje }), // Errores de validación/datos
            404 => NotFound(new { mensaje }), // Recurso no encontrado
            500 => StatusCode(500, new { mensaje }), // Error interno
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Respuesta de servidor no esperada." })
        };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TratamientoDto>>> Get(
            [FromQuery] int? procedimientoId,
            [FromQuery] bool? estado)
        {
            var tratamientos = await _repo.ListarAsync(null, procedimientoId, estado);
            return Ok(tratamientos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TratamientoDto>> GetIndividual(int id)
        {
            var resultado = await _repo.ListarAsync(id, null, null);
            var tratamiento = resultado.FirstOrDefault();

            if (tratamiento == null)
            {
                return NotFound($"Tratamiento con ID {id} no encontrado.");
            }
            return Ok(tratamiento);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TratamientoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (ok, mensaje, codigo) = await _repo.CrearAsync(dto);

            if (ok)
            {
                return ToHttp(codigo, mensaje);
            }

            return ToHttp(codigo, mensaje);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TratamientoUpdateDto dto)
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