using DTOs.Aseguradoras;

public interface IAseguradorasRepo
{
    Task<(bool ok, string mensaje, int? codigo)> CrearAsync(AseguradoraCreateDto dto); // C
    Task<IEnumerable<AseguradoraDto>> ListarActivasAsync();                            // R
    Task<AseguradoraDto?> ObtenerAsync(int id);                                        // I
    Task<(bool ok, string mensaje, int? codigo)> ActualizarAsync(int id, AseguradoraUpdateDto dto); // U
    Task<(bool ok, string mensaje, int? codigo)> DesactivarAsync(int id);              // D
}