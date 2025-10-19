namespace DTOs.MedicamentoCatalogo;
public sealed class MedicamentoCatalogoCreateDto
{
    public string Nombre_Generico { get; set; } = default!;
    public string? Nombre_Comercial { get; set; }
    public bool? Estado { get; set; } // opcional; SP usa 1 por defecto
}