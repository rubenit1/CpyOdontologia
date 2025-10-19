namespace DTOs.MedicamentoCatalogo;
public sealed class MedicamentoCatalogoUpdateDto
{
    public string? Nombre_Generico { get; set; }
    public string? Nombre_Comercial { get; set; }
    public bool? Estado { get; set; }
}