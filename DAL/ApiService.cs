using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ApiService : IDataService
{
    private readonly HttpClient _client;

    public ApiService()
    {
        _client = new HttpClient();
        //_client.BaseAddress = new Uri($"http://localhost:9999/api/"); 
        _client.BaseAddress = new Uri($"https://ea37-171-236-49-104.ngrok-free.app/api/"); 
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void UpdateBaseAddress(string baseUrl)
    {
        if (Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            _client.BaseAddress = uri;
        }
        else
        {
            throw new ArgumentException("Invalid URL");
        }
    }

    public async Task<string> GetAsync(string endpoint)
    {
        HttpResponseMessage response = await _client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string endpoint, string jsonContent)
    {
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PutAsync(string endpoint, string jsonContent)
    {
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PutAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteAsync(string endpoint)
    {
        HttpResponseMessage response = await _client.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
