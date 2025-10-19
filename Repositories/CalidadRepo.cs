using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Calidad;

public sealed class CalidadRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_calidad";

    public CalidadRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    // Mapeo estándar (reutiliza el patrón que ya usaste)
    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C: crear
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(CalidadCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'C', id = (int?)null, nombre = dto.Nombre, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R: listar activas
    public async Task<IEnumerable<CalidadDto>> ListarActivasAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<CalidadDto>(
            SP,
            new { opcion = 'R', id = (int?)null, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // I: obtener por id
    public async Task<CalidadDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<CalidadDto>(
            SP,
            new { opcion = 'I', id, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // U: actualizar
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, CalidadUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'U', id, nombre = dto.Nombre, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // D: desactivar (soft delete)
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}