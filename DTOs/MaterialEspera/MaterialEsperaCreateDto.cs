namespace DTOs.MaterialEspera;
public sealed class MaterialEsperaCreateDto
{
    public string Nombre { get; set; } = default!;
    public int? Stock { get; set; }
    public string? Apto_Para { get; set; }
    public bool? Estado { get; set; } // opcional; SP usa 1 por defecto
}