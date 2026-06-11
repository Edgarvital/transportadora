namespace Transportadora.DTOs.Users;

public sealed class UserRegistrationResponseDTO
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Situacao { get; init; } = string.Empty;
    public bool PossuiLogin { get; init; }
}
