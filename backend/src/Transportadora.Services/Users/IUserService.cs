using Transportadora.DTOs.Users;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Users;

public interface IUserService
{
    Task<Result<UserRegistrationResponseDTO>> RegisterAsync(UserRegistrationRequestDTO request, CancellationToken cancellationToken);
}
