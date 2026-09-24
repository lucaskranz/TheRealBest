namespace TheRealBest.Infrastructure.Data.Repositories;

using TheRealBest.Domain.Interfaces;
using TheRealBest.Infrastructure.Data;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}