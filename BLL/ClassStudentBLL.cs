using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Newtonsoft.Json;

public class ClassStudentBLL
{
    private readonly ClassStudentDAL _ClassStudentDAL;
    private readonly StudentDAL _StudentDAL;

    public ClassStudentBLL(ClassStudentDAL ClassStudentDAL, StudentDAL StudentDAL)
    {
        _ClassStudentDAL = ClassStudentDAL ?? throw new ArgumentNullException(nameof(ClassStudentDAL));
        _StudentDAL = StudentDAL ?? throw new ArgumentNullException(nameof(StudentDAL));
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

            // Save to local if insertion fails
            var classResponse = new ClassStudentResponse { data = classSession };
            string classJson = JsonConvert.SerializeObject(classResponse);
            _ClassStudentDAL.SaveLocalData(classJson);

            throw new Exception("Error inserting ClassStudent in BLL. Data saved locally.", ex);
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
            string ClassStudentsJson = _ClassStudentDAL.LoadLocalData();
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
            // Get ClassStudents from server
            string ClassStudentsJson = await _ClassStudentDAL.GetAllClassStudents();
            // Save ClassStudents and last update time to local database
            _ClassStudentDAL.SaveLocalData(ClassStudentsJson);
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassStudents from BLL", ex);
        }
    }


    

    public async Task<List<ClassStudent>> GetClassStudentsByID(int id)
    {
        try
        {
            string ClassStudentsJson = await GetClassStudentsByID1(id);
            ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
            return ClassStudentResponse.data;
        }
        catch (Exception ex)
        {
            string ClassStudentsJson = _ClassStudentDAL.LoadLocalData();
            if (!string.IsNullOrEmpty(ClassStudentsJson))
            {
                ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
                return ClassStudentResponse.data;
            }
            throw new Exception("Error fetching ClassStudents from API and local data", ex);
        }
    }
    public async Task<string> GetClassStudentsByID1(int id)
    {
        try
        {
            // Get ClassStudents from server
            string ClassStudentsJson = await _ClassStudentDAL.GetClassStudentsByID(id);
            // Save ClassStudents and last update time to local database
            _ClassStudentDAL.SaveLocalData(ClassStudentsJson);

            
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassStudents from BLL", ex);
        }
    }
    public async Task DeleteClassStudentsByClassID(int ClassID)
    {
        if (string.IsNullOrEmpty(ClassID.ToString()))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(ClassID));
        }

        try
        {
            // Call the DeleteStudentLocal method from the DAL
            await _ClassStudentDAL.DeleteClassStudentLocal(ClassID);
            Console.WriteLine($"Student with ID {ClassID} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student in BLL: {ex.Message}");
            throw new Exception($"Error deleting student with ID {ClassID} in BLL.", ex);
        }
    }

    public async Task DeleteClassStudentsByStudentID(string StudentID)
    {
        if (string.IsNullOrEmpty(StudentID))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(StudentID));
        }

        try
        {
            // Call the DeleteStudentLocal method from the DAL
            await _ClassStudentDAL.DeleteClassStudentLocalByStudentID(StudentID);
            Console.WriteLine($"Student with ID {StudentID} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student in BLL: {ex.Message}");
            throw new Exception($"Error deleting student with ID {StudentID} in BLL.", ex);
        }
    }

}
