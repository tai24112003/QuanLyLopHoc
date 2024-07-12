using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class SettingDAL
{
    private readonly IDataService _dataService;

    public SettingDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetLastTimeUpdateFromDB()
    {
        try
        {
            string lastTimeUpdateJson = await _dataService.GetAsync("setting/getSetting");
            return lastTimeUpdateJson;
        }
        catch (HttpRequestException ex)
        {
            // Handle 404 error (Not Found)
            throw new Exception("Last time update API endpoint not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception("Error fetching last time update from API.", ex);
        }
    }

}

