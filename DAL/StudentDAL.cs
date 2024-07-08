using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StudentDAL
{
    private readonly IDataService _dataService;

    public StudentDAL(IDataService dataService)
    {
        _dataService = dataService;
    }
    public async Task<string> GetAllStudents()
    {
        try
        {
            string ClassJson = await _dataService.GetAsync("student");
            return ClassJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> InsertListStudent(List<Student> student)
    {
        try
        {
            string studentJson = JsonConvert.SerializeObject(student);
            string responseJson = await _dataService.PostAsync("student/insert", studentJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting Student in DAL", ex);
        }
    }
}
