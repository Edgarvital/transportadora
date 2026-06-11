using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Configurations;

public class MotoristaConfiguration : IEntityTypeConfiguration<Motorista>
{
    public void Configure(EntityTypeBuilder<Motorista> builder)
    {
        builder.ToTable("Motorista");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Nome).HasColumnName("Nome").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Documento).HasColumnName("Documento").HasMaxLength(30).IsRequired();
    }
}
