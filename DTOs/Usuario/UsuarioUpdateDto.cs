namespace DTOs.Usuario;
// Actualizar (todos opcionales)
public sealed class UsuarioUpdateDto
{
    public string? Nombre_Usuario { get; set; }
    public string? Password_Hash { get; set; }
    public int? Rol_Id { get; set; }
    public string? Email { get; set; }
    public bool? Estado { get; set; }
}