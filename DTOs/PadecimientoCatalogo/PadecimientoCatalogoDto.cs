namespace DTOs.PadecimientoCatalogo;
public sealed class PadecimientoCatalogoDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; } // VARCHAR(MAX) en BD
    public bool Estado { get; set; }
}