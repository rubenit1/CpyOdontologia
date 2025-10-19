using System.Security.Claims;
public interface IIdentityService
{
    int? GetCurrentUserId();
}

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Intenta obtener el ID del usuario logueado de los Claims del token JWT.
    /// Busca en los Claims más comunes para resolver el problema de la nomenclatura.
    /// </summary>
    /// <returns>El ID del usuario como entero, o null si no se encuentra.</returns>
    public int? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return null; 
        }

        // Lista de nombres de claims comunes que podrían contener el ID del usuario
        var potentialClaimNames = new[]
        {
            ClaimTypes.NameIdentifier,
            "id",
            "Id",
            "name",
            "sub",                      // Subject (otro nombre común en JWT)
            "user_id"
        };

        foreach (var claimName in potentialClaimNames)
        {
            var userIdClaim = user.FindFirst(claimName);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
        }

        // Si no se encuentra el ID numérico, ahora intentamos obtener el nombre (que sabemos que existe)
        // Aunque no es el ID, si tu token solo tiene el nombre, el sistema de autenticación lo puede usar
        // para la autorización, pero esta función solo devuelve el ID numérico.
        return null;
    }
}
