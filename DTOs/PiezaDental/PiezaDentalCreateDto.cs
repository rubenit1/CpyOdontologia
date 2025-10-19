namespace DTOs.PiezaDental;
public sealed class PiezaDentalCreateDto
{
    public string Codigo_Fdi { get; set; } = default!;
    public string? Nombre { get; set; }
    public int? Cuadrante { get; set; }
    public bool? Estado { get; set; } // opcional; SP usa 1 por defecto
}
