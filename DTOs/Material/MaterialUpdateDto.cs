namespace DTOs.Material;
public sealed class MaterialUpdateDto
{
    public string? Nombre { get; set; }
    public bool? Estado { get; set; }
    public int? Calidad_Id { get; set; }       // opcional (puede no cambiar)
}