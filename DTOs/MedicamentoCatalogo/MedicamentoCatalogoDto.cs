namespace DTOs.MedicamentoCatalogo;
public sealed class MedicamentoCatalogoDto
{
    public int Id { get; set; }
    public string? Nombre_Generico { get; set; }    // mapea a nombre_generico
    public string? Nombre_Comercial { get; set; }   // mapea a nombre_comercial
    public bool Estado { get; set; }
}