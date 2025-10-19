namespace DTOs.Usuario;
// Para detalle (I)
public sealed class UsuarioDetailDto
{
    public int Id { get; set; }
    public string? Nombre_Usuario { get; set; }
    public string? Email { get; set; }
    public int Rol_Id { get; set; }
    public string? Rol_Nombre { get; set; }
    public bool Estado { get; set; }
    public DateTime Fecha_Creacion { get; set; }
    public DateTime? Ultimo_Acceso { get; set; }
}