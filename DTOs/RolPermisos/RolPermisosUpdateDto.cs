namespace DTOs.RolPermisos;
public sealed class RolPermisosUpdateDto
{
    public int RolId { get; set; }
    public IReadOnlyCollection<int> PermisosIds { get; set; } = Array.Empty<int>();
}