using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DTOs.PacienteMedicamentoActual; 

    public sealed class PacienteMedicamentoActualRepo
    {
        private readonly string _cs;
        private const string SP = "dbo.sp_crud_paciente_medicamento_actual"; 

        private sealed record ProcMsg(string Mensaje, int? Codigo);

        private static (bool ok, string mensaje, int? codigo) Map(ProcMsg? pm)
            => pm is null
                ? (true, "OK", null)
                : (!(pm.Codigo is >= 400), pm.Mensaje, pm.Codigo);

        public PacienteMedicamentoActualRepo(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        public async Task<(bool ok, string mensaje, int? codigo)> CrearAsync(PacienteMedicamentoActualCreateDto dto)
        {
            using var conn = new SqlConnection(_cs);

            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP,
                new
                {
                    opcion = 'C',
                    paciente_id = dto.PacienteId,
                    medicamento_id = dto.MedicamentoId,
                    dosis = dto.Dosis,
                    frecuencia = dto.Frecuencia,
                    estado = dto.Estado
                },
                commandType: CommandType.StoredProcedure);

            return Map(pm);
        }

        // ---------------------------------------------------------------------
        // R - Listar por Paciente
        // ---------------------------------------------------------------------
        public async Task<IEnumerable<PacienteMedicamentoActualDto>> ListarPorPacienteAsync(int pacienteId)
        {
            using var conn = new SqlConnection(_cs);
            return await conn.QueryAsync<PacienteMedicamentoActualDto>(
                SP,
                new
                {
                    opcion = 'R',
                    paciente_id = pacienteId
                },
                commandType: CommandType.StoredProcedure);
        }

        // ---------------------------------------------------------------------
        // U - Actualizar
        // ---------------------------------------------------------------------
        public async Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(
            int pacienteId,
            int medicamentoId,
            PacienteMedicamentoActualUpdateDto dto)
        {
            using var conn = new SqlConnection(_cs);

            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP,
                new
                {
                    opcion = 'U',
                    paciente_id = pacienteId,
                    medicamento_id = medicamentoId,
                    dosis = dto.Dosis,
                    frecuencia = dto.Frecuencia,
                    estado = dto.Estado
                },
                commandType: CommandType.StoredProcedure);

            return Map(pm);
        }

        // ---------------------------------------------------------------------
        // D - Desactivar (Borrado Lógico)
        // ---------------------------------------------------------------------
        public async Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int pacienteId, int medicamentoId)
        {
            using var conn = new SqlConnection(_cs);
            var pm = await conn.QueryFirstOrDefaultAsync<ProcMsg>(
                SP,
                new
                {
                    opcion = 'D',
                    paciente_id = pacienteId,
                    medicamento_id = medicamentoId
                },
                commandType: CommandType.StoredProcedure);

            return Map(pm);
        }
    }