using System;
using System.Net.Http;
using System.Threading.Tasks;

public class ApiService : IDataService
{
    private readonly HttpClient _client;

    public ApiService()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://103.104.122.137:9999/api/"); // Thay đổi thành địa chỉ Node.js server của bạn
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetAsync(string endpoint)
    {
        HttpResponseMessage response = await _client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
