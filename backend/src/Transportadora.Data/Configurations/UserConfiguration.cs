using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transportadora.Models.Entities;

namespace Transportadora.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Nome).HasColumnName("Nome").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Documento).HasColumnName("Documento").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Email).HasColumnName("Email").HasMaxLength(200);
        builder.Property(x => x.SenhaHash).HasColumnName("SenhaHash").HasMaxLength(255);
        builder.Property(x => x.RoleId).HasColumnName("Role_ID").IsRequired();
        builder.Property(x => x.EnderecoId).HasColumnName("Endereco_ID").IsRequired();
        builder.Property(x => x.Situacao).HasColumnName("Situacao").HasConversion<string>().IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Endereco)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.EnderecoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
