// DTOs/Cita/CitaCreateDto.cs

using System.ComponentModel.DataAnnotations;

namespace DTOs.Cita
{
    public class CitaCreateDto
    {
        [Required] public int Paciente_Id { get; set; }
        [Required] public int Odontologo_Id { get; set; }
        [Required] public int Sede_Id { get; set; }
        [Required] public int Consultorio_Id { get; set; }
        [Required] public DateTime Fecha_Hora_Inicio { get; set; }
        [Required] public DateTime Fecha_Hora_Fin { get; set; }
        [Required] public int Motivo_Id { get; set; }
        [Required] public int Estado_Id { get; set; }
        public int? Creada_Por_Usuario_Id { get; set; }
        public string? Gcal_Event_Id { get; set; }
        public string? Notas { get; set; }
    }
}