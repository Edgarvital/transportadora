using Microsoft.EntityFrameworkCore;
using Transportadora.Data.Context;
using Transportadora.DTOs.Motoristas;
using Transportadora.Models.Entities;
using Transportadora.Shared.Errors;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Motoristas;

public sealed class MotoristaService(TransportadoraDbContext dbContext) : IMotoristaService
{
    public async Task<IReadOnlyList<MotoristaResponseDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Motoristas
            .AsNoTracking()
            .OrderBy(x => x.Nome)
            .Select(x => new MotoristaResponseDTO
            {
                Id = x.Id,
                Nome = x.Nome,
                Documento = x.Documento
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<MotoristaResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Motoristas
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new MotoristaResponseDTO
            {
                Id = x.Id,
                Nome = x.Nome,
                Documento = x.Documento
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result<MotoristaResponseDTO>> CreateAsync(MotoristaRequestDTO request, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Motoristas.AnyAsync(x => x.Documento == request.Documento, cancellationToken);
        if (exists)
        {
            return Result<MotoristaResponseDTO>.Fail(ResourceErrorCodes.Duplicate, "Ja existe um motorista com este documento.");
        }

        var entity = new Motorista
        {
            Nome = request.Nome,
            Documento = request.Documento
        };

        dbContext.Motoristas.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<MotoristaResponseDTO>.Ok(new MotoristaResponseDTO
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Documento = entity.Documento
        });
    }

    public async Task<Result<MotoristaResponseDTO>> UpdateAsync(int id, MotoristaRequestDTO request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Motoristas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result<MotoristaResponseDTO>.Fail(ResourceErrorCodes.NotFound, "Motorista nao encontrado.");
        }

        var duplicate = await dbContext.Motoristas.AnyAsync(x => x.Documento == request.Documento && x.Id != id, cancellationToken);
        if (duplicate)
        {
            return Result<MotoristaResponseDTO>.Fail(ResourceErrorCodes.Duplicate, "Ja existe um motorista com este documento.");
        }

        entity.Nome = request.Nome;
        entity.Documento = request.Documento;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<MotoristaResponseDTO>.Ok(new MotoristaResponseDTO
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Documento = entity.Documento
        });
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Motoristas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result<bool>.Fail(ResourceErrorCodes.NotFound, "Motorista nao encontrado.");
        }

        dbContext.Motoristas.Remove(entity);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (DbUpdateException)
        {
            return Result<bool>.Fail(ResourceErrorCodes.InUse, "Motorista possui vinculos e nao pode ser removido.");
        }
    }
}
