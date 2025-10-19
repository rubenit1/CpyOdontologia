namespace DTOs.Consultorio;
public sealed class ConsultorioDto
{
    public int Id { get; set; }
    public int Sede_Id { get; set; }  // coincide con columna 'sede_id'
    public string? Nombre { get; set; }
    public bool Estado { get; set; }
}