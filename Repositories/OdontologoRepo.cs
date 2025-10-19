using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Odontologo;

public sealed class OdontologoRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_odontologo";
    private const string TVP_ESPECIALIDAD = "dbo.TipoTablaEspecialidad";
    private const string TVP_SEDE = "dbo.TipoTablaSede";

    public OdontologoRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo);
    private sealed record CreateMsg(string Mensaje, int? id);

    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null ? (true, "OK", null) : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // Helpers: construir DataTable para TVP
    private static DataTable BuildIntTvp(string colName, IEnumerable<int>? values)
    {
        var dt = new DataTable();
        dt.Columns.Add(colName, typeof(int));
        if (values != null)
            foreach (var v in values)
                dt.Rows.Add(v);
        return dt;
    }

    // Devuelve TVP SIEMPRE (aunque vacío) para llamadas que requieren el parámetro
    private static object TvpEspecialidades(IEnumerable<int>? ids)
        => BuildIntTvp("especialidad_id", ids ?? Array.Empty<int>()).AsTableValuedParameter(TVP_ESPECIALIDAD);

    private static object TvpSedes(IEnumerable<int>? ids)
        => BuildIntTvp("sede_id", ids ?? Array.Empty<int>()).AsTableValuedParameter(TVP_SEDE);

    // Crea odontólogo. El SP puede devolver (Mensaje, id) o (Mensaje, Codigo)
public async Task<(bool ok, string mensaje, int? id)> CrearAsync(OdontologoCreateDto dto)
{
    try
    {
        using var conn = new SqlConnection(_cs);

        var p = new DynamicParameters();
        p.Add("opcion", 'C');
        p.Add("id", (int?)null);
        p.Add("usuario_id", dto.Usuario_Id);
        p.Add("nombre", dto.Nombre);
        p.Add("apellido", dto.Apellido);
        p.Add("matricula", dto.Matricula);
        p.Add("estado", dto.Estado);
        p.Add("especialidades",
            BuildIntTvp("especialidad_id", dto.Especialidades ?? Array.Empty<int>())
            .AsTableValuedParameter(TVP_ESPECIALIDAD));
        p.Add("sedes",
            BuildIntTvp("sede_id", dto.Sedes ?? Array.Empty<int>())
            .AsTableValuedParameter(TVP_SEDE));

        // Tipo flexible para cubrir ambas formas del SELECT en el SP
        var row = await conn.QueryFirstOrDefaultAsync<CrearFlexible>(
            SP, p, commandType: CommandType.StoredProcedure);

        if (row is null)
            return (false, "No se recibió respuesta del procedimiento.", null);

        // Si viene Código (>=400) lo tratamos como error de negocio
        if (row.Codigo is >= 400)
            return (false, row.Mensaje, null);

        // Success: debe venir id
        return (true, row.Mensaje, row.id);
    }
    catch (SqlException ex)
    {
        throw new InvalidOperationException($"Fallo {SP} (C): {ex.Message}", ex);
    }
}

// Tipo interno flexible para mapear ambos casos sin fallar
private sealed class CrearFlexible
{
    public string Mensaje { get; set; } = string.Empty;
    public int? id { get; set; }        // cuando crea OK
    public int? Codigo { get; set; }    // cuando el SP devuelve error de negocio
}
    // Listar activos (opción 'R')
    public async Task<IEnumerable<OdontologoDto>> ListarActivosAsync()
    {
        try
        {
            using var conn = new SqlConnection(_cs);

            var p = new DynamicParameters();
            p.Add("opcion", 'R');
            p.Add("id", (int?)null);
            p.Add("usuario_id", (int?)null);
            p.Add("nombre", (string?)null);
            p.Add("apellido", (string?)null);
            p.Add("matricula", (string?)null);
            p.Add("estado", (bool?)null);
            p.Add("especialidades",
                BuildIntTvp("especialidad_id", Array.Empty<int>())
                .AsTableValuedParameter(TVP_ESPECIALIDAD));
            p.Add("sedes",
                BuildIntTvp("sede_id", Array.Empty<int>())
                .AsTableValuedParameter(TVP_SEDE));

            return await conn.QueryAsync<OdontologoDto>(
                SP, p, commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Fallo {SP} (R): {ex.Message}", ex);
        }
    }

    // Obtener por id (el SP retorna 3 result sets; leemos el primero y drenamos los demás)
    public async Task<OdontologoDto?> ObtenerAsync(int id)
    {
        try
        {
            using var conn = new SqlConnection(_cs);

            var p = new DynamicParameters();
            p.Add("opcion", 'I');
            p.Add("id", id);
            p.Add("usuario_id", (int?)null);
            p.Add("nombre", (string?)null);
            p.Add("apellido", (string?)null);
            p.Add("matricula", (string?)null);
            p.Add("estado", (bool?)null);
            p.Add("especialidades",
                BuildIntTvp("especialidad_id", Array.Empty<int>())
                .AsTableValuedParameter(TVP_ESPECIALIDAD));
            p.Add("sedes",
                BuildIntTvp("sede_id", Array.Empty<int>())
                .AsTableValuedParameter(TVP_SEDE));

            using var multi = await conn.QueryMultipleAsync(
                SP, p, commandType: CommandType.StoredProcedure);

            var od = await multi.ReadFirstOrDefaultAsync<OdontologoDto?>();
            _ = await multi.ReadAsync<dynamic>(); // drenar 2º RS
            _ = await multi.ReadAsync<dynamic>(); // drenar 3º RS

            return od;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Fallo {SP} (I): {ex.Message}", ex);
        }
    }

    // Actualizar (sincroniza relaciones con MERGE, según el SP)
    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, OdontologoUpdateDto dto)
    {
        try
        {
            using var conn = new SqlConnection(_cs);

            var p = new DynamicParameters();
            p.Add("opcion", 'U');
            p.Add("id", id);
            p.Add("usuario_id", (int?)null);
            p.Add("nombre", dto.Nombre);
            p.Add("apellido", dto.Apellido);
            p.Add("matricula", dto.Matricula);
            p.Add("estado", dto.Estado);
            p.Add("especialidades",
                BuildIntTvp("especialidad_id", dto.Especialidades ?? Array.Empty<int>())
                .AsTableValuedParameter(TVP_ESPECIALIDAD));
            p.Add("sedes",
                BuildIntTvp("sede_id", dto.Sedes ?? Array.Empty<int>())
                .AsTableValuedParameter(TVP_SEDE));

            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP, p, commandType: CommandType.StoredProcedure);

            return Map(pm);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Fallo {SP} (U): {ex.Message}", ex);
        }
    }

    // Desactivar (borrado lógico + desactivar relaciones)
    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        try
        {
            using var conn = new SqlConnection(_cs);

            var p = new DynamicParameters();
            p.Add("opcion", 'D');
            p.Add("id", id);
            p.Add("usuario_id", (int?)null);
            p.Add("nombre", (string?)null);
            p.Add("apellido", (string?)null);
            p.Add("matricula", (string?)null);
            p.Add("estado", (bool?)null);
            p.Add("especialidades",
                BuildIntTvp("especialidad_id", Array.Empty<int>())
                .AsTableValuedParameter(TVP_ESPECIALIDAD));
            p.Add("sedes",
                BuildIntTvp("sede_id", Array.Empty<int>())
                .AsTableValuedParameter(TVP_SEDE));

            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP, p, commandType: CommandType.StoredProcedure);

            return Map(pm);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Fallo {SP} (D): {ex.Message}", ex);
        }
    }
}