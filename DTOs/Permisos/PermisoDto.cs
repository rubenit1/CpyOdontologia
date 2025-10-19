namespace DTOs.Permisos;
public sealed class PermisoDto
{
    public int Id { get; set; }
    public string? PermisoNombre { get; set; }
    public string? Url { get; set; }
    public string? Icono { get; set; } // Nuevo Campo para el ícono
}