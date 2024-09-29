using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InertiaNetCore.Ssr;

internal interface IGateway
{
    public Task<SsrResponse?> Dispatch(object model, string url);
}

internal class Gateway(IHttpClientFactory httpClientFactory) : IGateway
{
    public async Task<SsrResponse?> Dispatch(dynamic model, string url)
    {
        var json = JsonSerializer.Serialize(model,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

        var client = httpClientFactory.CreateClient();
        var response = await client.PostAsync(url, content);
        return await response.Content.ReadFromJsonAsync<SsrResponse>();
    }
}
