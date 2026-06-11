using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Configurations;

public class OcorrenciaConfiguration : IEntityTypeConfiguration<Ocorrencia>
{
    public void Configure(EntityTypeBuilder<Ocorrencia> builder)
    {
        builder.ToTable("Ocorrencia");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.SolicitacaoId).HasColumnName("Solicitacao_ID").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("User_ID").IsRequired();
        builder.Property(x => x.Tipo).HasColumnName("Tipo").HasConversion<string>().IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("Descricao").HasColumnType("text").IsRequired();
        builder.Property(x => x.DataHora).HasColumnName("Data_Hora").IsRequired();

        builder.HasOne(x => x.Solicitacao)
            .WithMany(x => x.Ocorrencias)
            .HasForeignKey(x => x.SolicitacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Ocorrencias)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
