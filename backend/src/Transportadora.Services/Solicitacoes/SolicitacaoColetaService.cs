using Microsoft.EntityFrameworkCore;
using Transportadora.Data.Context;
using Transportadora.DTOs.Solicitacoes;
using Transportadora.Models.Entities;
using Transportadora.Models.Enums;
using Transportadora.Shared.Errors;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Solicitacoes;

public sealed class SolicitacaoColetaService(TransportadoraDbContext dbContext) : ISolicitacaoColetaService
{
    public async Task<PagedResult<SolicitacaoColetaResponseDTO>> GetAllAsync(
        SolicitacaoColetaQueryDTO queryDto,
        CancellationToken cancellationToken)
    {
        var pageNumber = queryDto.PageNumber < 1 ? 1 : queryDto.PageNumber;
        var pageSize = queryDto.PageSize < 1 ? 10 : queryDto.PageSize > 100 ? 100 : queryDto.PageSize;

        var query = dbContext.SolicitacoesColeta
            .AsNoTracking()
            .Include(x => x.Carga)
            .Include(x => x.Remetente)
            .Include(x => x.Destinatario)
            .Include(x => x.CriadoPorUser)
            .Include(x => x.Motorista)
            .Include(x => x.Veiculo)
            .Include(x => x.Ocorrencias)
            .ThenInclude(x => x.User)
            .AsQueryable();

        if (queryDto.Status.HasValue)
        {
            query = query.Where(x => x.Status == queryDto.Status.Value);
        }

        if (queryDto.Prioridade.HasValue)
        {
            query = query.Where(x => x.Prioridade == queryDto.Prioridade.Value);
        }

        if (queryDto.RemetenteId.HasValue)
        {
            query = query.Where(x => x.RemetenteId == queryDto.RemetenteId.Value);
        }

        if (queryDto.DestinatarioId.HasValue)
        {
            query = query.Where(x => x.DestinatarioId == queryDto.DestinatarioId.Value);
        }

        if (queryDto.DataSolicitacaoInicial.HasValue)
        {
            query = query.Where(x => x.DataSolicitacao >= queryDto.DataSolicitacaoInicial.Value);
        }

        if (queryDto.DataSolicitacaoFinal.HasValue)
        {
            query = query.Where(x => x.DataSolicitacao <= queryDto.DataSolicitacaoFinal.Value);
        }

        if (queryDto.DataPrevistaRetiradaInicial.HasValue)
        {
            query = query.Where(x => x.DataPrevistaRetirada >= queryDto.DataPrevistaRetiradaInicial.Value);
        }

        if (queryDto.DataPrevistaRetiradaFinal.HasValue)
        {
            query = query.Where(x => x.DataPrevistaRetirada <= queryDto.DataPrevistaRetiradaFinal.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var entities = await query
            .OrderByDescending(x => x.DataSolicitacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<SolicitacaoColetaResponseDTO>.Create(
            entities.Select(Map).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task<SolicitacaoColetaResponseDTO?> GetByIdAsync(int solicitacaoId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SolicitacoesColeta
            .AsNoTracking()
            .Include(x => x.Carga)
            .Include(x => x.Remetente)
            .Include(x => x.Destinatario)
            .Include(x => x.CriadoPorUser)
            .Include(x => x.Motorista)
            .Include(x => x.Veiculo)
            .Include(x => x.Ocorrencias)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == solicitacaoId, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<Result<SolicitacaoColetaResponseDTO>> CriarAsync(
        SolicitacaoColetaCreateRequestDTO dto,
        int criadoPorUserId,
        CancellationToken cancellationToken)
    {
        var remetenteExiste = await dbContext.Users.AnyAsync(x => x.Id == dto.RemetenteId, cancellationToken);
        if (!remetenteExiste)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.RemetenteNotFound, "Remetente nao encontrado.");
        }

        var destinatarioExiste = await dbContext.Users.AnyAsync(x => x.Id == dto.DestinatarioId, cancellationToken);
        if (!destinatarioExiste)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.DestinatarioNotFound, "Destinatario nao encontrado.");
        }

        var criadorExiste = await dbContext.Users.AnyAsync(x => x.Id == criadoPorUserId, cancellationToken);
        if (!criadorExiste)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.CriadorNotFound, "Usuario criador nao encontrado.");
        }

        var carga = new Carga
        {
            DescricaoNome = dto.Carga.DescricaoNome,
            Tipo = dto.Carga.Tipo,
            Peso = dto.Carga.Peso,
            Altura = dto.Carga.Altura,
            Largura = dto.Carga.Largura,
            Comprimento = dto.Carga.Comprimento
        };

        dbContext.Cargas.Add(carga);
        await dbContext.SaveChangesAsync(cancellationToken);

        var solicitacao = new SolicitacaoColeta
        {
            RemetenteId = dto.RemetenteId,
            DestinatarioId = dto.DestinatarioId,
            CargaId = carga.Id,
            CriadoPorUserId = criadoPorUserId,
            DataSolicitacao = DateTime.UtcNow,
            DataPrevistaRetirada = dto.DataPrevistaRetirada,
            Prioridade = dto.Prioridade,
            Status = StatusColeta.Aberta,
            ObservacoesGerais = dto.ObservacoesGerais
        };

        dbContext.SolicitacoesColeta.Add(solicitacao);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await LoadResultAsync(solicitacao.Id, cancellationToken);
    }

    public async Task<Result<SolicitacaoColetaResponseDTO>> RoteirizarAsync(
        int solicitacaoId,
        RoteirizarRequestDTO dto,
        CancellationToken cancellationToken)
    {
        var solicitacao = await dbContext.SolicitacoesColeta.FirstOrDefaultAsync(x => x.Id == solicitacaoId, cancellationToken);
        if (solicitacao is null)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.SolicitationNotFound, "Solicitacao nao encontrada.");
        }

        var motoristaExiste = await dbContext.Motoristas.AnyAsync(x => x.Id == dto.MotoristaId, cancellationToken);
        if (!motoristaExiste)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.MotoristaNotFound, "Motorista nao encontrado.");
        }

        var veiculoExiste = await dbContext.Veiculos.AnyAsync(x => x.Id == dto.VeiculoId, cancellationToken);
        if (!veiculoExiste)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.VeiculoNotFound, "Veiculo nao encontrado.");
        }

        solicitacao.MotoristaId = dto.MotoristaId;
        solicitacao.VeiculoId = dto.VeiculoId;
        solicitacao.Status = StatusColeta.Roteirizada;

        await dbContext.SaveChangesAsync(cancellationToken);

        return await LoadResultAsync(solicitacao.Id, cancellationToken);
    }

    public async Task<Result<SolicitacaoColetaResponseDTO>> RegistrarOcorrenciaAsync(
        int solicitacaoId,
        int userId,
        OcorrenciaRequestDTO dto,
        CancellationToken cancellationToken)
    {
        var solicitacaoExiste = await dbContext.SolicitacoesColeta.AnyAsync(x => x.Id == solicitacaoId, cancellationToken);
        if (!solicitacaoExiste)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.SolicitationNotFound, "Solicitacao nao encontrada.");
        }

        var usuarioExiste = await dbContext.Users.AnyAsync(x => x.Id == userId, cancellationToken);
        if (!usuarioExiste)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.CriadorNotFound, "Usuario nao encontrado.");
        }

        var ocorrencia = new Ocorrencia
        {
            SolicitacaoId = solicitacaoId,
            UserId = userId,
            Tipo = dto.Tipo,
            Descricao = dto.Descricao,
            DataHora = DateTime.UtcNow
        };

        dbContext.Ocorrencias.Add(ocorrencia);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await LoadResultAsync(solicitacaoId, cancellationToken);
    }

    public async Task<Result<SolicitacaoColetaResponseDTO>> AtualizarStatusAsync(
        int solicitacaoId,
        StatusColeta novoStatus,
        CancellationToken cancellationToken)
    {
        var solicitacao = await dbContext.SolicitacoesColeta.FirstOrDefaultAsync(x => x.Id == solicitacaoId, cancellationToken);
        if (solicitacao is null)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.SolicitationNotFound, "Solicitacao nao encontrada.");
        }

        if (solicitacao.Status == StatusColeta.Cancelada
            && (novoStatus == StatusColeta.EmColeta || novoStatus == StatusColeta.Coletada))
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(
                SolicitacaoErrorCodes.InvalidStatusTransition,
                "Pedido cancelado nao pode voltar para Em Coleta ou Coletado");
        }

        if (novoStatus == StatusColeta.Coletada
            && (!solicitacao.MotoristaId.HasValue || !solicitacao.VeiculoId.HasValue))
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(
                SolicitacaoErrorCodes.ColetadaNeedsRouting,
                "Nao e permitido marcar uma coleta como Coletada sem que haja motorista e veiculo vinculados");
        }

        solicitacao.Status = novoStatus;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await LoadResultAsync(solicitacaoId, cancellationToken);
    }

    private async Task<Result<SolicitacaoColetaResponseDTO>> LoadResultAsync(int solicitacaoId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SolicitacoesColeta
            .AsNoTracking()
            .Include(x => x.Carga)
            .Include(x => x.Remetente)
            .Include(x => x.Destinatario)
            .Include(x => x.CriadoPorUser)
            .Include(x => x.Motorista)
            .Include(x => x.Veiculo)
            .Include(x => x.Ocorrencias)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == solicitacaoId, cancellationToken);

        if (entity is null)
        {
            return Result<SolicitacaoColetaResponseDTO>.Fail(SolicitacaoErrorCodes.SolicitationNotFound, "Solicitacao nao encontrada.");
        }

        return Result<SolicitacaoColetaResponseDTO>.Ok(Map(entity));
    }

    private static SolicitacaoColetaResponseDTO Map(SolicitacaoColeta entity)
    {
        return new SolicitacaoColetaResponseDTO
        {
            Id = entity.Id,
            RemetenteId = entity.RemetenteId,
            RemetenteNome = entity.Remetente.Nome,
            DestinatarioId = entity.DestinatarioId,
            DestinatarioNome = entity.Destinatario.Nome,
            CriadoPorUserId = entity.CriadoPorUserId,
            CriadoPorNome = entity.CriadoPorUser.Nome,
            DataSolicitacao = entity.DataSolicitacao,
            DataPrevistaRetirada = entity.DataPrevistaRetirada,
            Prioridade = entity.Prioridade,
            Status = entity.Status,
            MotoristaId = entity.MotoristaId,
            MotoristaNome = entity.Motorista?.Nome,
            VeiculoId = entity.VeiculoId,
            VeiculoModelo = entity.Veiculo?.Modelo,
            VeiculoPlaca = entity.Veiculo?.Placa,
            ObservacoesGerais = entity.ObservacoesGerais,
            Carga = new CargaResponseDTO
            {
                Id = entity.Carga.Id,
                DescricaoNome = entity.Carga.DescricaoNome,
                Tipo = entity.Carga.Tipo,
                Peso = entity.Carga.Peso,
                Altura = entity.Carga.Altura,
                Largura = entity.Carga.Largura,
                Comprimento = entity.Carga.Comprimento
            },
            Ocorrencias = entity.Ocorrencias
                .OrderByDescending(x => x.DataHora)
                .Select(x => new OcorrenciaResponseDTO
                {
                    Id = x.Id,
                    Tipo = x.Tipo,
                    Descricao = x.Descricao,
                    DataHora = x.DataHora,
                    UserId = x.UserId,
                    UsuarioNome = x.User.Nome
                })
                .ToList()
        };
    }
}
