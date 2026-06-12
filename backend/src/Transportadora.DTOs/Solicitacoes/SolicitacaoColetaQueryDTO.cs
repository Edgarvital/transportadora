using Transportadora.Models.Enums;

public record SolicitacaoColetaQueryDTO(
    StatusColeta? Status,
    PrioridadeNivel? Prioridade,
    int? RemetenteId,
    int? DestinatarioId = null,
    DateTime? DataSolicitacaoInicial = null,
    DateTime? DataSolicitacaoFinal = null,
    DateTime? DataPrevistaRetiradaInicial = null,
    DateTime? DataPrevistaRetiradaFinal = null,
    int PageNumber = 1,
    int PageSize = 10
);