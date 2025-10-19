using System.ComponentModel.DataAnnotations;

namespace DTOs.PacienteSeguro;

public sealed record PacienteSeguroUpdateDto(
    int? Paciente_Id, 
    int? Aseguradora_Id,
    [MaxLength(255)] string? Numero_Poliza,
    [MaxLength(255)] string? Titular_Poliza,
    bool? Estado
);