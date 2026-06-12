using System.ComponentModel.DataAnnotations;

namespace Transportadora.DTOs.Solicitacoes;

public sealed class RoteirizarRequestDTO
{
    [Required(ErrorMessage = "MotoristaId e obrigatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "MotoristaId invalido.")]
    public int MotoristaId { get; init; }

    [Required(ErrorMessage = "VeiculoId e obrigatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "VeiculoId invalido.")]
    public int VeiculoId { get; init; }
}
