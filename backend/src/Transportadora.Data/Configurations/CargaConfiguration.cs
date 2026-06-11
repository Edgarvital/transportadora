using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Configurations;

public class CargaConfiguration : IEntityTypeConfiguration<Carga>
{
    public void Configure(EntityTypeBuilder<Carga> builder)
    {
        builder.ToTable("Carga");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.DescricaoNome).HasColumnName("Descricao_Nome").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Tipo).HasColumnName("Tipo").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Peso).HasColumnName("Peso").HasPrecision(12, 3).IsRequired();
        builder.Property(x => x.Altura).HasColumnName("Altura").HasPrecision(12, 3).IsRequired();
        builder.Property(x => x.Largura).HasColumnName("Largura").HasPrecision(12, 3).IsRequired();
        builder.Property(x => x.Comprimento).HasColumnName("Comprimento").HasPrecision(12, 3).IsRequired();
    }
}
