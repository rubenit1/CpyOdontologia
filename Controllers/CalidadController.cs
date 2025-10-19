using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.Calidad;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public sealed class CalidadController : ControllerBase
{
    private readonly CalidadRepo _repo;
    public CalidadController(CalidadRepo repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalidadDto>>> GetActivas()
    {
        try { 
            var result = await _repo.ListarActivasAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }
    

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CalidadDto>> Get(int id)
    {
        var item = await _repo.ObtenerAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CalidadCreateDto dto)
    {
        var (ok, msg, code) = await _repo.CrearAsync(dto);
        if (!ok) return StatusCode(code ?? 500, new { mensaje = msg, codigo = code });
        return Ok(new { mensaje = msg, codigo = code ?? 200 });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] CalidadUpdateDto dto)
    {
        var (ok, msg, code) = await _repo.ActualizarAsync(id, dto);
        if (!ok) return StatusCode(code ?? 500, new { mensaje = msg, codigo = code });
        return Ok(new { mensaje = msg, codigo = code ?? 200 });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, msg, code) = await _repo.DesactivarAsync(id);
        if (!ok) return StatusCode(code ?? 500, new { mensaje = msg, codigo = code });
        return Ok(new { mensaje = msg, codigo = code ?? 200 });
    }
}