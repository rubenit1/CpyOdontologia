namespace DTOs.MotivoCita;
public sealed class MotivoCitaCreateDto
{
    public string Nombre { get; set; } = default!;
    public bool? Estado { get; set; } // opcional; el SP usa 1 por defecto
}