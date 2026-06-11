using Microsoft.EntityFrameworkCore;
using Transportadora.Data.Seed;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Context;

public class TransportadoraDbContext(DbContextOptions<TransportadoraDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Motorista> Motoristas => Set<Motorista>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Carga> Cargas => Set<Carga>();
    public DbSet<SolicitacaoColeta> SolicitacoesColeta => Set<SolicitacaoColeta>();
    public DbSet<Ocorrencia> Ocorrencias => Set<Ocorrencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransportadoraDbContext).Assembly);
        modelBuilder.SeedDefaults();
        base.OnModelCreating(modelBuilder);
    }
}
