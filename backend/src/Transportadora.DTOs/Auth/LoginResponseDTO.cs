namespace Transportadora.DTOs.Auth;

public sealed class LoginResponseDTO
{
	public string Token { get; init; } = string.Empty;
	public string NomeUsuario { get; init; } = string.Empty;
	public string Role { get; init; } = string.Empty;
}
