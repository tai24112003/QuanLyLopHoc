using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class ClassStudentDAL
{
    private readonly IDataService _dataService;

    public ClassStudentDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetAllClassStudents()
    {
        try
        {
            string ClassStudentsJson = await _dataService.GetAsync("class_student");
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> InsertClassStudent(List<ClassStudent> student)
    {
        try
        {
            string studentJson = JsonConvert.SerializeObject(student);
            string responseJson = await _dataService.PostAsync("class_student/insert", studentJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting ClassStudent in DAL", ex);
        }
    }

}
