using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.PiezaDental;

public sealed class PiezaDentalRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_pieza_dental";

    public PiezaDentalRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(PiezaDentalCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                id = (int?)null,
                codigo_fdi = dto.Codigo_Fdi,
                nombre = dto.Nombre,
                cuadrante = dto.Cuadrante,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R (solo activos)
    public async Task<IEnumerable<PiezaDentalDto>> ListarActivasAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<PiezaDentalDto>(
            SP,
            new { opcion = 'R', id = (int?)null, codigo_fdi = (string?)null, nombre = (string?)null, cuadrante = (int?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // I (por id)
    public async Task<PiezaDentalDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<PiezaDentalDto>(
            SP,
            new { opcion = 'I', id, codigo_fdi = (string?)null, nombre = (string?)null, cuadrante = (int?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // U
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, PiezaDentalUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                id,
                codigo_fdi = dto.Codigo_Fdi,
                nombre = dto.Nombre,
                cuadrante = dto.Cuadrante,
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
            new { opcion = 'D', id, codigo_fdi = (string?)null, nombre = (string?)null, cuadrante = (int?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}