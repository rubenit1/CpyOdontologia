namespace DTOs.PacienteAlergiaMed;
public sealed record PacienteAlergiaMedUpdateDto(
    string? ReaccionObservada, // Permitir nulo para no actualizar
    bool? Estado // Permitir nulo para no actualizar
);