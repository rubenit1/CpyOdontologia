using System.ComponentModel.DataAnnotations;

namespace DTOs.PlanTratamiento;

public sealed record PlanTratamientoCreateDto(
    [Required(ErrorMessage = "El ID del Paciente es obligatorio.")]
    int Paciente_Id,
    // NOTA: El ID del Usuario creador se obtendrá automáticamente del token JWT del servidor
    DateTime? Validez_Hasta,
    string? Notas,
    bool Estado = true
);
