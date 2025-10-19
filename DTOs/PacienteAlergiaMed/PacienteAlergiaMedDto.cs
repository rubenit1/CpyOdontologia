namespace DTOs.PacienteAlergiaMed;
public sealed record PacienteAlergiaMedDto(
    int Paciente_Id,
    int Medicamento_Id,
    string Nombre_Medicamento, // Viene del JOIN en el SP
    string? Reaccion_Observada,
    bool Estado
);