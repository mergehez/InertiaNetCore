namespace InertiaNetCore.Models;

public readonly record struct InertiaPage
{
    public required InertiaProps Props { get; init; }
    public required Dictionary<string, List<string>> DeferredProps { get; init; }
    public required List<string> MergeProps { get; init; }
    public required string Component { get; init; }
    public required string? Version { get; init; }
    public required string Url { get; init; }
}
