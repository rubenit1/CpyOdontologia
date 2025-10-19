using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.PacienteAlergiaMed;

public sealed class PacienteAlergiaMedRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_paciente_alergia_medicamento";

    public PacienteAlergiaMedRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    // Record para la salida de operaciones C, U, D
    private sealed record ProcMsg(string Mensaje, int? Codigo);

    // Helper: Mapea mensajes de éxito/error (Usado por C, U, D)
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        // pm.Codigo >= 400 indica error (400, 404, 409, 500).
        => pm is null
            ? (true, "Operación exitosa.", 200)
            : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // ✅ Listar (R): Usa QueryAsync<DTO> para listados. ESTO ARREGLA EL ERROR 500 EN GET.
    public async Task<IEnumerable<PacienteAlergiaMedDto>> ListarPorPacienteAsync(int pacienteId)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<PacienteAlergiaMedDto>(
            SP,
            new { opcion = 'R', paciente_id = pacienteId },
            commandType: CommandType.StoredProcedure);
    }

    // ✅ Crear (C): Usa ProcMsg para mensajes de éxito/error. ESTO ARREGLA EL ERROR 500 EN POST.
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(PacienteAlergiaMedCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                paciente_id = dto.PacienteId,
                medicamento_id = dto.MedicamentoId,
                reaccion_observada = dto.ReaccionObservada,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);

        return Map(res);
    }

    // Actualizar (U): Usa ProcMsg
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int pacienteId, int medicamentoId, PacienteAlergiaMedUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                paciente_id = pacienteId,
                medicamento_id = medicamentoId,
                reaccion_observada = dto.ReaccionObservada,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);
        return Map(res);
    }

    // Desactivar (D): Usa ProcMsg
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int pacienteId, int medicamentoId)
    {
        using var conn = new SqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', paciente_id = pacienteId, medicamento_id = medicamentoId },
            commandType: CommandType.StoredProcedure);
        return Map(res);
    }
}