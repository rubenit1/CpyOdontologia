namespace DTOs.PacienteMedicamentoActual;
    public sealed record PacienteMedicamentoActualDto(
        int Paciente_Id,
        int Medicamento_Id,
        string Nombre_Medicamento,
        string? Dosis,
        string? Frecuencia,
        bool Estado
    );