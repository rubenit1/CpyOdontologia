namespace DTOs.Material;
public sealed class MaterialDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public bool Estado { get; set; }
    public int? Calidad_Id { get; set; } // FK opcional
}