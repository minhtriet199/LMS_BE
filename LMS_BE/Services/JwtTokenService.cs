using System.Security.Claims;
using System.Text;
using LMS_BE.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LMS_BE.Services;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string Issuer { get; set; } = "LMS_BE";
    public string Audience { get; set; } = "LMS_FE";
    public string Key { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; } = 15;
    public int RefreshTokenExpiresDays { get; set; } = 14;

    public SymmetricSecurityKey GetSigningKey() => new(Encoding.UTF8.GetBytes(Key));
}

public static class LmsClaims
{
    public const string UserId = JwtRegisteredClaimNames.Sub;
    public const string Name = "name";
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string Role = "role";
}

public class JwtTokenService(JwtOptions options)
{
    public string CreateToken(ApplicationUser user, string role)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Audience = options.Audience,
            Expires = DateTime.UtcNow.AddMinutes(options.ExpiresMinutes),
            SigningCredentials = new SigningCredentials(options.GetSigningKey(), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(
            [
                new Claim(LmsClaims.UserId, user.Id.ToString()),
                new Claim(LmsClaims.Name, user.FullName),
                new Claim(LmsClaims.Email, user.Email ?? string.Empty),
                new Claim(LmsClaims.Role, role),
            ]),
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(LmsClaims.UserId)
                   ?? throw new InvalidOperationException("Token thiếu claim sub"));
}
