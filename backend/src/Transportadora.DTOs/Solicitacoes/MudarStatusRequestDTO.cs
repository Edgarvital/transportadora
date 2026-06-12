using System.ComponentModel.DataAnnotations;
using Transportadora.Models.Enums;

namespace Transportadora.DTOs.Solicitacoes;

public sealed class MudarStatusRequestDTO
{
    [Required(ErrorMessage = "NovoStatus e obrigatorio.")]
    public StatusColeta NovoStatus { get; init; }
}
