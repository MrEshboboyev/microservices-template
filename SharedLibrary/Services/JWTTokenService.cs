using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Models;
using System.Security.Claims;

namespace SharedLibrary.Services;

public class JWTTokenService(JWTService jwtService) : IJWTTokenService
{
    public JWTToken GenerateTokens(IEnumerable<Claim> claims)
    {
        var accessToken = jwtService.GenerateAccessToken(claims);
        var refreshToken = jwtService.GenerateRefreshToken();

        return new JWTToken
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
        };
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        return jwtService.GetPrincipalFromExpiredToken(token);
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        return jwtService.ValidateRefreshToken(refreshToken);
    }
}
