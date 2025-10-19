namespace DTOs.Material;
public sealed class MaterialCreateDto
{
    public string Nombre { get; set; } = default!;
    public bool? Estado { get; set; }          // opcional; SP usa 1 por defecto
    public int? Calidad_Id { get; set; }       // opcional
}