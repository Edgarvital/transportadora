using Microsoft.EntityFrameworkCore;
using Transportadora.Data.Context;
using Transportadora.DTOs.Veiculos;
using Transportadora.Models.Entities;
using Transportadora.Shared.Errors;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Veiculos;

public sealed class VeiculoService(TransportadoraDbContext dbContext) : IVeiculoService
{
    public async Task<IReadOnlyList<VeiculoResponseDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Veiculos
            .AsNoTracking()
            .OrderBy(x => x.Modelo)
            .Select(x => new VeiculoResponseDTO
            {
                Id = x.Id,
                Modelo = x.Modelo,
                Placa = x.Placa
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<VeiculoResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Veiculos
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new VeiculoResponseDTO
            {
                Id = x.Id,
                Modelo = x.Modelo,
                Placa = x.Placa
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result<VeiculoResponseDTO>> CreateAsync(VeiculoRequestDTO request, CancellationToken cancellationToken)
    {
        var normalizedPlaca = request.Placa.ToUpperInvariant();
        var exists = await dbContext.Veiculos.AnyAsync(x => x.Placa == normalizedPlaca, cancellationToken);
        if (exists)
        {
            return Result<VeiculoResponseDTO>.Fail(ResourceErrorCodes.Duplicate, "Ja existe um veiculo com esta placa.");
        }

        var entity = new Veiculo
        {
            Modelo = request.Modelo,
            Placa = normalizedPlaca
        };

        dbContext.Veiculos.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<VeiculoResponseDTO>.Ok(new VeiculoResponseDTO
        {
            Id = entity.Id,
            Modelo = entity.Modelo,
            Placa = entity.Placa
        });
    }

    public async Task<Result<VeiculoResponseDTO>> UpdateAsync(int id, VeiculoRequestDTO request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Veiculos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result<VeiculoResponseDTO>.Fail(ResourceErrorCodes.NotFound, "Veiculo nao encontrado.");
        }

        var normalizedPlaca = request.Placa.ToUpperInvariant();
        var duplicate = await dbContext.Veiculos.AnyAsync(x => x.Placa == normalizedPlaca && x.Id != id, cancellationToken);
        if (duplicate)
        {
            return Result<VeiculoResponseDTO>.Fail(ResourceErrorCodes.Duplicate, "Ja existe um veiculo com esta placa.");
        }

        entity.Modelo = request.Modelo;
        entity.Placa = normalizedPlaca;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<VeiculoResponseDTO>.Ok(new VeiculoResponseDTO
        {
            Id = entity.Id,
            Modelo = entity.Modelo,
            Placa = entity.Placa
        });
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Veiculos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result<bool>.Fail(ResourceErrorCodes.NotFound, "Veiculo nao encontrado.");
        }

        dbContext.Veiculos.Remove(entity);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (DbUpdateException)
        {
            return Result<bool>.Fail(ResourceErrorCodes.InUse, "Veiculo possui vinculos e nao pode ser removido.");
        }
    }
}
