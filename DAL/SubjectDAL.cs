using System;
using System.Net.Http;
using System.Threading.Tasks;

public class SubjectDAL
{
    private readonly IDataService _dataService;

    public SubjectDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetAllSubjects()
    {
        try
        {
            string subjectsJson = await _dataService.GetAsync("subject");
            return subjectsJson;
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
            throw new Exception("Last time update API endpoint not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception("Error fetching last time update from API.", ex);
        }
    }
}
