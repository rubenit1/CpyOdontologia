using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.Odontologo;

[ApiController]
[Route("api/[controller]")]
[Authorize] // requerido para todos los endpoints
public sealed class OdontologoController : ControllerBase
{
    private readonly OdontologoRepo _repo;
    public OdontologoController(OdontologoRepo repo) => _repo = repo;

    private ObjectResult FromException(Exception ex)
        => StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });

    [HttpGet] // activos
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OdontologoDto>>> GetActivos()
    {
        try
        {
            var data = await _repo.ListarActivosAsync();
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return FromException(ex);
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OdontologoDto>> Get(int id)
    {
        try
        {
            var x = await _repo.ObtenerAsync(id);
            return x is { } ? Ok(x) : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return FromException(ex);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] OdontologoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var (ok, msg, nuevoId) = await _repo.CrearAsync(dto);
            return ok
                ? CreatedAtAction(nameof(Get), new { id = nuevoId }, new { id = nuevoId, mensaje = msg })
                : BadRequest(new { mensaje = msg });
        }
        catch (InvalidOperationException ex)
        {
            return FromException(ex);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put(int id, [FromBody] OdontologoUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var (ok, msg, code) = await _repo.ActualizarAsync(id, dto);
            return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                      : ToHttp(code, msg);
        }
        catch (InvalidOperationException ex)
        {
            return FromException(ex);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var (ok, msg, code) = await _repo.DesactivarAsync(id);
            return ok ? Ok(new { mensaje = msg, codigo = code ?? 200 })
                      : ToHttp(code, msg);
        }
        catch (InvalidOperationException ex)
        {
            return FromException(ex);
        }
    }

    private ObjectResult ToHttp(int? codigo, string mensaje) => codigo switch
    {
        404 => NotFound(new { mensaje }),
        409 => Conflict(new { mensaje }),
        500 => StatusCode(500, new { mensaje }),
        _   => BadRequest(new { mensaje })
    };
}