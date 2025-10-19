// DTOs/Cita/CitaDto.cs

namespace DTOs.Cita
{
    public class CitaDto
    {
        public int Id { get; set; }
        public int Paciente_Id { get; set; }
        public string? Nombre_Paciente { get; set; }
        public int Odontologo_Id { get; set; }
        public string? Nombre_Odontologo { get; set; }
        public int Sede_Id { get; set; }
        public string? Nombre_Sede { get; set; }
        public int Consultorio_Id { get; set; }
        public string? Nombre_Consultorio { get; set; }
        public DateTime Fecha_Hora_Inicio { get; set; }
        public DateTime Fecha_Hora_Fin { get; set; }
        public int? Motivo_Id { get; set; }
        public int Estado_Id { get; set; }
        public string? Estado_Cita { get; set; }
        public string? Notas { get; set; }
        public string? Gcal_Event_Id { get; set; }
    }
}