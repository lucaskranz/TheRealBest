namespace TheRealBest.API.Models;

public sealed class ApiResponse<T>
{
    public T? Data { get; init; }
    public ApiMeta? Meta { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, ApiMeta? meta = null) =>
        new() { Data = data, Meta = meta, Errors = null };

    public static ApiResponse<T> Fail(IEnumerable<string> errors, ApiMeta? meta = null) =>
        new() { Data = default, Meta = meta, Errors = errors.ToList() };

    public static ApiResponse<T> Fail(string error, ApiMeta? meta = null) =>
        new() { Data = default, Meta = meta, Errors = [error] };
}