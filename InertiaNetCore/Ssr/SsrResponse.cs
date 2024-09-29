using Microsoft.AspNetCore.Html;

namespace InertiaNetCore.Ssr;

internal class SsrResponse
{
    public List<string> Head { get; set; } = default!;
    public string Body { get; set; } = default!;

    public IHtmlContent GetBody() => new HtmlString(Body);

    public IHtmlContent GetHead() => new HtmlString(string.Join("\n", Head));
}
