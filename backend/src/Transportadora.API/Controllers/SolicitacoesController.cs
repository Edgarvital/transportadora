using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportadora.DTOs.Solicitacoes;
using Transportadora.Models.Enums;
using Transportadora.Services.Solicitacoes;
using Transportadora.Shared.Errors;
using Transportadora.Shared.Results;

namespace Transportadora.API.Controllers;

[ApiController]
[Authorize]
[Route("api/solicitacoes")]
public class SolicitacoesController(ISolicitacaoColetaService solicitacaoColetaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<SolicitacaoColetaResponseDTO>>> GetAll(
        [FromQuery] SolicitacaoColetaQueryDTO queryDto,
        CancellationToken cancellationToken)
    {
        var response = await solicitacaoColetaService.GetAllAsync(queryDto, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SolicitacaoColetaResponseDTO>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await solicitacaoColetaService.GetByIdAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<SolicitacaoColetaResponseDTO>> CriarSolicitacao(
        [FromBody] SolicitacaoColetaCreateRequestDTO request,
        CancellationToken cancellationToken)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var criadoPorUserId))
        {
            return Unauthorized("Token inválido para identificação do usuário.");
        }

        var result = await solicitacaoColetaService.CriarAsync(request, criadoPorUserId, cancellationToken);
        if (!result.Success)
        {
            return result.ErrorCode switch
            {
                SolicitacaoErrorCodes.RemetenteNotFound => NotFound(result.ErrorMessage),
                SolicitacaoErrorCodes.DestinatarioNotFound => NotFound(result.ErrorMessage),
                SolicitacaoErrorCodes.CriadorNotFound => NotFound(result.ErrorMessage),
                _ => BadRequest(result.ErrorMessage)
            };
        }

        return CreatedAtAction(nameof(CriarSolicitacao), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}/roteirizar")]
    public async Task<ActionResult<SolicitacaoColetaResponseDTO>> RoteirizarSolicitacao(
        int id,
        [FromBody] RoteirizarRequestDTO request,
        CancellationToken cancellationToken)
    {
        var result = await solicitacaoColetaService.RoteirizarAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            return result.ErrorCode switch
            {
                SolicitacaoErrorCodes.SolicitationNotFound => NotFound(result.ErrorMessage),
                SolicitacaoErrorCodes.MotoristaNotFound => NotFound(result.ErrorMessage),
                SolicitacaoErrorCodes.VeiculoNotFound => NotFound(result.ErrorMessage),
                _ => BadRequest(result.ErrorMessage)
            };
        }

        return Ok(result.Data);
    }

    [HttpPost("{id:int}/ocorrencias")]
    public async Task<ActionResult<SolicitacaoColetaResponseDTO>> RegistrarOcorrencia(
        int id,
        [FromBody] OcorrenciaRequestDTO request,
        CancellationToken cancellationToken)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var userId))
        {
            return Unauthorized("Token inválido para identificação do usuário.");
        }

        var result = await solicitacaoColetaService.RegistrarOcorrenciaAsync(id, userId, request, cancellationToken);
        if (!result.Success)
        {
            return result.ErrorCode switch
            {
                SolicitacaoErrorCodes.SolicitationNotFound => NotFound(result.ErrorMessage),
                SolicitacaoErrorCodes.CriadorNotFound => NotFound(result.ErrorMessage),
                _ => BadRequest(result.ErrorMessage)
            };
        }

        return Ok(result.Data);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<SolicitacaoColetaResponseDTO>> MudarStatus(
        int id,
        [FromBody] MudarStatusRequestDTO request,
        CancellationToken cancellationToken)
    {
        var result = await solicitacaoColetaService.AtualizarStatusAsync(id, request.NovoStatus, cancellationToken);
        if (!result.Success)
        {
            return result.ErrorCode switch
            {
                SolicitacaoErrorCodes.SolicitationNotFound => NotFound(result.ErrorMessage),
                SolicitacaoErrorCodes.InvalidStatusTransition => BadRequest(result.ErrorMessage),
                SolicitacaoErrorCodes.ColetadaNeedsRouting => BadRequest(result.ErrorMessage),
                _ => BadRequest(result.ErrorMessage)
            };
        }

        return Ok(result.Data);
    }
}
