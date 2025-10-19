namespace AppOdontologia.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rol { get; set; } = "Usuario";
    }
}
