using System.ComponentModel.DataAnnotations;

namespace Transportadora.DTOs.Users;

public sealed class EnderecoRegistrationRequestDTO
{
    [Required(ErrorMessage = "CEP é obrigatório.")]
    [RegularExpression("^\\d{5}-?\\d{3}$", ErrorMessage = "CEP deve estar no formato 00000-000.")]
    public string CEP { get; init; } = string.Empty;

    [Required(ErrorMessage = "Logradouro é obrigatório.")]
    [MaxLength(150, ErrorMessage = "Logradouro deve ter no máximo 150 caracteres.")]
    public string Logradouro { get; init; } = string.Empty;

    [Required(ErrorMessage = "Número é obrigatório.")]
    [MaxLength(20, ErrorMessage = "Número deve ter no máximo 20 caracteres.")]
    public string Numero { get; init; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres.")]
    public string? Complemento { get; init; }

    [Required(ErrorMessage = "Bairro é obrigatório.")]
    [MaxLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres.")]
    public string Bairro { get; init; } = string.Empty;

    [Required(ErrorMessage = "Cidade é obrigatória.")]
    [MaxLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres.")]
    public string Cidade { get; init; } = string.Empty;

    [Required(ErrorMessage = "UF é obrigatória.")]
    [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "UF deve conter 2 letras.")]
    public string UF { get; init; } = string.Empty;
}
