using System.Net.Http.Headers;
using WebMVC.Config;

namespace WebMVC.Clients;

public class PlateClient : IPlateClient
{
    public HttpClient Client { get; }

    private readonly PlateServiceConfig _config;

    public PlateClient(HttpClient client, IOptions<PlateServiceConfig> config)
    {
        _config = config.Value;
        Client = client;
        Client.BaseAddress = new Uri(_config.BaseAddress);
        Client.Timeout = new TimeSpan(0, 0, _config.Timeout);

        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    
    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        return await Client.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string endpoint, T data)
    {
        return await Client.PutAsJsonAsync<T>(endpoint, data);
    }
}
