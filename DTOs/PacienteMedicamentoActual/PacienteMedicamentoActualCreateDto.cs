namespace DTOs.PacienteMedicamentoActual;

using System.ComponentModel.DataAnnotations;

// DTO para la opción 'C' (Crear)
public sealed record PacienteMedicamentoActualCreateDto(
    [Required] int PacienteId,
    [Required] int MedicamentoId,
    string? Dosis,
    string? Frecuencia,
    bool Estado = true
);