using System.ComponentModel.DataAnnotations;

namespace Transportadora.DTOs.Auth;

public sealed class LoginRequestDTO
{
    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres.")]
    [MaxLength(100, ErrorMessage = "Senha deve ter no máximo 100 caracteres.")]
    public string Senha { get; init; } = string.Empty;
}
