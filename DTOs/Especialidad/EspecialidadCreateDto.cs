namespace DTOs.Especialidad;
public sealed class EspecialidadCreateDto
{
    public string Nombre { get; set; } = default!;
    public bool? Estado { get; set; } // opcional; el SP usa 1 por defecto
}