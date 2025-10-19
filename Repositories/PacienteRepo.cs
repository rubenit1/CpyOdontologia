using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.Paciente;

public sealed class PacienteRepo
{
    private readonly string _cs;
    private const string SP = "dbo.sp_crud_paciente";

    // 1. TIPOS DE MENSAJES: Usamos clases flexibles para evitar problemas de Dapper
    private sealed class ProcMsg // Para U y D
    {
        public string Mensaje { get; set; } = string.Empty;
        public int? Codigo { get; set; }
    }

    private sealed class CrearFlexible // Para C
    {
        public string Mensaje { get; set; } = string.Empty;
        public int? Codigo { get; set; }    // Cuando hay error de negocio (ej. DPI duplicado)
        public int? id { get; set; }        // Cuando la creación es exitosa
    }

    // Helper para mapear la respuesta del SP (solo usa Mensaje y Codigo)
    private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
        => pm is null
            ? (true, "OK", null)
            : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

    // Constructor
    public PacienteRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    // NOTA: El método CreateSpParams() fue eliminado.

    public async Task<(bool ok, string mensaje, int? id)> CrearAsync(PacienteCreateDto dto)
    {
        using var conn = new SqlConnection(_cs);

        // 2. PARÁMETROS ESPECÍFICOS: Se construye el objeto anónimo localmente con solo los datos necesarios
        var parameters = new
        {
            opcion = "C",
            id = (int?)null,
            nombre = dto.Nombre,
            apellido = dto.Apellido,
            fecha_nacimiento = dto.FechaNacimiento.ToDateTime(TimeOnly.MinValue), // Conversión de DateOnly
            genero_id = dto.GeneroId,
            estado_civil_id = dto.EstadoCivilId,
            dpi = dto.Dpi,
            nit = dto.Nit,
            direccion = dto.Direccion,
            email = dto.Email,
            telefono = dto.Telefono,
            tipo_sangre_id = dto.TipoSangreId,
            origen_id = dto.OrigenId,
            referido_por_paciente_id = dto.ReferidoPorPacienteId,
            sede_id = dto.SedeId,
            estado = dto.Estado,
            instagram = dto.Instagram,
            tiktok = dto.Tiktok,
            facebook = dto.Facebook
        };

        // 3. LLAMADA FLEXIBLE: Usamos CrearFlexible para manejar el resultado de 3 columnas
        var result = await conn.QueryFirstOrDefaultAsync<CrearFlexible>(
            SP,
            parameters,
            commandType: CommandType.StoredProcedure);

        if (result?.Codigo is >= 400)
        {
            return (false, result.Mensaje, result.Codigo);
        }

        // Si el SP devolvió un ID (operación exitosa)
        return result is null || result.id is null
            ? (false, result?.Mensaje ?? "No se recibió ID de paciente nuevo.", result?.Codigo)
            : (true, result.Mensaje, result.id);
    }

    public async Task<IEnumerable<PacienteDto>> ListarActivosAsync()
    {
        using var conn = new SqlConnection(_cs);
        // PARÁMETROS ESPECÍFICOS: Solo se necesita la opción y el estado por defecto del SP
        var parameters = new { opcion = "R" };

        return await conn.QueryAsync<PacienteDto>(
            SP,
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<PacienteDto?> ObtenerAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        // PARÁMETROS ESPECÍFICOS: Se necesita la opción y el ID
        var parameters = new { opcion = "I", id };

        return await conn.QueryFirstOrDefaultAsync<PacienteDto>(
            SP,
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, PacienteUpdateDto dto)
    {
        using var conn = new SqlConnection(_cs);
        // PARÁMETROS ESPECÍFICOS: Datos de actualización y el ID
        var parameters = new
        {
            opcion = "U",
            id = id,
            nombre = dto.Nombre,
            apellido = dto.Apellido,
            fecha_nacimiento = dto.FechaNacimiento.ToDateTime(TimeOnly.MinValue), // Conversión opcional
            genero_id = dto.GeneroId,
            estado_civil_id = dto.EstadoCivilId,
            dpi = dto.Dpi,
            nit = dto.Nit,
            direccion = dto.Direccion,
            email = dto.Email,
            telefono = dto.Telefono,
            tipo_sangre_id = dto.TipoSangreId,
            origen_id = dto.OrigenId,
            referido_por_paciente_id = dto.ReferidoPorPacienteId,
            sede_id = dto.SedeId,
            estado = dto.Estado,
            instagram = dto.Instagram,
            tiktok = dto.Tiktok,
            facebook = dto.Facebook
        };

        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            parameters,
            commandType: CommandType.StoredProcedure);

        return Map(pm);
    }

    public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        // PARÁMETROS ESPECÍFICOS: Solo se necesita la opción y el ID
        var parameters = new { opcion = "D", id };

        var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
            SP,
            parameters,
            commandType: CommandType.StoredProcedure);

        return Map(pm);
    }

    public async Task<IEnumerable<PacienteBusquedaDto>> BuscarAsync(string terminoBusqueda)
    {
        using var conn = new SqlConnection(_cs);
        var parameters = new { termino_busqueda = terminoBusqueda };

       
        var pacientes = await conn.QueryAsync<PacienteBusquedaDto>(
            "dbo.sp_buscar_paciente_referido", // <-- El nombre que le diste a tu SP
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return pacientes;
    }

    public async Task<PacienteBusquedaDto?> ObtenerNombrePorIdAsync(int id)
    {
        using var conn = new SqlConnection(_cs);
        var parameters = new { id };
        return await conn.QueryFirstOrDefaultAsync<PacienteBusquedaDto>(
            "dbo.sp_get_paciente_nombre_por_id",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }
    // =================================================================
}
