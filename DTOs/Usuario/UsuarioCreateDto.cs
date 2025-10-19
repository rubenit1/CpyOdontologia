namespace DTOs.Usuario;
// Crear
public sealed class UsuarioCreateDto
{
    public string Nombre_Usuario { get; set; } = default!;
    public string Password_Hash { get; set; } = default!;
    public int Rol_Id { get; set; }
    public string Email { get; set; } = default!;
    public bool? Estado { get; set; } // opcional; SP usa 1 por defecto
}