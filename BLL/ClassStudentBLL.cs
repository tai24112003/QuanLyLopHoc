using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ClassStudentBLL
{
    private readonly ClassStudentDAL _ClassStudentDAL;
    private readonly string localFilePath = "localClassStudents.json";

    public ClassStudentBLL(ClassStudentDAL ClassStudentDAL)
    {
        _ClassStudentDAL = ClassStudentDAL ?? throw new ArgumentNullException(nameof(ClassStudentDAL));
    }
    public async Task<ClassStudent> InsertClassStudent(List<ClassStudent> classSession)
    {
        try
        {
            string responseJson = await _ClassStudentDAL.InsertClassStudent(classSession);
            var insertedSession = JsonConvert.DeserializeObject<ClassStudent>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting ClassStudent in BLL: " + ex.Message);
            throw new Exception("Error inserting ClassStudent in BLL", ex);
        }
    }
    public async Task<List<ClassStudent>> GetAllClassStudents()
    {
        try
        {
            string ClassStudentsJson = await GetClassStudents();
            ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
            return ClassStudentResponse.data;
        }
        catch (Exception ex)
        {
            string ClassStudentsJson = LoadLocalData();
            if (!string.IsNullOrEmpty(ClassStudentsJson))
            {
                ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
                return ClassStudentResponse.data;
            }
            throw new Exception("Error fetching ClassStudents from API and local data", ex);
        }
    }
    public async Task<List<ClassStudent>> GetClassStudentsByClassID(int classID)
    {
        List<ClassStudent> allClassStudents = await GetAllClassStudents();
        return allClassStudents.Where(cs => cs.ClassID == classID).ToList();
    }
    public async Task<string> GetClassStudents()
    {
        try
        {


            if (File.Exists(localFilePath))
            {
                // Load ClassStudents from local file
                Console.WriteLine("load local");
                return LoadLocalData();
            }
            else
            {
                Console.WriteLine("load api");

                // Get ClassStudents from server
                string ClassStudentsJson = await _ClassStudentDAL.GetAllClassStudents();
                // Save ClassStudents and last update time to local file
                SaveLocalData(ClassStudentsJson);
                return ClassStudentsJson;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassStudents from BLL", ex);
        }
    }
    public async Task<List<ClassStudent>> GetStudentsByClassID(int classID)
    {
        List<ClassStudent> allStudents = await GetAllClassStudents();
        return allStudents.Where(cs => cs.ClassID == classID).ToList();
    }
    private void SaveLocalData(string ClassStudentsJson)
    {
        var localData = new LocalDataResponse
        {
            ClassStudentsJson = ClassStudentsJson,
        };
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localData));
    }

    private string LoadLocalData()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.ClassStudentsJson;
        }
        return null;
    }
}
