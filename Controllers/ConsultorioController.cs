    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using DTOs.Consultorio;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación
    public sealed class ConsultorioController : ControllerBase
    {
        private readonly ConsultorioRepo _repo;
        public ConsultorioController(ConsultorioRepo repo) => _repo = repo;

        [HttpGet] // activos
        public async Task<ActionResult<IEnumerable<ConsultorioDto>>> GetActivos()
            => Ok(await _repo.ListarActivosAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConsultorioDto>> Get(int id)
            => (await _repo.ObtenerAsync(id)) is { } x ? Ok(x) : NotFound();

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ConsultorioCreateDto dto)
        {
            var (ok, msg, code) = await _repo.CrearAsync(dto);
            return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                    : ToHttp(code, msg);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ConsultorioUpdateDto dto)
        {
            var (ok, msg, code) = await _repo.ActualizarAsync(id, dto);
            return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                    : ToHttp(code, msg);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, msg, code) = await _repo.DesactivarAsync(id);
            return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                    : ToHttp(code, msg);
        }

        private ObjectResult ToHttp(int? codigo, string mensaje) => codigo switch
        {
            404 => NotFound(new { mensaje }),
            409 => Conflict(new { mensaje }),
            500 => StatusCode(500, new { mensaje }),
            _   => BadRequest(new { mensaje })
        };
    }