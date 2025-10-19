using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.MaterialEspera;

public sealed class MaterialEsperaRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_material_espera";

    public MaterialEsperaRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(MaterialEsperaCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                id = (int?)null,
                nombre = dto.Nombre,
                stock = dto.Stock,
                apto_para = dto.Apto_Para,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R (solo activos)
    public async Task<IEnumerable<MaterialEsperaDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<MaterialEsperaDto>(
            SP,
            new
            {
                opcion = 'R',
                id = (int?)null,
                nombre = (string?)null,
                stock = (int?)null,
                apto_para = (string?)null,
                estado = (bool?)null
            },
            commandType: CommandType.StoredProcedure);
    }

    // I (por id)
    public async Task<MaterialEsperaDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<MaterialEsperaDto>(
            SP,
            new
            {
                opcion = 'I',
                id,
                nombre = (string?)null,
                stock = (int?)null,
                apto_para = (string?)null,
                estado = (bool?)null
            },
            commandType: CommandType.StoredProcedure);
    }

    // U
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, MaterialEsperaUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                id,
                nombre = dto.Nombre,
                stock = dto.Stock,
                apto_para = dto.Apto_Para,
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
            new
            {
                opcion = 'D',
                id,
                nombre = (string?)null,
                stock = (int?)null,
                apto_para = (string?)null,
                estado = (bool?)null
            },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}