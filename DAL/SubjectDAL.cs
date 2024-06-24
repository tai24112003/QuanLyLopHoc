using System;
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
}
