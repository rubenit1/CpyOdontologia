namespace DTOs.MaterialEspera;
public sealed class MaterialEsperaUpdateDto
{
    public string? Nombre { get; set; }
    public int? Stock { get; set; }
    public string? Apto_Para { get; set; }
    public bool? Estado { get; set; }
}