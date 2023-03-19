using BlueBirds.Models;

namespace BlueBirds.TokenService
{
    public interface ITokenService
    {
        Task<string> GenerateToken(AppUser user);
    }
}
