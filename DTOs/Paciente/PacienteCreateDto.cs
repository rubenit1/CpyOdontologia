using System.ComponentModel.DataAnnotations;

namespace DTOs.Paciente;
public sealed class PacienteCreateDto
{
    public string Nombre { get; set; } = default!;
    public string Apellido { get; set; } = default!;
    public DateOnly FechaNacimiento { get; set; }
    public string Dpi { get; set; } = default!;
    public string? Nit { get; set; }
    public string Direccion { get; set; }
    public string Email { get; set; }
    public int Telefono { get; set; }
    public bool Estado { get; set; }
    public string? Instagram { get; set; }
    public string? Tiktok { get; set; }
    public string? Facebook { get; set; }

    // llaves foraneas
    public int GeneroId { get; set; }
    public int EstadoCivilId { get; set; }
    public int TipoSangreId { get; set; }
    public int OrigenId { get; set; }
    public int? ReferidoPorPacienteId { get; set; }
    public int SedeId { get; set; }
}