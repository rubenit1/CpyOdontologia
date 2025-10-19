// Controllers/AseguradorasController.cs
using DTOs.Aseguradoras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class AseguradorasController : ControllerBase
{
    private readonly IAseguradorasRepo _repo;
    public AseguradorasController(IAseguradorasRepo repo) => _repo = repo;

    [HttpGet] // R: solo activos
    public async Task<ActionResult<IEnumerable<AseguradoraDto>>> Listar()
        => Ok(await _repo.ListarActivasAsync());

    [HttpGet("{id:int}")] // I
    public async Task<ActionResult<AseguradoraDto>> Obtener(int id)
        => (await _repo.ObtenerAsync(id)) is { } x ? Ok(x) : NotFound();

    [HttpPost] // C
    public async Task<IActionResult> Crear([FromBody] AseguradoraCreateDto dto)
    {
        var (ok, msg, code) = await _repo.CrearAsync(dto);
        if (!ok) return ToHttp(code, msg);
        return Ok(new { mensaje = msg });
    }

    [HttpPut("{id:int}")] // U
    public async Task<IActionResult> Actualizar(int id, [FromBody] AseguradoraUpdateDto dto)
    {
        var (ok, msg, code) = await _repo.ActualizarAsync(id, dto);
        if (!ok) return ToHttp(code, msg);
        return Ok(new { mensaje = msg });
    }

    [HttpDelete("{id:int}")] // D lógico
    public async Task<IActionResult> Desactivar(int id)
    {
        var (ok, msg, code) = await _repo.DesactivarAsync(id);
        if (!ok) return ToHttp(code, msg);
        return Ok(new { mensaje = msg });
    }

    private ObjectResult ToHttp(int? codigo, string mensaje) => codigo switch
    {
        404 => NotFound(new { mensaje }),
        409 => Conflict(new { mensaje }),
        500 => StatusCode(500, new { mensaje }),
        _   => BadRequest(new { mensaje })
    };
}