namespace DTOs.PacientePreferencias;
using System.ComponentModel.DataAnnotations;

public sealed record PacientePreferenciaCreateDto(
    [Required] int PacienteId,
    [Required] int PreferenciaId,
    [StringLength(255)] string? Nota,
    bool Estado = true
);