using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Transportadora.Data.Context;

public class TransportadoraDbContextFactory : IDesignTimeDbContextFactory<TransportadoraDbContext>
{
    public TransportadoraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TransportadoraDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULT")
            ?? "Host=localhost;Port=5433;Database=TransportadoraDB;Username=admin;Password=adminpassword";

        optionsBuilder.UseNpgsql(connectionString);

        return new TransportadoraDbContext(optionsBuilder.Options);
    }
}
