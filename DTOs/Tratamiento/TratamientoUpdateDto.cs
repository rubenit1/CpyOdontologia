namespace DTOs.Tratamiento;

public sealed record TratamientoUpdateDto(
    int? Procedimiento_Id,
    int? Pieza_Dental_Id,
    string? Notas,
    bool? Estado
);