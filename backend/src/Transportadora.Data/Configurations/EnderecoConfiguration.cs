using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Configurations;

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("Endereco");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Cep).HasColumnName("CEP").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Logradouro).HasColumnName("Logradouro").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Numero).HasColumnName("Numero").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Complemento).HasColumnName("Complemento").HasMaxLength(100);
        builder.Property(x => x.Bairro).HasColumnName("Bairro").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Cidade).HasColumnName("Cidade").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Uf).HasColumnName("UF").HasMaxLength(2).IsRequired();
    }
}
