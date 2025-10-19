namespace DTOs.Rol;
public sealed class RolCreateDto
{
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public bool? Estado { get; set; } // opcional; SP usa 1 por defecto
}