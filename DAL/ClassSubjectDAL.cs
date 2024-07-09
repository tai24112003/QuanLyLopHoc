//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Threading.Tasks;

//public class ClassSubjectDAL
//{
//    private readonly IDataService _dataService;

//    public ClassSubjectDAL(IDataService dataService)
//    {
//        _dataService = dataService;
//    }

//    public async Task<string> GetAllClassSubjects()
//    {
//        try
//        {
//            string ClassSubjectsJson = await _dataService.GetAsync("class_subject");
//            return ClassSubjectsJson;
//        }
//        catch (Exception ex)
//        {
//            throw ex;
//        }
//    }
//    public async Task<string> InsertClassSubject(ClassSubject student)
//    {
//        try
//        {
//            string studentJson = JsonConvert.SerializeObject(student);
//            string responseJson = await _dataService.PostAsync("class_subject/insert", studentJson);
//            return responseJson;
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("Error inserting ClassSubject in DAL", ex);
//        }
//    }

//}
