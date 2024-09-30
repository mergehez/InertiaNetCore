namespace InertiaNetCore.Models;

public readonly record struct InertiaPage()
{
    public InertiaProps Props { get; init; } = default!;
    public string Component { get; init; } = default!;
    public string? Version { get; init; } = null;
    public string Url { get; init; } = default!;
}
