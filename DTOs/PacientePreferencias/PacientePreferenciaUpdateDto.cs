namespace DTOs.PacientePreferencias;
using System.ComponentModel.DataAnnotations;

public sealed record PacientePreferenciaUpdateDto(
    [StringLength(255)] string? Nota, // Corresponde al parámetro @nota
    bool? Estado
);