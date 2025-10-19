namespace DTOs.PlanTratamiento;
public sealed record PlanTratamientoUpdateDto(
    int? Paciente_Id,
    bool? Estado,
    DateTime? Validez_Hasta,
    string? Notas
);
