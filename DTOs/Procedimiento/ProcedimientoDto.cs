namespace DTOs.Procedimiento;
public sealed class ProcedimientoDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }           // VARCHAR(MAX) en BD
    public int? Duracion_Estimada_Min { get; set; }    // coincide con columna BD
    public bool Estado { get; set; }
}