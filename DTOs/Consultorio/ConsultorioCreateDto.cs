namespace DTOs.Consultorio;
public sealed class ConsultorioCreateDto
{
    public int Sede_Id { get; set; }              // requerido por el SP
    public string Nombre { get; set; } = default!;
    public bool? Estado { get; set; }             // opcional; SP usa 1 por defecto
}