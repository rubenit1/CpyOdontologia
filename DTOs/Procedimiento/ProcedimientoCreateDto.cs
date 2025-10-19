namespace DTOs.Procedimiento;
public sealed class ProcedimientoCreateDto
{
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public int? Duracion_Estimada_Min { get; set; }
    public bool? Estado { get; set; } // opcional; SP usa 1 por defecto
}