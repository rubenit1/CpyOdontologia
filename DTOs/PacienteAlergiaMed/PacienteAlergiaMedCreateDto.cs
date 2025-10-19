namespace DTOs.PacienteAlergiaMed;

using System.ComponentModel.DataAnnotations;

public sealed record PacienteAlergiaMedCreateDto(
    [Required] int PacienteId,
    [Required] int MedicamentoId,
    string? ReaccionObservada,
    bool Estado = true // Opcional, por defecto true
);