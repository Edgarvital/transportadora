using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Configurations;

public class SolicitacaoColetaConfiguration : IEntityTypeConfiguration<SolicitacaoColeta>
{
    public void Configure(EntityTypeBuilder<SolicitacaoColeta> builder)
    {
        builder.ToTable("Solicitacao_Coleta");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.RemetenteId).HasColumnName("Remetente_ID").IsRequired();
        builder.Property(x => x.DestinatarioId).HasColumnName("Destinatario_ID").IsRequired();
        builder.Property(x => x.CargaId).HasColumnName("Carga_ID").IsRequired();
        builder.Property(x => x.CriadoPorUserId).HasColumnName("Criado_Por_User_ID").IsRequired();
        builder.Property(x => x.DataSolicitacao).HasColumnName("Data_Solicitacao").IsRequired();
        builder.Property(x => x.DataPrevistaRetirada).HasColumnName("Data_Prevista_Retirada").IsRequired();
        builder.Property(x => x.Prioridade).HasColumnName("Prioridade").HasConversion<string>().IsRequired();
        builder.Property(x => x.Status).HasColumnName("Status").HasConversion<string>().IsRequired();
        builder.Property(x => x.MotoristaId).HasColumnName("Motorista_ID");
        builder.Property(x => x.VeiculoId).HasColumnName("Veiculo_ID");
        builder.Property(x => x.ObservacoesGerais).HasColumnName("Observacoes_Gerais").HasColumnType("text");

        builder.HasOne(x => x.Remetente)
            .WithMany(x => x.SolicitacoesComoRemetente)
            .HasForeignKey(x => x.RemetenteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Destinatario)
            .WithMany(x => x.SolicitacoesComoDestinatario)
            .HasForeignKey(x => x.DestinatarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUser)
            .WithMany(x => x.SolicitacoesCriadas)
            .HasForeignKey(x => x.CriadoPorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Motorista)
            .WithMany(x => x.Solicitacoes)
            .HasForeignKey(x => x.MotoristaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veiculo)
            .WithMany(x => x.Solicitacoes)
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Carga)
            .WithOne(x => x.SolicitacaoColeta)
            .HasForeignKey<SolicitacaoColeta>(x => x.CargaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CargaId).IsUnique();
    }
}
