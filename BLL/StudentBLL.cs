using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class StudentBLL
{
    private readonly StudentDAL _StudentDAL;

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
            Console.WriteLine("Error inserting Student in BLL: " + ex.Message);

            // Save to local if insertion fails
            var studentResponse = new StudentResponse { data = classSession };
            string studentJson = JsonConvert.SerializeObject(studentResponse);
            _StudentDAL.SaveLocalData(studentJson);

            throw new Exception("Error inserting Student in BLL. Data saved locally.", ex);
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
            string StudentsJson =_StudentDAL.LoadLocalData();
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
            // Get Students from server
            string StudentsJson = await _StudentDAL.GetAllStudents();
            // Save Students and last update time to local database
            _StudentDAL.SaveLocalData(StudentsJson);
            return StudentsJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching Students from BLL", ex);
        }
    }

  
}
