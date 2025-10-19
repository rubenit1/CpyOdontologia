using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using DTOs.RolPermisos;

public sealed class RolesPermisosRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_roles_permisos_sync";

    public RolesPermisosRepo(IConfiguration cfg)
        => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    public async Task<(bool ok, string mensaje, int? codigo)> SincronizarAsync(RolPermisosUpdateDto dto)
    {
        // Construir TVP
        var tvp = new DataTable();
        tvp.Columns.Add("PermisosID", typeof(int));

        foreach (var id in dto.PermisosIds.Distinct())
            tvp.Rows.Add(id);

        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                RolId = dto.RolId,
                Permisos = tvp.AsTableValuedParameter("dbo.TVP_PermisoId")
            },
            commandType: CommandType.StoredProcedure);

        return Map(pm);
    }
}