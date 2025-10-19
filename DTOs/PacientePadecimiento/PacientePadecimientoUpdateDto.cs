namespace DTOs.PacientePadecimiento;
public sealed record PacientePadecimientoUpdateDto(
    string? Observacion, 
    bool? Estado
);