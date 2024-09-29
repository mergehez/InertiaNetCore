using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Models;

public class InertiaOptions
{
    public string RootView { get; set; } = "~/Views/App.cshtml";

    public bool SsrEnabled { get; set; } = false;
    public string SsrUrl { get; set; } = "http://127.0.0.1:13714/render";

    public Func<InertiaPage?, Task<IActionResult>> JsonResultResolver { get; set; } = page => Task.FromResult<IActionResult>(new JsonResult(page, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    }));
}
