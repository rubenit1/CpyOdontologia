namespace DTOs.PacienteMedicamentoActual;

public sealed record PacienteMedicamentoActualUpdateDto(
    string? Dosis,
    string? Frecuencia,
    bool? Estado
);