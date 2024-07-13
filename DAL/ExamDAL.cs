using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class ExamDAL
{
    private readonly IDataService _dataService;

    public ExamDAL(IDataService dataService)
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
    public async Task<string> getExam(int id)
    {
        try
        {
            string responseJson = await _dataService.GetAsync("public/exam/"+id.ToString());
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class in DAL", ex);
        }
    }

    public async Task<string> getListExam()
    {
        try
        {
            string responseJson = await _dataService.GetAsync("public/exam/list/1");
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class in DAL", ex);
        }
    }

}
