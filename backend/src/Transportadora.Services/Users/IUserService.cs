using Transportadora.DTOs.Users;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Users;

public interface IUserService
{
    Task<Result<UserRegistrationResponseDTO>> RegisterAsync(UserRegistrationRequestDTO request, CancellationToken cancellationToken);
    Task<Result<UserRegistrationResponseDTO?>> GetByDocumentAsync(string documento, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserRegistrationResponseDTO>> GetAllAsync(CancellationToken cancellationToken);
}