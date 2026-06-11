using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transportadora.Data.Context;
using Transportadora.DTOs.Auth;
using Transportadora.Services.Authentication;

namespace Transportadora.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(TransportadoraDbContext dbContext, IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
        {
            return BadRequest("Email e senha são obrigatórios.");
        }

        var user = await dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null || user.Situacao != Transportadora.Models.Enums.SituacaoUser.Ativo)
        {
            return Unauthorized("Credenciais inválidas ou usuário inativo.");
        }

        if (!authService.VerifyPassword(request.Senha, user.SenhaHash))
        {
            return Unauthorized("Credenciais inválidas.");
        }

        var token = authService.GenerateToken(user);
        return Ok(new LoginResponseDTO
        {
            Token = token,
        });
    }
}
