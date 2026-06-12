using Transportadora.DTOs.Solicitacoes;
using Transportadora.Models.Enums;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Solicitacoes;

public interface ISolicitacaoColetaService
{
    Task<PagedResult<SolicitacaoColetaResponseDTO>> GetAllAsync(SolicitacaoColetaQueryDTO queryDto, CancellationToken cancellationToken);
    Task<SolicitacaoColetaResponseDTO?> GetByIdAsync(int solicitacaoId, CancellationToken cancellationToken);
    Task<Result<SolicitacaoColetaResponseDTO>> CriarAsync(SolicitacaoColetaCreateRequestDTO dto, int criadoPorUserId, CancellationToken cancellationToken);
    Task<Result<SolicitacaoColetaResponseDTO>> RoteirizarAsync(int solicitacaoId, RoteirizarRequestDTO dto, CancellationToken cancellationToken);
    Task<Result<SolicitacaoColetaResponseDTO>> RegistrarOcorrenciaAsync(int solicitacaoId, int userId, OcorrenciaRequestDTO dto, CancellationToken cancellationToken);
    Task<Result<SolicitacaoColetaResponseDTO>> AtualizarStatusAsync(int solicitacaoId, StatusColeta novoStatus, CancellationToken cancellationToken);
}
