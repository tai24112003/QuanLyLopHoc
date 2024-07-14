using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class ClassDAL
{
    private readonly IDataService _dataService;

    public ClassDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetAllClass()
    {
        try
        {
            string ClassJson = await _dataService.GetAsync("class");
            return ClassJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> InsertClass(Class classSession)
    {
        try
        {
            string classJson = JsonConvert.SerializeObject(classSession);
            string responseJson = await _dataService.PostAsync("class/", classJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class in DAL", ex);
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
            throw new Exception("Last time update API endpoint not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception("Error fetching last time update from API.", ex);
        }
    }
}
