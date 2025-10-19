namespace DTOs.Odontologo;

// Para crear: listas de ids obligatorias (el SP las usa para insertar en tablas puente)
public sealed class OdontologoCreateDto
{
    public int Usuario_Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Apellido { get; set; } = default!;
    public string Matricula { get; set; } = default!;
    public bool? Estado { get; set; }

    // IDs de especialidades y sedes asignadas
    public IEnumerable<int> Especialidades { get; set; } = Array.Empty<int>();
    public IEnumerable<int> Sedes { get; set; } = Array.Empty<int>();
}