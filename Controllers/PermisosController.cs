using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.Permisos;

[ApiController]
[Route("api/[controller]")]
[Authorize] // quita esto si quieres acceso público
public sealed class PermisosController : ControllerBase
{
    private readonly PermisosRepo _repo;

    public PermisosController(PermisosRepo repo) => _repo = repo;

    // GET /api/permisos?q=texto
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermisoDto>>> Get([FromQuery] string? q)
        => Ok(await _repo.ListarAsync(q));
}