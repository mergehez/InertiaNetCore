using System.Net.Http.Json;
using System.Text;
using InertiaNetCore.Models;
using Microsoft.Extensions.Options;

namespace InertiaNetCore.Ssr;

internal class SsrGateway(IHttpClientFactory httpClientFactory, IOptions<InertiaOptions> options)
{
    public async Task<SsrResponse?> Dispatch(dynamic model, string url)
    {
        var json = options.Value.JsonSerializeFn(model);
        
        var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

        var client = httpClientFactory.CreateClient();
        var response = await client.PostAsync(url, content);
        return await response.Content.ReadFromJsonAsync<SsrResponse>();
    }
}
