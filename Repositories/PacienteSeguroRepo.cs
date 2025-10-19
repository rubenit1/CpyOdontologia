using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.PacienteSeguro;

public sealed class PacienteSeguroRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_paciente_seguro";

    public PacienteSeguroRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    // Tipo de respuesta
    private sealed record ProcMsg(string Mensaje, int? Codigo);

    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null
            ? (true, "Operación exitosa.", 200)
            : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    public async Task<IEnumerable<PacienteSeguroDto>> ListarAsync(int? pacienteId)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<PacienteSeguroDto>(
            SP,
            new { opcion = 'R', paciente_id = pacienteId },
            commandType: CommandType.StoredProcedure);
    }
    public async Task<PacienteSeguroDto?> LeerIndividualAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<PacienteSeguroDto>(
            SP,
            new { opcion = 'I', id = id },
            commandType: CommandType.StoredProcedure);
    }
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(PacienteSeguroCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                paciente_id = dto.Paciente_Id,
                aseguradora_id = dto.Aseguradora_Id,
                numero_poliza = dto.Numero_Poliza,
                titular_poliza = dto.Titular_Poliza,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);

        return Map(res);
    }
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, PacienteSeguroUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                id = id,
                paciente_id = dto.Paciente_Id,
                aseguradora_id = dto.Aseguradora_Id,
                numero_poliza = dto.Numero_Poliza,
                titular_poliza = dto.Titular_Poliza,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);

        return Map(res);
    }
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id = id },
            commandType: CommandType.StoredProcedure);

        return Map(res);
    }
}