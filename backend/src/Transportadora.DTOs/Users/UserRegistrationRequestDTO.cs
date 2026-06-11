using System.ComponentModel.DataAnnotations;

namespace Transportadora.DTOs.Users;

public sealed class UserRegistrationRequestDTO
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; init; } = string.Empty;

    [Required(ErrorMessage = "Documento é obrigatório.")]
    [RegularExpression("^(\\d{11}|\\d{14})$", ErrorMessage = "Documento deve conter 11 (CPF) ou 14 (CNPJ) dígitos.")]
    public string Documento { get; init; } = string.Empty;

    [Required(ErrorMessage = "Endereço é obrigatório.")]
    public EnderecoRegistrationRequestDTO Endereco { get; init; } = new();

    [EmailAddress(ErrorMessage = "Email inválido.")]
    [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres.")]
    public string? Email { get; init; }

    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres.")]
    [MaxLength(100, ErrorMessage = "Senha deve ter no máximo 100 caracteres.")]
    public string? Senha { get; init; }
}
