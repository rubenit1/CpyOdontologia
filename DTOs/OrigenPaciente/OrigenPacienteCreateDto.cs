namespace DTOs.OrigenPaciente;
public sealed class OrigenPacienteCreateDto
{
    public string Nombre { get; set; } = default!;
    public bool? Estado { get; set; } // opcional; SP usa 1 por defecto
}