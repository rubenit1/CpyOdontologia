using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.MetodoPago;

public sealed class MetodoPagoRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_metodo_pago";

    public MetodoPagoRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // C
    public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(MetodoPagoCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'C', id = (int?)null, nombre = dto.Nombre, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // R
    public async Task<IEnumerable<MetodoPagoDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<MetodoPagoDto>(
            SP,
            new { opcion = 'R', id = (int?)null, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // I
    public async Task<MetodoPagoDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<MetodoPagoDto>(
            SP,
            new { opcion = 'I', id, nombre = (string?)null, estado = (bool?)null },
            commandType: CommandType.StoredProcedure);
    }

    // U
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, MetodoPagoUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'U', id, nombre = dto.Nombre, estado = dto.Estado },
            commandType: CommandType.StoredProcedure);
        return Map(pm);
    }

    // D (soft delete)
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