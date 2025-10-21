using SharedLibrary.Models;
using System.Security.Claims;

namespace SharedLibrary.Services;

public interface IJWTTokenService
{
    JWTToken GenerateTokens(IEnumerable<Claim> claims);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    bool ValidateRefreshToken(string refreshToken);
}
