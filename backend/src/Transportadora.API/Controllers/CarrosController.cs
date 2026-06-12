using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transportadora.DTOs.Veiculos;
using Transportadora.Services.Veiculos;
using Transportadora.Shared.Errors;

namespace Transportadora.API.Controllers;

[ApiController]
[Route("api/carros")]
public class CarrosController(IVeiculoService veiculoService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<VeiculoResponseDTO>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await veiculoService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<VeiculoResponseDTO>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await veiculoService.GetByIdAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VeiculoResponseDTO>> Create([FromBody] VeiculoRequestDTO request, CancellationToken cancellationToken)
    {
        var result = await veiculoService.CreateAsync(request, cancellationToken);
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
    public async Task<ActionResult<VeiculoResponseDTO>> Update(int id, [FromBody] VeiculoRequestDTO request, CancellationToken cancellationToken)
    {
        var result = await veiculoService.UpdateAsync(id, request, cancellationToken);
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
        var result = await veiculoService.DeleteAsync(id, cancellationToken);
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
