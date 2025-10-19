namespace DTOs.ResultadoConfirmacion;
public sealed class ResultadoConfirmacionCreateDto
{
    public string Nombre { get; set; } = default!;
    public bool? Estado { get; set; } // opcional; el SP usa 1 por defecto
}
