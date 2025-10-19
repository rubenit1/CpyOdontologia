using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.ResultadoConfirmacion;

[ApiController]
[Route("api/[controller]")]
[Authorize] // cualquier usuario autenticado
public sealed class ResultadoConfirmacionController : ControllerBase
{
    private readonly ResultadoConfirmacionRepo _repo;
    public ResultadoConfirmacionController(ResultadoConfirmacionRepo repo) => _repo = repo;

    [HttpGet] // activos
    public async Task<ActionResult<IEnumerable<ResultadoConfirmacionDto>>> GetActivos()
        => Ok(await _repo.ListarActivosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResultadoConfirmacionDto>> Get(int id)
        => (await _repo.ObtenerAsync(id)) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ResultadoConfirmacionCreateDto dto)
    {
        var (ok, msg, code) = await _repo.CrearAsync(dto);
        return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                  : ToHttp(code, msg);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] ResultadoConfirmacionUpdateDto dto)
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