namespace TheRealBest.API.Models;

public sealed class ApiMeta
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public int? TotalCount { get; init; }
    public int? TotalPages { get; init; }
    public bool? HasNextPage { get; init; }
    public bool? HasPreviousPage { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string Version { get; init; } = "v1";
}