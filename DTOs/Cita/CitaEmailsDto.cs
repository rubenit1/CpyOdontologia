public class CitaEmailDetailsDto
{
    public string PacienteEmail { get; set; }
    public string PacienteNombre { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string OdontologoNombre { get; set; }
    public string SedeNombre { get; set; }
}