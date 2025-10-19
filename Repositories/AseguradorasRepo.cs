using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Aseguradoras;

public class AseguradorasRepo : IAseguradorasRepo
{
    private readonly string _cs;
    
    public AseguradorasRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(AseguradoraCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            "dbo.sp_crud_aseguradora",
            new { opcion = 'C', id = (int?)null, nombre = dto.Nombre, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    public async Task<IEnumerable<AseguradoraDto>> ListarActivasAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<AseguradoraDto>(
            "dbo.sp_crud_aseguradora",
            new { opcion = 'R', id = (int?)null, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<AseguradoraDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<AseguradoraDto>(
            "dbo.sp_crud_aseguradora",
            new { opcion = 'I', id, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, AseguradoraUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            "dbo.sp_crud_aseguradora",
            new { opcion = 'U', id, nombre = dto.Nombre, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            "dbo.sp_crud_aseguradora",
            new { opcion = 'D', id, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}