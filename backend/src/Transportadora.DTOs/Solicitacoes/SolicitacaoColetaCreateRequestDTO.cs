using System.ComponentModel.DataAnnotations;
using Transportadora.Models.Enums;

namespace Transportadora.DTOs.Solicitacoes;

public sealed class SolicitacaoColetaCreateRequestDTO
{
    [Required(ErrorMessage = "RemetenteId e obrigatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "RemetenteId invalido.")]
    public int RemetenteId { get; init; }

    [Required(ErrorMessage = "DestinatarioId e obrigatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "DestinatarioId invalido.")]
    public int DestinatarioId { get; init; }

    [Required(ErrorMessage = "Data prevista de retirada e obrigatoria.")]
    public DateTime DataPrevistaRetirada { get; init; }

    [Required(ErrorMessage = "Prioridade e obrigatoria.")]
    public PrioridadeNivel Prioridade { get; init; }

    [MaxLength(2000, ErrorMessage = "Observacoes gerais devem ter no maximo 2000 caracteres.")]
    public string? ObservacoesGerais { get; init; }

    [Required(ErrorMessage = "Carga e obrigatoria.")]
    public CargaDTO Carga { get; init; } = new();
}
