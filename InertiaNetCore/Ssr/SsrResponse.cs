using Microsoft.AspNetCore.Html;

namespace InertiaNetCore.Ssr;

internal readonly record struct SsrResponse()
{
    public IEnumerable<string> Head { get; init; } = [];
    public string Body { get; init; } = default!;

    public IHtmlContent GetBody() => new HtmlString(Body);

    public IHtmlContent GetHead() => new HtmlString(string.Join("\n", Head));
}
