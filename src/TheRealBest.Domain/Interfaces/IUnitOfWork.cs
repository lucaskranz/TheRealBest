namespace TheRealBest.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Descarta as entidades rastreadas. Usar entre lotes de processamentos longos (importação de temporada): cada gravação
    /// percorre tudo o que está rastreado, e o custo cresceria a cada partida. Alterações não gravadas são perdidas.
    /// </summary>
    void ClearTracking();
}