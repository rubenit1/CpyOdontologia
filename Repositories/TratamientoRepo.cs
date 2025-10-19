using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Tratamiento;

public sealed class TratamientoRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_tratamiento";

    public TratamientoRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;
    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is not null
            ? (pm.Codigo is < 400, pm.Mensaje, pm.Codigo)
            : (false, "Error desconocido en la ejecución del procedimiento.", 500);

    public async Task<IEnumerable<TratamientoDto>> ListarAsync(int? id = null, int? procedimientoId = null, bool? estado = null)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<TratamientoDto>(
            SP,
            new { opcion = 'R', id = id, procedimiento_id = procedimientoId, estado = estado },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(TratamientoCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);

        var parameters = new DynamicParameters();
        parameters.Add("@opcion", 'C', DbType.AnsiStringFixedLength, size: 1);
        parameters.Add("@procedimiento_id", dto.Procedimiento_Id, DbType.Int32);
        parameters.Add("@pieza_dental_id", dto.Pieza_Dental_Id, DbType.Int32);
        parameters.Add("@notas", dto.Notas, DbType.String);
        parameters.Add("@estado", dto.Estado, DbType.Boolean);

        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            parameters, 
            commandType: CommandType.StoredProcedure);

        return Map(res);
    }

    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, TratamientoUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);

        var parameters = new DynamicParameters();
        parameters.Add("@opcion", 'U', DbType.AnsiStringFixedLength, size: 1);
        parameters.Add("@id", id, DbType.Int32);
        parameters.Add("@procedimiento_id", dto.Procedimiento_Id, DbType.Int32);
        parameters.Add("@pieza_dental_id", dto.Pieza_Dental_Id, DbType.Int32);

        parameters.Add("@notas", dto.Notas, DbType.String);
        parameters.Add("@estado", dto.Estado, DbType.Boolean);

        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            parameters, 
            commandType: CommandType.StoredProcedure);

        return Map(res);
    }

    public async Task<(bool ok, string mensaje, int? codigo)> AnularAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id = id },
            commandType: CommandType.StoredProcedure);

        return Map(res);
    }
}