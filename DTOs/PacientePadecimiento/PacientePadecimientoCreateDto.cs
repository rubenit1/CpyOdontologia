namespace DTOs.PacientePadecimiento;

using System.ComponentModel.DataAnnotations;
public sealed record PacientePadecimientoCreateDto(
    [Required] int PacienteId,
    [Required] int PadecimientoId,
    string? Observacion,
    bool Estado = true
);