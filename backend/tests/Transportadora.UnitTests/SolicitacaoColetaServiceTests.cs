using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Transportadora.Data.Context;
using Transportadora.Models.Entities;
using Transportadora.Models.Enums;
using Transportadora.Services.Solicitacoes;
using Transportadora.Shared.Errors;
using Xunit;

namespace Transportadora.Tests;

public sealed class SolicitacaoColetaServiceTests
{
    [Fact]
    public async Task GetAllAsync_DeveRetornarSolicitacoesOrdenadasPorDataDesc()
    {
        // Arrange
        var solicitacaoAntiga = CriarSolicitacao(
            id: 1,
            dataSolicitacao: new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc),
            status: StatusColeta.Aberta,
            prioridade: PrioridadeNivel.Media,
            motoristaId: 10,
            veiculoId: 20,
            criadoPorNome: "Criador 1",
            cargaDescricaoNome: "Caixa 1");

        var solicitacaoNova = CriarSolicitacao(
            id: 2,
            dataSolicitacao: new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc),
            status: StatusColeta.Roteirizada,
            prioridade: PrioridadeNivel.Alta,
            motoristaId: 11,
            veiculoId: 21,
            criadoPorNome: "Criador 2",
            cargaDescricaoNome: "Caixa 2");

        var dbContext = CriarDbContext(solicitacaoAntiga, solicitacaoNova);
        var service = new SolicitacaoColetaService(dbContext.Object);

        // Act
        var result = await service.GetAllAsync(new SolicitacaoColetaQueryDTO(null, null, null, null, null, PageNumber: 1, PageSize: 10), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items[0].Id.Should().Be(solicitacaoNova.Id);
        result.Items[1].Id.Should().Be(solicitacaoAntiga.Id);
        result.Items[0].CriadoPorNome.Should().Be("Criador 2");
        result.Items[0].Carga.DescricaoNome.Should().Be("Caixa 2");
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetAllAsync_DeveFiltrarPorStatus()
    {
        // Arrange
        var solicitacaoAberta = CriarSolicitacao(
            id: 1,
            dataSolicitacao: new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc),
            status: StatusColeta.Aberta,
            prioridade: PrioridadeNivel.Baixa,
            motoristaId: null,
            veiculoId: null);

        var solicitacaoRoteirizada = CriarSolicitacao(
            id: 2,
            dataSolicitacao: new DateTime(2026, 1, 11, 8, 0, 0, DateTimeKind.Utc),
            status: StatusColeta.Roteirizada,
            prioridade: PrioridadeNivel.Alta,
            motoristaId: 10,
            veiculoId: 20,
            criadoPorNome: "Criador 2",
            cargaDescricaoNome: "Caixa 2");

        var dbContext = CriarDbContext(solicitacaoAberta, solicitacaoRoteirizada);
        var service = new SolicitacaoColetaService(dbContext.Object);

        // Act
        var result = await service.GetAllAsync(new SolicitacaoColetaQueryDTO(StatusColeta.Roteirizada, null, null, null, null, PageNumber: 1, PageSize: 10), CancellationToken.None);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.Single().Id.Should().Be(solicitacaoRoteirizada.Id);
        result.Items.Single().Status.Should().Be(StatusColeta.Roteirizada);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarSolicitacaoCompleta()
    {
        // Arrange
        var solicitacao = CriarSolicitacao(
            id: 1,
            dataSolicitacao: new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc),
            status: StatusColeta.Roteirizada,
            prioridade: PrioridadeNivel.Alta,
            motoristaId: 10,
            veiculoId: 20,
            criadoPorNome: "Criador 1",
            cargaDescricaoNome: "Caixa 1",
            ocorrencias: new List<Ocorrencia>
            {
                new()
                {
                    Id = 99,
                    SolicitacaoId = 1,
                    UserId = 777,
                    Tipo = OcorrenciaTipo.Atraso,
                    Descricao = "Atraso na coleta",
                    DataHora = new DateTime(2026, 1, 12, 10, 0, 0, DateTimeKind.Utc),
                    User = new User { Id = 777, Nome = "Operador" }
                }
            });

        var dbContext = CriarDbContext(solicitacao);
        var service = new SolicitacaoColetaService(dbContext.Object);

        // Act
        var result = await service.GetByIdAsync(solicitacao.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(solicitacao.Id);
        result.Status.Should().Be(StatusColeta.Roteirizada);
        result.CriadoPorNome.Should().Be("Criador 1");
        result.MotoristaNome.Should().Be("Motorista 1");
        result.VeiculoPlaca.Should().Be("ABC1234");
        result.Carga.DescricaoNome.Should().Be("Caixa 1");
        result.Ocorrencias.Should().ContainSingle();
        result.Ocorrencias.Single().UsuarioNome.Should().Be("Operador");
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoSolicitacaoNaoExistir()
    {
        // Arrange
        var solicitacao = CriarSolicitacao(
            id: 1,
            dataSolicitacao: new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc),
            status: StatusColeta.Aberta,
            prioridade: PrioridadeNivel.Baixa,
            motoristaId: null,
            veiculoId: null,
            criadoPorNome: "Criador 1",
            cargaDescricaoNome: "Caixa 1");

        var dbContext = CriarDbContext(solicitacao);
        var service = new SolicitacaoColetaService(dbContext.Object);

        // Act
        var result = await service.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(StatusColeta.EmColeta)]
    [InlineData(StatusColeta.Coletada)]
    public async Task AtualizarStatusAsync_DeveRetornarErro_QuandoSolicitacaoCanceladaEStatusNovoForEmColetaOuColetada(StatusColeta novoStatus)
    {
        var solicitacao = CriarSolicitacao(StatusColeta.Cancelada, motoristaId: 1, veiculoId: 1);
        var dbContext = CriarDbContext(solicitacao);
        var service = new SolicitacaoColetaService(dbContext.Object);

        var result = await service.AtualizarStatusAsync(solicitacao.Id, novoStatus, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(SolicitacaoErrorCodes.InvalidStatusTransition);
        result.ErrorMessage.Should().Contain("cancelado");
        dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveRetornarErro_QuandoColetadaSemMotorista()
    {
        var solicitacao = CriarSolicitacao(StatusColeta.Roteirizada, motoristaId: null, veiculoId: 10);
        var dbContext = CriarDbContext(solicitacao);
        var service = new SolicitacaoColetaService(dbContext.Object);

        var result = await service.AtualizarStatusAsync(solicitacao.Id, StatusColeta.Coletada, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(SolicitacaoErrorCodes.ColetadaNeedsRouting);
        result.ErrorMessage.Should().Contain("motorista");
        dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveRetornarErro_QuandoColetadaSemVeiculo()
    {
        var solicitacao = CriarSolicitacao(StatusColeta.Roteirizada, motoristaId: 10, veiculoId: null);
        var dbContext = CriarDbContext(solicitacao);
        var service = new SolicitacaoColetaService(dbContext.Object);

        var result = await service.AtualizarStatusAsync(solicitacao.Id, StatusColeta.Coletada, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(SolicitacaoErrorCodes.ColetadaNeedsRouting);
        result.ErrorMessage.Should().Contain("veiculo");
        dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveRetornarSucesso_QuandoColetadaComMotoristaEVeiculo()
    {
        var solicitacao = CriarSolicitacao(StatusColeta.Roteirizada, motoristaId: 10, veiculoId: 20);
        var dbContext = CriarDbContext(solicitacao);
        dbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new SolicitacaoColetaService(dbContext.Object);

        var result = await service.AtualizarStatusAsync(solicitacao.Id, StatusColeta.Coletada, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.ErrorCode.Should().BeNull();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(StatusColeta.Coletada);
        result.Data.MotoristaId.Should().Be(solicitacao.MotoristaId);
        result.Data.VeiculoId.Should().Be(solicitacao.VeiculoId);
        result.Data.Carga.DescricaoNome.Should().Be(solicitacao.Carga.DescricaoNome);
        dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static SolicitacaoColeta CriarSolicitacao(
        StatusColeta status,
        int? motoristaId,
        int? veiculoId,
        int id = 1,
        DateTime? dataSolicitacao = null,
        PrioridadeNivel prioridade = PrioridadeNivel.Alta,
        string criadoPorNome = "Criador 1",
        string cargaDescricaoNome = "Caixa 1",
        IReadOnlyCollection<Ocorrencia>? ocorrencias = null)
    {
        return new SolicitacaoColeta
        {
            Id = id,
            RemetenteId = 101,
            DestinatarioId = 202,
            CargaId = 303,
            CriadoPorUserId = 404,
            DataSolicitacao = dataSolicitacao ?? new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc),
            DataPrevistaRetirada = new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc),
            Prioridade = prioridade,
            Status = status,
            MotoristaId = motoristaId,
            VeiculoId = veiculoId,
            Remetente = new User { Id = 101, Nome = "Remetente" },
            Destinatario = new User { Id = 202, Nome = "Destinatario" },
            CriadoPorUser = new User { Id = 404, Nome = criadoPorNome },
            Carga = new Carga
            {
                Id = 303,
                DescricaoNome = cargaDescricaoNome,
                Tipo = "Frágil",
                Peso = 12m,
                Altura = 10m,
                Largura = 20m,
                Comprimento = 30m
            },
            Motorista = motoristaId.HasValue ? new Motorista { Id = motoristaId.Value, Nome = $"Motorista {id}", Documento = "12345678901" } : null,
            Veiculo = veiculoId.HasValue ? new Veiculo { Id = veiculoId.Value, Modelo = "Fiorino", Placa = "ABC1234" } : null,
            Ocorrencias = ocorrencias?.ToList() ?? new List<Ocorrencia>()
        };
    }

    private static Mock<TransportadoraDbContext> CriarDbContext(params SolicitacaoColeta[] solicitacoes)
    {
        var dbContext = new Mock<TransportadoraDbContext>(new DbContextOptionsBuilder<TransportadoraDbContext>().Options) { CallBase = true };
        dbContext.Setup(x => x.SolicitacoesColeta).ReturnsDbSet(solicitacoes.ToList());
        dbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        return dbContext;
    }
}