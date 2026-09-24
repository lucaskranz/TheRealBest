namespace TheRealBest.Domain.Entities;

using TheRealBest.Domain.Enums;

public class ScoringWeightRule : EntityBase
{
    public PlayerPosition Position { get; private set; }
    public ActionType ActionType { get; private set; }
    public decimal WeightValue { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsPenalty { get; private set; }
    public int Version { get; private set; } = 1;

    protected ScoringWeightRule() { }

    public ScoringWeightRule(
        PlayerPosition position,
        ActionType actionType,
        decimal weightValue,
        string description,
        bool isPenalty,
        int version = 1,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Position = position;
        ActionType = actionType;
        WeightValue = weightValue;
        Description = description;
        IsPenalty = isPenalty;
        Version = version;
    }

    public void UpdateWeight(decimal newWeight, string description)
    {
        WeightValue = newWeight;
        Description = description;
        Version++;
        MarkUpdated();
    }
}