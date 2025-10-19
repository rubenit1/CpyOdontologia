namespace DTOs.Consultorio;
public sealed class ConsultorioUpdateDto
{
    public int? Sede_Id { get; set; }             // opcional (puede no cambiar)
    public string? Nombre { get; set; }
    public bool? Estado { get; set; }
}