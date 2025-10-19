namespace DTOs.Odontologo;

// Para actualizar: campos y listas opcionales.
    // OJO: si envías lista vacía → el SP desactiva TODAS; si no deseas cambios, envía null.
public sealed class OdontologoUpdateDto
{
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Matricula { get; set; }
    public bool? Estado { get; set; }

    public IEnumerable<int>? Especialidades { get; set; } // null = no tocar; vacía = desactivar todas
    public IEnumerable<int>? Sedes { get; set; }          // null = no tocar; vacía = desactivar todas
}