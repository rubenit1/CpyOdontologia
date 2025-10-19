using System.ComponentModel.DataAnnotations;

namespace DTOs.PacienteSeguro;

public sealed record PacienteSeguroCreateDto(
    [Required] int Paciente_Id,
    [Required] int Aseguradora_Id,
    [Required][MaxLength(255)] string Numero_Poliza,
    [Required][MaxLength(255)] string Titular_Poliza,
    bool Estado = true
);