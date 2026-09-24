namespace TheRealBest.Domain.Entities;

/// <summary>
/// Classe base abstrata para todas as entidades com identificador GUID e timestamps UTC.
/// </summary>
public abstract class EntityBase
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected EntityBase(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}