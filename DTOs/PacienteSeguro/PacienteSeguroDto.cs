namespace DTOs.PacienteSeguro;

public sealed record PacienteSeguroDto(
    int Id,
    int Paciente_Id,
    int Aseguradora_Id,
    string Aseguradora_Nombre, 
    string Numero_Poliza,
    string Titular_Poliza,
    bool Estado
);