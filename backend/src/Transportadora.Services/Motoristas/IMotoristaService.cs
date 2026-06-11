using Transportadora.DTOs.Motoristas;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Motoristas;

public interface IMotoristaService
{
    Task<IReadOnlyList<MotoristaResponseDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<MotoristaResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<MotoristaResponseDTO>> CreateAsync(MotoristaRequestDTO request, CancellationToken cancellationToken);
    Task<Result<MotoristaResponseDTO>> UpdateAsync(int id, MotoristaRequestDTO request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
}
