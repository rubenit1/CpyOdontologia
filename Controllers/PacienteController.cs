using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs.Paciente;

[ApiController]
[Route("api/pacientes")]
[Authorize]
public sealed class PacientesController : ControllerBase
{
    private readonly PacienteRepo _repo;

    public PacientesController(PacienteRepo repo)
    {
        _repo = repo;
    }
    // GET /api/pacientes/buscar_referido
    [HttpGet("buscar")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PacienteBusquedaDto>))]
    public async Task<ActionResult<IEnumerable<PacienteBusquedaDto>>> Buscar([FromQuery] string termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            return Ok(Enumerable.Empty<PacienteBusquedaDto>());
        }

        var pacientes = await _repo.BuscarAsync(termino);
        return Ok(pacientes);
    }

    // GET /api/pacientes
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PacienteDto>))]
    public async Task<ActionResult<IEnumerable<PacienteDto>>> Get()
    {
        var pacientes = await _repo.ListarActivosAsync();
        return Ok(pacientes);
    }

    // GET /api/pacientes/id
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PacienteDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PacienteDto>> Get(int id)
    {
        var paciente = await _repo.ObtenerAsync(id);

        if (paciente is null)
        {
            return NotFound($"Paciente con ID {id} no encontrado.");
        }

        return Ok(paciente);
    }
    // GET /api/pacientes/{id}/nombre_referido
    [HttpGet("{id:int}/nombre")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PacienteBusquedaDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PacienteBusquedaDto>> GetNombrePorId(int id)
    {
        var paciente = await _repo.ObtenerNombrePorIdAsync(id);
        if (paciente is null)
        {
            return NotFound();
        }
        return Ok(paciente);
    }

    // POST /api/pacientes
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)] // Nuevo: Para errores de negocio (ej. DPI duplicado)
    public async Task<ActionResult> Post(PacienteCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // El repositorio devuelve: (ok, mensaje, id_o_codigo_error)
        var (ok, mensaje, idOrCodigoError) = await _repo.CrearAsync(dto);

        if (ok && idOrCodigoError.HasValue)
        {
            // ÉXITO: 201 Created. Obtenemos el nuevo paciente para devolverlo
            var nuevoPaciente = await _repo.ObtenerAsync(idOrCodigoError.Value);
            return CreatedAtAction(nameof(Get), new { id = idOrCodigoError.Value }, nuevoPaciente);
        }

        // FALLO: idOrCodigoError contiene el código de error (ej. 409, 500)
        if (idOrCodigoError.HasValue && idOrCodigoError.Value >= 400)
        {
            // Error de Negocio o Interno (ej. 409 Conflict, 500 Server Error)
            return StatusCode(idOrCodigoError.Value, new { Mensaje = mensaje });
        }

        // Fallo Genérico (ej. no se recibió ID del SP sin mensaje de error claro)
        return BadRequest(new { Mensaje = mensaje });
    }

    // PUT /api/pacientes/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Put(int id, PacienteUpdateDto dto)
    {
        var (ok, mensaje, codigo) = await _repo.ActualizarAsync(id, dto);

        if (ok)
        {
            return NoContent(); // 204 No Content
        }

        // Si el código es 404, devolvemos NotFound
        if (codigo == StatusCodes.Status404NotFound)
        {
            return NotFound(new { Mensaje = mensaje });
        }

        // Devolvemos el código de error de negocio devuelto (ej. 409) o 400 por defecto
        return StatusCode(codigo ?? StatusCodes.Status400BadRequest, new { Mensaje = mensaje });
    }

    // DELETE /api/pacientes/5 (Borrado Lógico/Desactivación)
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var (ok, mensaje, codigo) = await _repo.DesactivarAsync(id);

        if (ok)
        {
            return NoContent(); // 204 No Content
        }

        // Si el código es 404, devolvemos NotFound
        if (codigo == StatusCodes.Status404NotFound)
        {
            return NotFound(new { Mensaje = mensaje });
        }

        // Devolvemos el código de error de negocio devuelto o 400 por defecto
        return StatusCode(codigo ?? StatusCodes.Status400BadRequest, new { Mensaje = mensaje });
    }
}