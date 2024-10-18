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
            InsertStudentLocal(classSession);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting Student in BLL: " + ex.Message);
            foreach(var _classSession in classSession)
            {
                _classSession.StudentID.Insert(0,"-");  
            }
            // Save to local if insertion fails
            InsertStudentLocal(classSession);

            throw new Exception("Error inserting Student in BLL. Data saved locally.", ex);
        }
    }

    public  void InsertStudentLocal(List<Student> classSession)
    {
        try
        {
            // Save to local if insertion fails
            var studentResponse = new StudentResponse { data = classSession };
            string studentJson = JsonConvert.SerializeObject(studentResponse);
            _StudentDAL.SaveLocalData(studentJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting Student in BLL: " + ex.Message);

           

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
    public List<Student> LoadNegativeIDStudentes()
    {
        try
        {
            string StudentJson = _StudentDAL.GetStudentsWithNegativeID();
            if (!string.IsNullOrEmpty(StudentJson))
            {
                StudentResponse StudentResponse = JsonConvert.DeserializeObject<StudentResponse>(StudentJson);
                return StudentResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        {

            throw new Exception("Error fetching Class negative from local data", ex);
            return null;
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
    public async Task DeleteStudent(string studentId)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(studentId));
        }

        try
        {
            // Call the DeleteStudentLocal method from the DAL
            await _StudentDAL.DeleteStudentLocal(studentId);
            Console.WriteLine($"Student with ID {studentId} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student in BLL: {ex.Message}");
            throw new Exception($"Error deleting student with ID {studentId} in BLL.", ex);
        }
    }



}
