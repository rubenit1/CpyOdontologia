namespace DTOs.PacientePreferencias;
public sealed record PacientePreferenciaDto(
    int Paciente_Id,
    int Preferencia_Id,
    string Nombre_Preferencia, // Mapea a pc.nombre
    string? Nota,
    bool Estado
);