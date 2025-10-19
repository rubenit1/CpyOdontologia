using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.Especialidad;

[ApiController]
[Route("api/[controller]")]
[Authorize] // todas las rutas requieren token válido
public sealed class EspecialidadController : ControllerBase
{
    private readonly EspecialidadRepo _repo;
    public EspecialidadController(EspecialidadRepo repo) => _repo = repo;

    [HttpGet] // activas
    public async Task<ActionResult<IEnumerable<EspecialidadDto>>> GetActivas()
        => Ok(await _repo.ListarActivasAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EspecialidadDto>> Get(int id)
        => (await _repo.ObtenerAsync(id)) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EspecialidadCreateDto dto)
    {
        var (ok, msg, code) = await _repo.CrearAsync(dto);
        return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                  : ToHttp(code, msg);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] EspecialidadUpdateDto dto)
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