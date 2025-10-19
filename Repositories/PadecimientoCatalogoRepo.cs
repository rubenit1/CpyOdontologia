using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.PadecimientoCatalogo;

public sealed class PadecimientoCatalogoRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_padecimiento_catalogo";

    public PadecimientoCatalogoRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(PadecimientoCatalogoCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'C', id = (int?)null, nombre = dto.Nombre, descripcion = dto.Descripcion, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R (solo activos)
    public async Task<IEnumerable<PadecimientoCatalogoDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<PadecimientoCatalogoDto>(
            SP,
            new { opcion = 'R', id = (int?)null, nombre = (string?)null, descripcion = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // I (por id)
    public async Task<PadecimientoCatalogoDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<PadecimientoCatalogoDto>(
            SP,
            new { opcion = 'I', id, nombre = (string?)null, descripcion = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // U
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, PadecimientoCatalogoUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'U', id, nombre = dto.Nombre, descripcion = dto.Descripcion, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // D (soft delete → estado=0)
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id, nombre = (string?)null, descripcion = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }
}