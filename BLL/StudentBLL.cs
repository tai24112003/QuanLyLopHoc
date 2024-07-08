using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class StudentBLL
{
    private readonly StudentDAL _StudentDAL;
    private readonly string localFilePath = "localStudents.json";

    public StudentBLL(StudentDAL StudentDAL)
    {
        _StudentDAL = StudentDAL ?? throw new ArgumentNullException(nameof(StudentDAL));
    }
    public async Task<StudentResponse> InsertStudent(List<Student> classSession)
    {
        try
        {
            string responseJson = await _StudentDAL.InsertListStudent(classSession);
            var insertedSession = JsonConvert.DeserializeObject<StudentResponse>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting Student in BLL: " + ex);
            throw new Exception("Error inserting Student in BLL", ex);
        }
    }
    public async Task<List<Student>> GetAllStudents()
    {
        try
        {
            string StudentsJson = await GetStudents();
            StudentResponse StudentResponse = JsonConvert.DeserializeObject<StudentResponse>(StudentsJson);
            return StudentResponse.data;
        }
        catch (Exception ex)
        {
            string StudentsJson = LoadLocalData();
            if (!string.IsNullOrEmpty(StudentsJson))
            {
                StudentResponse StudentResponse = JsonConvert.DeserializeObject<StudentResponse>(StudentsJson);
                return StudentResponse.data;
            }
            throw new Exception("Error fetching Students from API and local data", ex);
        }
    }

    public async Task<string> GetStudents()
    {
        try
        {


            if (File.Exists(localFilePath))
            {
                // Load Students from local file
                Console.WriteLine("load local");
                return LoadLocalData();
            }
            else
            {
                Console.WriteLine("load api");

                // Get Students from server
                string StudentsJson = await _StudentDAL.GetAllStudents();
                // Save Students and last update time to local file
                SaveLocalData(StudentsJson);
                return StudentsJson;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching Students from BLL", ex);
        }
    }

    private void SaveLocalData(string StudentsJson)
    {
        var localData = new LocalDataResponse
        {
            StudentsJson = StudentsJson,
        };
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localData));
    }

    private string LoadLocalData()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.StudentsJson;
        }
        return null;
    }
}
