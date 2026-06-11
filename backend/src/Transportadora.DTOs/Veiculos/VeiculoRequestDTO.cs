using System.ComponentModel.DataAnnotations;

namespace Transportadora.DTOs.Veiculos;

public sealed class VeiculoRequestDTO
{
    [Required(ErrorMessage = "Modelo é obrigatório.")]
    [MaxLength(120, ErrorMessage = "Modelo deve ter no máximo 120 caracteres.")]
    public string Modelo { get; init; } = string.Empty;

    [Required(ErrorMessage = "Placa é obrigatória.")]
    [RegularExpression("^[A-Za-z]{3}[0-9][A-Za-z0-9][0-9]{2}$", ErrorMessage = "Placa inválida.")]
    public string Placa { get; init; } = string.Empty;
}
