using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Procedimiento;

public sealed class ProcedimientoRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_procedimiento";

    public ProcedimientoRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(ProcedimientoCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                id = (int?)null,
                nombre = dto.Nombre,
                descripcion = dto.Descripcion,
                duracion_estimada_min = dto.Duracion_Estimada_Min,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R (solo activos)
    public async Task<IEnumerable<ProcedimientoDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<ProcedimientoDto>(
            SP,
            new { opcion = 'R', id = (int?)null, nombre = (string?)null, descripcion = (string?)null, duracion_estimada_min = (int?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // I (por id)
    public async Task<ProcedimientoDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<ProcedimientoDto>(
            SP,
            new { opcion = 'I', id, nombre = (string?)null, descripcion = (string?)null, duracion_estimada_min = (int?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // U
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, ProcedimientoUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                id,
                nombre = dto.Nombre,
                descripcion = dto.Descripcion,
                duracion_estimada_min = dto.Duracion_Estimada_Min,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // D (soft delete → estado=0)
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id, nombre = (string?)null, descripcion = (string?)null, duracion_estimada_min = (int?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}