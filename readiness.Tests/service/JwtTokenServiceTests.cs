using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenServiceTests
{
    [Fact]
    public void CreateToken_ProducesSignedTokenWithUserClaimsAndExpiration()
    {
        const string signingKey = "a-long-test-signing-key-with-at-least-32-bytes";
        var settings = new JwtSettings
        {
            Issuer = "Readiness.Tests",
            Audience = "Readiness.Tests.Client",
            SigningKey = signingKey,
            ExpirationMinutes = 15
        };
        var user = new User
        {
            Id = 42,
            Email = "reader@example.com",
            Name = "Reader",
            Role = new Role { Id = 1, Type = RoleType.User }
        };
        var service = new JwtTokenService(settings);

        var (tokenValue, expiresAtUtc) = service.CreateToken(user);
        var principal = new JwtSecurityTokenHandler().ValidateToken(
            tokenValue,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            },
            out var validatedToken);

        Assert.Equal(SecurityAlgorithms.HmacSha256, ((JwtSecurityToken)validatedToken).Header.Alg);
        Assert.Equal("42", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("reader@example.com", principal.FindFirst(ClaimTypes.Email)?.Value);
        Assert.Equal("Reader", principal.FindFirst(ClaimTypes.Name)?.Value);
        Assert.Equal(RoleType.User.ToString(), principal.FindFirst(ClaimTypes.Role)?.Value);
        Assert.InRange(expiresAtUtc, DateTime.UtcNow.AddMinutes(14), DateTime.UtcNow.AddMinutes(16));
    }
}
