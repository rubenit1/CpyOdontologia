using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.MedicamentoCatalogo;

public sealed class MedicamentoCatalogoRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_medicamento_catalogo";

    public MedicamentoCatalogoRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(MedicamentoCatalogoCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                id = (int?)null,
                nombre_generico = dto.Nombre_Generico,
                nombre_comercial = dto.Nombre_Comercial,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R (solo activos)
    public async Task<IEnumerable<MedicamentoCatalogoDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<MedicamentoCatalogoDto>(
            SP,
            new
            {
                opcion = 'R',
                id = (int?)null,
                nombre_generico = (string?)null,
                nombre_comercial = (string?)null,
                estado = (bool?)null
            },
            commandType: CommandType.StoredProcedure);
    }

    // I (por id)
    public async Task<MedicamentoCatalogoDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<MedicamentoCatalogoDto>(
            SP,
            new
            {
                opcion = 'I',
                id,
                nombre_generico = (string?)null,
                nombre_comercial = (string?)null,
                estado = (bool?)null
            },
            commandType: CommandType.StoredProcedure);
    }

    // U
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, MedicamentoCatalogoUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                id,
                nombre_generico = dto.Nombre_Generico,
                nombre_comercial = dto.Nombre_Comercial,
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
                nombre_generico = (string?)null,
                nombre_comercial = (string?)null,
                estado = (bool?)null
            },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}