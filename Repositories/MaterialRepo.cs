using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Material;

public sealed class MaterialRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_material";

    public MaterialRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(MaterialCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'C', id = (int?)null, nombre = dto.Nombre, estado = dto.Estado, calidad_id = dto.Calidad_Id },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R (solo activos)
    public async Task<IEnumerable<MaterialDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<MaterialDto>(
            SP,
            new { opcion = 'R', id = (int?)null, nombre = (string?)null, estado = (bool?)null, calidad_id = (int?)null },
            commandType: CommandType.StoredProcedure);
    }

    // I (por id)
    public async Task<MaterialDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<MaterialDto>(
            SP,
            new { opcion = 'I', id, nombre = (string?)null, estado = (bool?)null, calidad_id = (int?)null },
            commandType: CommandType.StoredProcedure);
    }

    // U
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, MaterialUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'U', id, nombre = dto.Nombre, estado = dto.Estado, calidad_id = dto.Calidad_Id },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // D (soft delete → estado=0)
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id, nombre = (string?)null, estado = (bool?)null, calidad_id = (int?)null },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}