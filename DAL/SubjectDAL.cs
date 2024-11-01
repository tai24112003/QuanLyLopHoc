using Newtonsoft.Json;
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
    public async Task<string> InsertSubject(Subject student)
    {
        try
        {
            string studentJson = JsonConvert.SerializeObject(student);
            string responseJson = await _dataService.PostAsync("subject/", studentJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting Subject in DAL"+ ex);
            return null;
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
