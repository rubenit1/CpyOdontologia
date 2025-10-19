using AppOdontologia.Services;
using DTOs.Cita;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CitaController : ControllerBase
{
    private readonly CitaRepo _repo;
    private readonly MailService _mailService;

    public CitaController(CitaRepo repo, MailService mailService)
    {
        _repo = repo;
        _mailService = mailService;
    }

    // GET /api/cita
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CitaDto>>> Get(
        [FromQuery] int? id,
        [FromQuery] int? paciente_id,
        [FromQuery] int? odontologo_id,
        [FromQuery] int? sede_id)
        => Ok(await _repo.ListarAsync(id, paciente_id, odontologo_id, sede_id));

    // GET /api/cita/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CitaDto>> Get(int id)
        => (await _repo.ObtenerAsync(id)) is { } x ? Ok(x) : NotFound();

    // POST /api/cita
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CitaCreateDto dto)
    {
        var (ok, msg, idOrCode) = await _repo.CrearAsync(dto);

        if (ok && idOrCode.HasValue)
            return CreatedAtAction(nameof(Get), new { id = idOrCode.Value }, new { mensaje = msg, id = idOrCode.Value });

        return ToHttp(idOrCode, msg);
    }

    // PUT /api/cita/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] CitaUpdateDto dto)
    {
        var (ok, msg, code) = await _repo.ActualizarAsync(id, dto);
        if (!ok)
        {
            return ToHttp(code, msg);
        }

        string? errorCorreo = null;
        bool emailEnviado = false;
        var datosCita = await _repo.ObtenerDatosParaCorreoAsync(id);

        if (datosCita is not null)
        {
            try
            {
                await _mailService.SendUpdateNotificationAsync(
                    datosCita.PacienteEmail,
                    datosCita.PacienteNombre,
                    datosCita.Fecha,
                    datosCita.HoraInicio,
                    datosCita.HoraFin,
                    datosCita.OdontologoNombre,
                    datosCita.SedeNombre
                );
                emailEnviado = true;
            }
            catch (Exception ex)
            {
                errorCorreo = ex.Message;
            }
        }
        return Ok(new { mensaje = "Cita actualizada con éxito.", emailEnviado, errorCorreo });
    }

    // ---  MÉTODO DELETE MODIFICADO ---
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // 1. Obtener datos antes de cancelar
        var datosCita = await _repo.ObtenerDatosParaCorreoAsync(id);
        if (datosCita is null)
        {
            return NotFound(new { mensaje = "La cita que intentas cancelar no existe." });
        }

        // 2. Cancelar la cita en la base de datos
        var (ok, msg, code) = await _repo.CancelarAsync(id);
        if (!ok)
        {
            return ToHttp(code, msg);
        }

        // 3. Notificar al paciente (si la cancelación fue exitosa)
        try
        {
            await _mailService.SendCancellationNotificationAsync(
                datosCita.PacienteEmail,
                datosCita.PacienteNombre,
                datosCita.Fecha,
                datosCita.HoraInicio
            );
        }
        catch (Exception ex)
        {
            // Opcional: Registrar el error en un sistema de logs
            Console.WriteLine($"FALLO AL ENVIAR CORREO DE CANCELACIÓN para cita {id}: {ex.Message}");
        }

        // 4. Responder al frontend
        return NoContent();
    }

    // --- Endpoints de confirmación y reagendamiento (sin cambios) ---

    [HttpPost("crear_con_confirmacion")]
    public async Task<IActionResult> CrearConConfirmacion([FromBody] CitaCreateDto dto)
    {
        var (ok, mensaje, nuevaCitaId) = await _repo.CrearAsync(dto);
        if (!ok || !nuevaCitaId.HasValue)
            return BadRequest(new { mensaje });

        var datosCita = await _repo.ObtenerDatosParaCorreoAsync(nuevaCitaId.Value);
        bool emailEnviado = false;
        string? errorCorreo = null;

        if (datosCita is not null)
        {
            try
            {
                await _mailService.SendAppointmentConfirmationAsync(
                    datosCita.PacienteEmail, datosCita.PacienteNombre, datosCita.Fecha,
                    datosCita.HoraInicio, datosCita.HoraFin, datosCita.OdontologoNombre, datosCita.SedeNombre
                );
                emailEnviado = true;
            }
            catch (Exception ex) { errorCorreo = ex.Message; }
        }
        return StatusCode(201, new { id = nuevaCitaId.Value, emailEnviado, errorCorreo });
    }

    [HttpPost("{id:int}/reagendar_con_notificacion")]
    public async Task<IActionResult> ReagendarConNotificacion(int id, [FromBody] CitaUpdateDto dto)
    {
        var (ok, msg, code) = await _repo.ActualizarAsync(id, dto);
        if (!ok) return ToHttp(code, msg);

        var datosCita = await _repo.ObtenerDatosParaCorreoAsync(id);
        bool emailEnviado = false;
        string? errorCorreo = null;

        if (datosCita is not null)
        {
            try
            {
                await _mailService.SendRescheduleNotificationAsync(
                    datosCita.PacienteEmail, datosCita.PacienteNombre, datosCita.Fecha,
                    datosCita.HoraInicio, datosCita.HoraFin, datosCita.OdontologoNombre, datosCita.SedeNombre
                );
                emailEnviado = true;
            }
            catch (Exception ex) { errorCorreo = ex.Message; }
        }
        return Ok(new { mensaje = "Cita reagendada con éxito.", emailEnviado, errorCorreo });
    }

    [HttpGet("canceladas")]
    public async Task<ActionResult<IEnumerable<CitaDto>>> GetCanceladas()
    {
        // ID del estado "Cancelada"
        const int ESTADO_CANCELADA = 4;

        // Reutilizamos tu método de listado, pasándole el filtro de estado
        var citasCanceladas = await _repo.ListarAsync(estado_id: ESTADO_CANCELADA);

        return Ok(citasCanceladas);
    }

    // --- Endpoints de diagnóstico (sin cambios) ---
    [HttpPost("test_correo")]
    public async Task<IActionResult> TestCorreo([FromQuery] string to)
    {
        try
        {
            var fecha = DateTime.UtcNow.Date;
            var inicio = new TimeSpan(15, 0, 0);
            var fin = new TimeSpan(15, 30, 0);
            await _mailService.SendAppointmentConfirmationAsync(to, "Prueba SMTP", fecha, inicio, fin, "Dra. Prueba", "Sede Prueba");
            return Ok(new { enviado = true, a = to });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { enviado = false, tipo = ex.GetType().FullName, error = ex.Message });
        }
    }

    [HttpGet("diag_mail")]
    public IActionResult DiagMail([FromServices] IOptions<MailSettings> opt)
    {
        var s = opt.Value;
        return Ok(new { s.Mail, s.DisplayName, s.Host, s.Port, PasswordSet = !string.IsNullOrWhiteSpace(s.Password) });
    }

    [HttpPut("{id:int}/seguimiento")]
    public async Task<IActionResult> PutSeguimiento(int id, [FromBody] CitaSeguimientoUpdateDto dto)
    {
        // NOTA: Deberás crear este método en tu CitaRepo.cs
        var (ok, msg, code) = await _repo.ActualizarSeguimientoAsync(id, dto.Estado_Seguimiento, dto.Notas_Seguimiento);

        return ok ? Ok(new { mensaje = msg }) : ToHttp(code, msg);
    }

    private ObjectResult ToHttp(int? codigo, string mensaje) => codigo switch
    {
        404 => NotFound(new { mensaje }),
        409 => Conflict(new { mensaje }),
        500 => StatusCode(500, new { mensaje }),
        _ => BadRequest(new { mensaje })
    };
}