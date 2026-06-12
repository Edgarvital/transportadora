using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    public async Task<ActionResult<UserRegistrationResponseDTO>> Register(
        [FromBody] UserRegistrationRequestDTO request, 
        CancellationToken cancellationToken)
    {
        var result = await userService.RegisterAsync(request, cancellationToken);
        if (!result.Success)
        {
            if (result.ErrorCode == UserErrorCodes.EmailAlreadyExists)
            {
                return Conflict(result.ErrorMessage);
            }

            if (result.ErrorCode == UserErrorCodes.InvalidLoginPair)
            {
                return BadRequest(result.ErrorMessage);
            }

            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(Register),
            new { id = result.Data!.Id },
            result.Data);
    }

    [HttpGet]
    [Authorize] 
    public async Task<ActionResult<IReadOnlyList<UserRegistrationResponseDTO>>> GetAll(
        CancellationToken cancellationToken)
    {
        var users = await userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("documento/{documento}")]
    [Authorize]
    public async Task<ActionResult<UserRegistrationResponseDTO>> GetByDocument(
        string documento, 
        CancellationToken cancellationToken)
    {
        var result = await userService.GetByDocumentAsync(documento, cancellationToken);
        
        if (result.Success && result.Data is null)
        {
            return NoContent(); 
        }

        return Ok(result.Data);
    }
}