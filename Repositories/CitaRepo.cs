using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Cita;


public sealed class CitaRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_cita";

    public CitaRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    // Helper interno para mapear la respuesta del SP
    private sealed class ProcMsg
    {
        public string Mensaje { get; set; } = string.Empty;
        public int? Id { get; set; }      // Devuelto en CREATE
        public int? Codigo { get; set; }  // Devuelto en errores
    }

    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
    {
        if (pm == null)
            return (false, "Error interno: sin respuesta del procedimiento almacenado.", 500);

        // Si el SP devolvió un código >= 400, es error
        if (pm.Codigo.HasValue && pm.Codigo >= 400)
            return (false, pm.Mensaje, pm.Codigo);

        // Si devolvió un Id (CREATE)
        if (pm.Id.HasValue)
            return (true, pm.Mensaje, pm.Id);

        // Si solo hay mensaje, se considera éxito general (UPDATE o DELETE)
        return (true, pm.Mensaje, 200);
    }

    // C: Crear
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(CitaCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);

        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                paciente_id = dto.Paciente_Id,
                odontologo_id = dto.Odontologo_Id,
                sede_id = dto.Sede_Id,
                consultorio_id = dto.Consultorio_Id,
                fecha_hora_inicio = dto.Fecha_Hora_Inicio,
                fecha_hora_fin = dto.Fecha_Hora_Fin,
                motivo_id = dto.Motivo_Id,
                estado_id = dto.Estado_Id,
                creada_por_usuario_id = dto.Creada_Por_Usuario_Id,
                gcal_event_id = dto.Gcal_Event_Id,
                notas = dto.Notas
            },
            commandType: CommandType.StoredProcedure
        );

        return Map(pm);
    }

    // R: Listar (con filtros opcionales)
    public async Task<IEnumerable<CitaDto>> ListarAsync(
        int? id = null,
        int? paciente_id = null,
        int? odontologo_id = null,
        int? sede_id = null,
        int? estado_id = null)
    {
        using var conn = new SqlConnection(_cs);

        return await conn.QueryAsync<CitaDto>(
            SP,
            new
            {
                opcion = 'R',
                id,
                paciente_id,
                odontologo_id,
                sede_id,
                estado_id
            },
            commandType: CommandType.StoredProcedure
        );
    }

    // R: Obtener por ID
    public async Task<CitaDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);

        return await conn.QueryFirstOrDefaultAsync<CitaDto>(
            SP,
            new { opcion = 'R', id },
            commandType: CommandType.StoredProcedure
        );
    }

    // U: Actualizar
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, CitaUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);

        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                id,
                paciente_id = dto.Paciente_Id,
                odontologo_id = dto.Odontologo_Id,
                sede_id = dto.Sede_Id,
                consultorio_id = dto.Consultorio_Id,
                fecha_hora_inicio = dto.Fecha_Hora_Inicio,
                fecha_hora_fin = dto.Fecha_Hora_Fin,
                motivo_id = dto.Motivo_Id,
                estado_id = dto.Estado_Id,
                gcal_event_id = dto.Gcal_Event_Id,
                notas = dto.Notas
            },
            commandType: CommandType.StoredProcedure
        );

        return Map(pm);
    }

    public async Task<CitaEmailDetailsDto?> ObtenerDatosParaCorreoAsync(int citaId)
    {
        using var conn = new SqlConnection(_cs);
        var parameters = new { cita_id = citaId };
        return await conn.QueryFirstOrDefaultAsync<CitaEmailDetailsDto>(
            "sp_get_cita_details_for_email",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }
    
    //ActualizarSeguimiento
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarSeguimientoAsync(int id, string estadoSeguimiento, string notasSeguimiento)
    {
        using var conn = new SqlConnection(_cs);

        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'F',
                id,
                estado_seguimiento = estadoSeguimiento,
                notas_seguimiento = notasSeguimiento
            },
            commandType: CommandType.StoredProcedure
        );

        return Map(pm);
    }
    // D: Cancelar (soft delete)
    public async Task<(bool ok, string mensaje, int? codigo)> CancelarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);

        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'D',
                id
            },
            commandType: CommandType.StoredProcedure
        );

        return Map(pm);
    }
}
