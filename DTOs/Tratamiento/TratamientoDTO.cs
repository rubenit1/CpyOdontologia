
namespace DTOs.Tratamiento;

public sealed record TratamientoDto(
    int Id,
    int Procedimiento_Id,
    string Nombre_Procedimiento, 
    int? Pieza_Dental_Id, 
    string? Nombre_Pieza, 
    bool Estado,
    string Estado_Descripcion, 
    string? Notas
);