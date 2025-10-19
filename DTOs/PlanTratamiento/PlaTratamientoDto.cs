namespace DTOs.PlanTratamiento;

public sealed record PlanTratamientoDto(
    int Id,
    int Paciente_Id,
    string Nombre_Paciente,
    int Creado_Por_Usuario_Id,
    string Nombre_Usuario_Creador,
    DateTime Fecha_Creacion,
    bool Estado,
    string Estado_Descripcion,
    DateTime? Validez_Hasta,
    string? Notas
);
