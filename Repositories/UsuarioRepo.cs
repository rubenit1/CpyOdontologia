using Dapper;
using DTOs.Usuario;
using Microsoft.Data.SqlClient;
using System.Data;

public sealed class UsuarioRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_usuario";
    public UsuarioRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private sealed record ProcMsg(string Mensaje, int? Codigo, int? Id);

    // C: Crear usuario
    public async Task<(string mensaje, int? codigo, int? id)> CrearAsync(UsuarioCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'C',
                nombre_usuario = dto.Nombre_Usuario,
                password_hash = dto.Password_Hash,
                rol_id = dto.Rol_Id,
                email = dto.Email,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);

        // Si no hubo respuesta del SP
        if (pm is null)
            return ("No se recibió respuesta del procedimiento.", 500, null);

        // Devolvemos directamente lo que venga del SP
        return (pm.Mensaje, pm.Codigo, pm.Id);
    }

    public async Task<IEnumerable<UsuarioListDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryAsync<UsuarioListDto>(
            SP,
            new { opcion = 'R' },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<UsuarioDetailDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<UsuarioDetailDto>(
            SP,
            new { opcion = 'I', id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(string mensaje, int? codigo, int? id)> ActualizarAsync(int id, UsuarioUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new
            {
                opcion = 'U',
                id,
                nombre_usuario = dto.Nombre_Usuario,
                password_hash = dto.Password_Hash,
                rol_id = dto.Rol_Id,
                email = dto.Email,
                estado = dto.Estado
            },
            commandType: CommandType.StoredProcedure);

        if (pm is null)
            return ("No se recibió respuesta del procedimiento.", 500, null);

        return (pm.Mensaje, pm.Codigo, pm.Id);
    }

    public async Task<(string mensaje, int? codigo, int? id)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            new { opcion = 'D', id },
            commandType: CommandType.StoredProcedure);

        if (pm is null)
            return ("No se recibió respuesta del procedimiento.", 500, null);

        return (pm.Mensaje, pm.Codigo, pm.Id);
    }
}
