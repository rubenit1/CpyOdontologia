using System.ComponentModel.DataAnnotations;

namespace DTOs.Tratamiento;

public sealed record TratamientoCreateDto(
    [Required] int Procedimiento_Id,
    int? Pieza_Dental_Id, 
    string? Notas,
    bool Estado = true
);