namespace DTOs.MetodoPago;
public sealed class MetodoPagoCreateDto
{
    public string Nombre { get; set; } = default!;
    public bool? Estado { get; set; } // opcional; el SP usa 1 por defecto
}