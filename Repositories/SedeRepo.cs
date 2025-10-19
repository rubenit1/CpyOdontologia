using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Sede;

public sealed class SedeRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_sede";

    public SedeRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C: crear
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(SedeCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'C', id = (int?)null, nombre = dto.Nombre, direccion = dto.Direccion, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R: listar activas
    public async Task<IEnumerable<SedeDto>> ListarActivasAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<SedeDto>(
            SP,
            new { opcion = 'R', id = (int?)null, nombre = (string?)null, direccion = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // I: obtener por id
    public async Task<SedeDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<SedeDto>(
            SP,
            new { opcion = 'I', id, nombre = (string?)null, direccion = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // U: actualizar
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, SedeUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'U', id, nombre = dto.Nombre, direccion = dto.Direccion, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // D: desactivar (soft delete)
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id, nombre = (string?)null, direccion = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}