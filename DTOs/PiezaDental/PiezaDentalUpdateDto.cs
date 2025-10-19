namespace DTOs.PiezaDental;
public sealed class PiezaDentalUpdateDto
{
    public string? Codigo_Fdi { get; set; }
    public string? Nombre { get; set; }
    public int? Cuadrante { get; set; }
    public bool? Estado { get; set; }
}