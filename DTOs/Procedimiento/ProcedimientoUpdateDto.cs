namespace DTOs.Procedimiento;
public sealed class ProcedimientoUpdateDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int? Duracion_Estimada_Min { get; set; }
    public bool? Estado { get; set; }
}