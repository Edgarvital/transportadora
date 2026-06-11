using System.ComponentModel.DataAnnotations;

namespace Transportadora.DTOs.Motoristas;

public sealed class MotoristaRequestDTO
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; init; } = string.Empty;

    [Required(ErrorMessage = "Documento é obrigatório.")]
    [RegularExpression("^(\\d{11}|\\d{14})$", ErrorMessage = "Documento deve conter 11 (CPF) ou 14 (CNPJ) dígitos.")]
    public string Documento { get; init; } = string.Empty;
}
