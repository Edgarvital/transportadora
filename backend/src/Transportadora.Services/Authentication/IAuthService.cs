using Transportadora.Models.Entities;

namespace Transportadora.Services.Authentication;

public interface IAuthService
{
    string GenerateToken(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string? passwordHash);
}
