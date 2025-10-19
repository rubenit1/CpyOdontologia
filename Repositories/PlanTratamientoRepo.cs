using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using DTOs.PlanTratamiento;

    public sealed class PlanTratamientoRepo
    {
        private readonly string _connectionString;

        public PlanTratamientoRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int?> ObtenerIdUsuarioPorNombreAsync(string nombreUsuario)
        {
            const string sql = @"
                SELECT id 
                FROM dbo.usuario 
                WHERE nombre_usuario = @NombreUsuario 
                COLLATE SQL_Latin1_General_CP1_CI_AS; -- Solución para problemas de mayúsculas/minúsculas (Case Insensitive)
            ";

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var parameters = new { NombreUsuario = nombreUsuario };

                // Intenta obtener un solo ID. Si no encuentra nada, devuelve null.
                return await db.QuerySingleOrDefaultAsync<int?>(sql, parameters);
            }
        }

        public async Task<IEnumerable<PlanTratamientoDto>> ListarAsync(int? id, int? pacienteId, bool? estado)
        {
            return await Task.FromResult(new List<PlanTratamientoDto>());
        }

        public async Task<(bool ok, string mensaje, int codigo)> CrearAsync(PlanTratamientoCreateDto dto, int creadoPorUsuarioId)
        {
           return await Task.FromResult((true, "Plan de tratamiento creado (placeholder).", 201));
        }

        public Task<(bool ok, string mensaje, int codigo)> ActualizarAsync(int id, PlanTratamientoUpdateDto dto)
        {
            return Task.FromResult((true, "Actualización exitosa (placeholder).", 204));
        }

        public Task<(bool ok, string mensaje, int codigo)> AnularAsync(int id)
        {
            return Task.FromResult((true, "Anulación exitosa (placeholder).", 204));
        }
    }