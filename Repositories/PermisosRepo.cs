using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Permisos;

public sealed class PermisosRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_listar_permisos";

    public PermisosRepo(IConfiguration cfg)
        => _cs = cfg.GetConnectionString("DefaultConnection")!;

    public async Task<IEnumerable<PermisoDto>> ListarAsync(string? buscar = null)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<PermisoDto>(
            SP,
            new { buscar },
            commandType: CommandType.StoredProcedure);
    }
}