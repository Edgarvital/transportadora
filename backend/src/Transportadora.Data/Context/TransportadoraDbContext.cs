using Microsoft.EntityFrameworkCore;
using Transportadora.Data.Seed;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Context;

public class TransportadoraDbContext(DbContextOptions<TransportadoraDbContext> options) : DbContext(options)
{
    public virtual DbSet<Role> Roles => Set<Role>();
    public virtual DbSet<User> Users => Set<User>();
    public virtual DbSet<Motorista> Motoristas => Set<Motorista>();
    public virtual DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public virtual DbSet<Endereco> Enderecos => Set<Endereco>();
    public virtual DbSet<Carga> Cargas => Set<Carga>();
    public virtual DbSet<SolicitacaoColeta> SolicitacoesColeta => Set<SolicitacaoColeta>();
    public virtual DbSet<Ocorrencia> Ocorrencias => Set<Ocorrencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransportadoraDbContext).Assembly);
        modelBuilder.SeedDefaults();
        base.OnModelCreating(modelBuilder);
    }
}
