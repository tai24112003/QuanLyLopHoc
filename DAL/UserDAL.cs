using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class UserDAL
{
    private readonly IDataService _dataService;

    public UserDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetUsersByRole(string role)
    {
        try
        {
            string usersJson = await _dataService.GetAsync($"user/getUserCanTeach");
            return usersJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
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
            Console.WriteLine("Last time update API endpoint not found.", ex);
            return null;
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine("Error fetching last time update from API.", ex);
            return null;
        }
    }

}

