using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.MetodoPago;

[ApiController]
[Route("api/[controller]")]
[Authorize] // se requiere autenticación para todos los endpoints
public sealed class MetodoPagoController : ControllerBase
{
    private readonly MetodoPagoRepo _repo;
    public MetodoPagoController(MetodoPagoRepo repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MetodoPagoDto>>> GetActivos()
        => Ok(await _repo.ListarActivosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MetodoPagoDto>> Get(int id)
        => (await _repo.ObtenerAsync(id)) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MetodoPagoCreateDto dto)
    {
        var (ok, msg, code) = await _repo.CrearAsync(dto);
        return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                  : ToHttp(code, msg);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] MetodoPagoUpdateDto dto)
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