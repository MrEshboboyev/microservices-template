using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Services;
using System.Text;

namespace SharedLibrary.DependencyInjection;

public static class JWTAuthenticationScheme
{
    public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration config)
    {
        var issuer = config.GetSection("Authentication:Issuer").Value!;
        var audience = config.GetSection("Authentication:Audience").Value!;
        var key = config.GetSection("Authentication:Key").Value!;
        var accessTokenExpiration = config.GetValue<int>("Authentication:AccessTokenExpirationMinutes", 15);

        // Register JWT Service
        services.AddSingleton(new JWTService(
            issuer,
            audience,
            key,
            accessTokenExpiration));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", options =>
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // Enable lifetime validation
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero // Remove delay of token when expire
                };
            });
        return services;
    }
}
