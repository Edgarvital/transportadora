using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Configurations;

public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculo");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Modelo).HasColumnName("Modelo").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Placa).HasColumnName("Placa").HasMaxLength(10).IsRequired();

        builder.HasIndex(x => x.Placa).IsUnique();
    }
}
