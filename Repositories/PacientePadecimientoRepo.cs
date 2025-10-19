using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.PacientePadecimiento;

    public sealed class PacientePadecimientoRepo
    {
        private readonly string _cs;
        private const string SP = "dbo.sp_crud_paciente_padecimiento"; 

        // Record interno para mapear el mensaje del SP
        private sealed record ProcMsg(string Mensaje, int? Codigo);

        // Función de mapeo de respuesta del SP
        private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
            => pm is null
                ? (true, "OK", null)
                : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

        public PacientePadecimientoRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(PacientePadecimientoCreateDto dto)
        {
            using var conn = new SqlConnection(_cs);

            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP,
                new
                {
                    opcion = 'C',
                    paciente_id = dto.PacienteId,
                    padecimiento_id = dto.PadecimientoId,
                    observacion = dto.Observacion,
                    estado = dto.Estado
                },
                commandType: CommandType.StoredProcedure);

            return Map(pm);
        }
        public async Task<IEnumerable<PacientePadecimientoDto>> ListarPorPacienteAsync(int pacienteId)
        {
            using var conn = new SqlConnection(_cs);
            return await conn.QueryAsync<PacientePadecimientoDto>(
                SP,
                new
                {
                    opcion = 'R',
                    paciente_id = pacienteId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(
            int pacienteId,
            int padecimientoId,
            PacientePadecimientoUpdateDto dto)
        {
            using var conn = new SqlConnection(_cs);

            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP,
                new
                {
                    opcion = 'U',
                    paciente_id = pacienteId,
                    padecimiento_id = padecimientoId,
                    observacion = dto.Observacion,
                    estado = dto.Estado
                },
                commandType: CommandType.StoredProcedure);

            return Map(pm);
        }

        public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int pacienteId, int padecimientoId)
        {
            using var conn = new SqlConnection(_cs);
            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP,
                new
                {
                    opcion = 'D',
                    paciente_id = pacienteId,
                    padecimiento_id = padecimientoId
                },
                commandType: CommandType.StoredProcedure);

            return Map(pm);
        }
    }