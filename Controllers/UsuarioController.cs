using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.Usuario;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public sealed class UsuariosController : ControllerBase
{
    private readonly UsuarioRepo _repo;
    public UsuariosController(UsuarioRepo repo) => _repo = repo;

    // Mapea el Codigo del SP a un resultado HTTP
    private ObjectResult ToHttp(int? codigo, string mensaje) => codigo switch
    {
        404 => NotFound(new { mensaje }),
        409 => Conflict(new { mensaje }),
        500 => StatusCode(500, new { mensaje }),
        201 => StatusCode(201, new { mensaje }),
        200 => Ok(new { mensaje }),
        _ => BadRequest(new { mensaje }) // 400 por defecto
    };

    // R: GET /api/usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioListDto>>> Get()
        => Ok(await _repo.ListarActivosAsync());

    // I: GET /api/usuarios/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDetailDto>> Get(int id)
        => (await _repo.ObtenerAsync(id)) is { } x ? Ok(x) : NotFound();

    // C: POST /api/usuarios
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UsuarioCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (msg, code, id) = await _repo.CrearAsync(dto);

        if (code is >= 200 and < 300 && id.HasValue)
        {
            var nuevoUsuario = await _repo.ObtenerAsync(id.Value);
            return CreatedAtAction(nameof(Get), new { id = id.Value }, nuevoUsuario);
        }

        return ToHttp(code, msg);
    }

    // U: PUT /api/usuarios/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] UsuarioUpdateDto dto)
    {
        var (msg, code, returnedId) = await _repo.ActualizarAsync(id, dto);

        // Éxito (2xx): respóndelo con el mismo estándar que manda el SP
        if (code is >= 200 and < 300)
            return Ok(new { mensaje = msg, codigo = code, id = returnedId });

        // Error/negocio: usa el mapeo existente
        return ToHttp(code, msg);
    }

    // D: DELETE /api/usuarios/{id} (soft delete)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (msg, code, returnedId) = await _repo.DesactivarAsync(id);

        // Éxito (2xx): responde con el mismo estándar que manda el SP
        if (code is >= 200 and < 300)
            return Ok(new { mensaje = msg, codigo = code, id = returnedId ?? id });

        // Error/negocio: usa el mapeo existente
        return ToHttp(code, msg);
    }
}
