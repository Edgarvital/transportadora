using System.ComponentModel.DataAnnotations;
using Transportadora.Models.Enums;

namespace Transportadora.DTOs.Solicitacoes;

public sealed class OcorrenciaRequestDTO
{
    [Required(ErrorMessage = "Tipo da ocorrencia e obrigatorio.")]
    public OcorrenciaTipo Tipo { get; init; }

    [Required(ErrorMessage = "Descricao da ocorrencia e obrigatoria.")]
    [MinLength(3, ErrorMessage = "Descricao deve ter no minimo 3 caracteres.")]
    [MaxLength(2000, ErrorMessage = "Descricao deve ter no maximo 2000 caracteres.")]
    public string Descricao { get; init; } = string.Empty;
}
