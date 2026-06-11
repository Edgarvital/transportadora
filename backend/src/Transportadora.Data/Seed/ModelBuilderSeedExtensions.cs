using Microsoft.EntityFrameworkCore;
using Transportadora.Models.Entities;
using Transportadora.Models.Enums;

namespace Transportadora.Data.Seed;

public static class ModelBuilderSeedExtensions
{
    public static void SeedDefaults(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Nome = RoleTipo.Cliente },
            new Role { Id = 2, Nome = RoleTipo.Atendente },
            new Role { Id = 3, Nome = RoleTipo.Admin }
        );
    }
}
