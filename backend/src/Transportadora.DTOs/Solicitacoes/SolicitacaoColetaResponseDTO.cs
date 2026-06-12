using Transportadora.Models.Enums;

namespace Transportadora.DTOs.Solicitacoes;

public sealed class SolicitacaoColetaResponseDTO
{
    public int Id { get; init; }
    public int RemetenteId { get; init; }
    public string RemetenteNome { get; init; } = string.Empty;
    public int DestinatarioId { get; init; }
    public string DestinatarioNome { get; init; } = string.Empty;
    public int CriadoPorUserId { get; init; }
    public string CriadoPorNome { get; init; } = string.Empty;
    public DateTime DataSolicitacao { get; init; }
    public DateTime DataPrevistaRetirada { get; init; }
    public PrioridadeNivel Prioridade { get; init; }
    public StatusColeta Status { get; init; }
    public int? MotoristaId { get; init; }
    public string? MotoristaNome { get; init; }
    public int? VeiculoId { get; init; }
    public string? VeiculoModelo { get; init; }
    public string? VeiculoPlaca { get; init; }
    public string? ObservacoesGerais { get; init; }
    public CargaResponseDTO Carga { get; init; } = new();
    public IReadOnlyList<OcorrenciaResponseDTO> Ocorrencias { get; init; } = Array.Empty<OcorrenciaResponseDTO>();
}
