namespace DTOs.PreferenciaCatalogo;
public sealed class PreferenciaCatalogoCreateDto
{
    public string Nombre { get; set; } = default!;
    public string? Tipo { get; set; }
    public bool? Estado { get; set; } // opcional; el SP usa 1 por defecto
}