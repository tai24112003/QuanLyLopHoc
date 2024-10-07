using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class SessionDAL
{
    private readonly IDataService _dataService;

    public SessionDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetSession()
    {
        try
        {
            string lastTimeUpdateJson = await _dataService.GetAsync("Session/getSession");
            return lastTimeUpdateJson;
        }
        catch (HttpRequestException ex)
        {
            // Handle 404 error (Not Found)
            throw new Exception("Get Session API endpoint not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception("Error fetching session from API.", ex);
        }
    }

}

