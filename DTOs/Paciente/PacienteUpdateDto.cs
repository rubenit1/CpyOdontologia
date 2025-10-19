public sealed class PacienteUpdateDto
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public string Dpi { get; set; }
    public string? Nit { get; set; }
    public string Direccion { get; set; }
    public string Email { get; set; }
    public int Telefono { get; set; }
    public bool Estado { get; set; }
    public string? Instagram { get; set; }
    public string? Tiktok { get; set; }
    public string? Facebook { get; set; }

    // llaves foraneas

    public int GeneroId { get; set; }           
    public int EstadoCivilId { get; set; }      
    public int TipoSangreId { get; set; }       
    public int OrigenId { get; set; }         
    public int? ReferidoPorPacienteId { get; set; } 
    public int SedeId { get; set; }             
}