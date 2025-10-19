namespace DTOs.MaterialEspera;
public sealed class MaterialEsperaDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public int? Stock { get; set; }
    public string? Apto_Para { get; set; } // mapea a apto_para
    public bool Estado { get; set; }
}