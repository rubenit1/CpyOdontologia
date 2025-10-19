namespace DTOs.PiezaDental;
public sealed class PiezaDentalDto
{
    public int Id { get; set; }
    public string? Codigo_Fdi { get; set; }  // mapea a codigo_fdi
    public string? Nombre { get; set; }
    public int? Cuadrante { get; set; }
    public bool Estado { get; set; }
}
