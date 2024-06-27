using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class ClassSessionDAL
{
    private readonly IDataService _dataService;

    public ClassSessionDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> InsertClassSession(ClassSession classSession)
    {
        try
        {
            string classSessionJson = JsonConvert.SerializeObject(classSession);
            string responseJson = await _dataService.PostAsync("class_session/insert", classSessionJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class session in DAL", ex);
        }
    }

}

