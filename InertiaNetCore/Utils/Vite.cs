using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using InertiaNetCore.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace InertiaNetCore.Utils;

internal class ViteBuilder(IOptions<ViteOptions> options) : IViteBuilder
{
    /// <summary>
    /// Get the public directory and build path.
    /// </summary>
    private string GetPublicPathForFile(string path)
    {
        var pieces = new List<string> { options.Value.PublicDirectory };
        if (!string.IsNullOrEmpty(options.Value.BuildDirectory))
        {
            pieces.Add(options.Value.BuildDirectory);
        }

        pieces.Add(path);
        return string.Join("/", pieces);
    }

    /// <summary>
    /// Generates various tags from a given input file path.
    /// </summary>
    public HtmlString Input(string path)
    {
        if (IsRunningHot())
        {
            return new HtmlString(MakeModuleTag("@vite/client").Value + MakeModuleTag(path).Value);
        }

        if (!File.Exists(GetPublicPathForFile(options.Value.ManifestFilename)))
        {
            throw new Exception("Vite Manifest is missing. Run `npm run build` and try again.");
        }

        var manifest = File.ReadAllText(GetPublicPathForFile(options.Value.ManifestFilename));
        var manifestJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(manifest);

        if (manifestJson == null)
        {
            throw new Exception("Vite Manifest is invalid. Run `npm run build` and try again.");
        }

        if (!manifestJson.TryGetValue(path, out var manifestPath))
        {
            throw new Exception("Asset not found in manifest: " + path);
        }

        var filePath = manifestPath.GetProperty("file");

        if (IsCssPath(filePath.ToString()))
        {
            return MakeTag(filePath.ToString());
        }

        var html = MakeTag(filePath.ToString());

        try
        {
            var css = manifestPath.GetProperty("css");
            return css.EnumerateArray().Aggregate(html,
                (current, item) => new HtmlString(current.Value + MakeTag(item.ToString()).Value));
        }
        catch (Exception)
        {
            // ignored
        }

        return html;
    }

    /// <summary>
    /// Generate script tag with type="module"
    /// </summary>
    private HtmlString MakeModuleTag(string path)
    {
        var builder = new TagBuilder("script");
        builder.Attributes.Add("type", "module");
        builder.Attributes.Add("src", Asset(path));

        return new HtmlString(GetString(builder) + "\n\t");
    }

    /// <summary>
    /// Generate an appropriate tag for the given URL in HMR mode.
    /// </summary>
    private HtmlString MakeTag(string url)
    {
        return IsCssPath(url) ? MakeStylesheetTag(url) : MakeModuleTag(url);
    }

    /// <summary>
    /// Generate a stylesheet tag for the given URL in HMR mode.
    /// </summary>
    private HtmlString MakeStylesheetTag(string filePath)
    {
        var builder = new TagBuilder("link");
        builder.Attributes.Add("rel", "stylesheet");
        builder.Attributes.Add("href", Asset(filePath));
        return new HtmlString(GetString(builder).Replace("></link>", " />") + "\n\t");
    }

    /// <summary>
    /// Determine whether the given path is a CSS file.
    /// </summary>
    private static bool IsCssPath(string path)
    {
        return Regex.IsMatch(path, @".\.(css|less|sass|scss|styl|stylus|pcss|postcss)", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Generate React refresh runtime script.
    /// </summary>
    public HtmlString ReactRefresh()
    {
        if (!IsRunningHot())
        {
            return new HtmlString("<!-- no hot -->");
        }

        var builder = new TagBuilder("script");
        builder.Attributes.Add("type", "module");

        var inner = $"import RefreshRuntime from '{Asset("@react-refresh")}';" +
                    "RefreshRuntime.injectIntoGlobalHook(window);" +
                    "window.$RefreshReg$ = () => { };" +
                    "window.$RefreshSig$ = () => (type) => type;" +
                    "window.__vite_plugin_react_preamble_installed__ = true;";

        builder.InnerHtml.AppendHtml(inner);

        return new HtmlString(GetString(builder));
    }

    /// <summary>
    /// Get the URL to a given asset when running in HMR mode.
    /// </summary>
    private string HotAsset(string path)
    {
        var hotFilePath = GetPublicPathForFile(options.Value.HotFile);
        var hotContents = File.ReadAllText(hotFilePath);

        return hotContents + "/" + path;
    }

    /// <summary>
    /// Get the URL for an asset.
    /// </summary>
    private string Asset(string path)
    {
        if (IsRunningHot())
        {
            return HotAsset(path);
        }

        var pieces = new List<string>();
        if (!string.IsNullOrEmpty(options.Value.BuildDirectory))
        {
            pieces.Add(options.Value.BuildDirectory);
        }

        pieces.Add(path);
        return "/" + string.Join("/", pieces);
    }

    /// <summary>
    /// Determine if Vite is running in HMR mode.
    /// </summary>
    private bool IsRunningHot()
    {
        return File.Exists(GetPublicPathForFile(options.Value.HotFile));
    }

    /// <summary>
    /// Convert an IHtmlContent to a string.
    /// </summary>
    private static string GetString(IHtmlContent content)
    {
        var writer = new StringWriter();
        content.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }
}

public static class Vite
{
    private static IViteBuilder _instance = default!;

    internal static void UseBuilder(IViteBuilder instance) => _instance = instance;

    /// <summary>
    /// Generates various tags from a given input file path.
    /// </summary>
    public static HtmlString Input(string path) => _instance.Input(path);

    /// <summary>
    /// Generate React refresh runtime script.
    /// </summary>
    public static HtmlString ReactRefresh() => _instance.ReactRefresh();
}
