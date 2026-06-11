using Transportadora.DTOs.Veiculos;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Veiculos;

public interface IVeiculoService
{
    Task<IReadOnlyList<VeiculoResponseDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<VeiculoResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VeiculoResponseDTO>> CreateAsync(VeiculoRequestDTO request, CancellationToken cancellationToken);
    Task<Result<VeiculoResponseDTO>> UpdateAsync(int id, VeiculoRequestDTO request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
}
