using Microsoft.AspNetCore.Mvc;
using Transportadora.DTOs.Users;
using Transportadora.Services.Users;
using Transportadora.Shared.Errors;

namespace Transportadora.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserRegistrationResponseDTO>> Register([FromBody] UserRegistrationRequestDTO request, CancellationToken cancellationToken)
    {
        var result = await userService.RegisterAsync(request, cancellationToken);
        if (!result.Success)
        {
            if (result.ErrorCode == UserErrorCodes.EmailAlreadyExists)
            {
                return Conflict(result.ErrorMessage);
            }

            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(Register),
            new { id = result.Data!.Id },
            result.Data);
    }
}
