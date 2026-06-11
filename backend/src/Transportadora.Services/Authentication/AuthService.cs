using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Transportadora.Models.Entities;

namespace Transportadora.Services.Authentication;

public sealed class AuthService(IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Nome),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, user.Role.Nome.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashPassword(string password)
    {
        var saltedPassword = $"{password}|TransportadoraMvpSalt";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword));
        return Convert.ToHexString(bytes);
    }

    public bool VerifyPassword(string password, string? passwordHash)
        => !string.IsNullOrWhiteSpace(passwordHash)
           && string.Equals(HashPassword(password), passwordHash, StringComparison.OrdinalIgnoreCase);
}
