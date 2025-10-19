namespace DTOs.Odontologo;
public sealed class OdontologoDto
{
    public int Id { get; set; }
    public int Usuario_Id { get; set; }     // mapea a usuario_id
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Matricula { get; set; }
    public bool Estado { get; set; }
}