namespace DTOs.Sede;
public sealed class SedeCreateDto
{
    public string Nombre { get; set; } = default!;
    public string? Direccion { get; set; }
    public bool? Estado { get; set; }  // opcional; SP usa 1 por defecto
}