namespace TheRealBest.Infrastructure.Data.Seeds;

public interface IRealDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}