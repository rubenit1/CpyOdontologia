namespace DTOs.PacientePadecimiento;
public sealed record PacientePadecimientoDto(
    int Paciente_Id,
    int Padecimiento_Id,
    string Nombre_Padecimiento,
    string? Observacion,
    bool Estado
);