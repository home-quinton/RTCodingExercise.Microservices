namespace WebMVC.Clients;

public interface IPlateClient
{
    //these pass-through methods make it easier to mock during unit testing
    Task<HttpResponseMessage> GetAsync(string endpoint);
    Task<HttpResponseMessage> PutAsJsonAsync<T>(string endpoint, T data);
}
