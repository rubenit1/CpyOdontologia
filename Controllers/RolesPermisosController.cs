using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.RolPermisos;

[ApiController]
[Route("api/roles/{rolId:int}/permisos")]
[Authorize] // opcional pero recomendado
public sealed class RolesPermisosController : ControllerBase
{
    private readonly RolesPermisosRepo _repo;
    public RolesPermisosController(RolesPermisosRepo repo) => _repo = repo;

    [HttpPut]
    public async Task<IActionResult> Put(int rolId, [FromBody] PermisosRequest body)
    {
        var dto = new RolPermisosUpdateDto
        {
            RolId = rolId,
            PermisosIds = (body?.PermisosIds ?? new List<int>()).Distinct().ToArray()
        };

        var (ok, msg, code) = await _repo.SincronizarAsync(dto);
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