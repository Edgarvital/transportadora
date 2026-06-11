using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportadora.DTOs.Motoristas;
using Transportadora.Services.Motoristas;
using Transportadora.Shared.Errors;

namespace Transportadora.API.Controllers;

[ApiController]
[Route("api/motoristas")]
public class MotoristasController(IMotoristaService motoristaService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<MotoristaResponseDTO>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await motoristaService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<MotoristaResponseDTO>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await motoristaService.GetByIdAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MotoristaResponseDTO>> Create([FromBody] MotoristaRequestDTO request, CancellationToken cancellationToken)
    {
        var result = await motoristaService.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            if (result.ErrorCode == ResourceErrorCodes.Duplicate)
            {
                return Conflict(result.ErrorMessage);
            }

            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MotoristaResponseDTO>> Update(int id, [FromBody] MotoristaRequestDTO request, CancellationToken cancellationToken)
    {
        var result = await motoristaService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            if (result.ErrorCode == ResourceErrorCodes.NotFound)
            {
                return NotFound(result.ErrorMessage);
            }

            if (result.ErrorCode == ResourceErrorCodes.Duplicate)
            {
                return Conflict(result.ErrorMessage);
            }

            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await motoristaService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            if (result.ErrorCode == ResourceErrorCodes.NotFound)
            {
                return NotFound(result.ErrorMessage);
            }

            if (result.ErrorCode == ResourceErrorCodes.InUse)
            {
                return Conflict(result.ErrorMessage);
            }

            return BadRequest(result.ErrorMessage);
        }

        return NoContent();
    }
}
