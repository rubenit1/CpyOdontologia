namespace DTOs.Paciente;
public sealed class PacienteDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Dpi { get; set; }
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Estado { get; set; }
    public string? Instagram { get; set; }
    public string? Tiktok { get; set; }
    public string? Facebook { get; set; }

    // CAMPOS DE LLAVES FORÁNEAS (FKs) Y SUS NOMBRES DESCRIPTIVOS
    public int? GeneroId { get; set; }
    public string? GeneroNombre { get; set; }

    public int? EstadoCivilId { get; set; }
    public string? EstadoCivilNombre { get; set; } // Opciones R - I

    public int? TipoSangreId { get; set; }
    public string? TipoSangreNombre { get; set; } // Opcion R - I

    public int? OrigenId { get; set; }           // Opción I
    public string? OrigenNombre { get; set; }   // Opción I

    public int? ReferidoPorPacienteId { get; set; } // Opción I
    public string? ReferidoPorNombre { get; set; }
    public int? DebugIdQueBusco { get; set; }
    public int? DebugReferidoEncontrado { get; set; }
    public int? SedeId { get; set; }
    public string? SedeNombre { get; set; }      // Opcion R - I
}