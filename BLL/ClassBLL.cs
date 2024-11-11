using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ClassBLL
{
    private readonly ClassDAL _ClassDAL;

    public ClassBLL(ClassDAL ClassDAL)
    {
        _ClassDAL = ClassDAL ?? throw new ArgumentNullException(nameof(ClassDAL));
    }

    public async Task<Class> InsertClass(Class classSession)
    {
        try
        {
            //var existingClass = await GetClassByName(classSession.ClassName);
            //if (existingClass != null)
            //{
            //    // Class already exists, return the existing class
            //    return existingClass;
            //}

            // Class does not exist, insert new class
            string responseJson = await _ClassDAL.InsertClass(classSession);
            var insertedClass = JsonConvert.DeserializeObject<Class>(responseJson);
            classSession.ClassID = insertedClass.ClassID;
            var classList = new List<Class> { classSession };
            var classResponse = new ClassResponse { data = classList };
            string classJson = JsonConvert.SerializeObject(classResponse);
            await _ClassDAL.SaveLocalData(classJson);
            return classSession;
        }
        catch (Exception ex)
        {
            Random random = new Random();
            
            var classId = random.Next() * -1;
            classSession.ClassID = classId;
            var classList = new List<Class> { classSession };
            var classResponse = new ClassResponse { data = classList };
            string classJson = JsonConvert.SerializeObject(classResponse);
            await _ClassDAL.SaveLocalData(classJson);

            Console.WriteLine("Error inserting class session in BLL, Save local Success:  " + ex.Message);
            return classSession;
        }
    }

    private async Task<Class> GetClassByName(string className)
    {
        var lstClass = await GetAllClass();
        return lstClass.FirstOrDefault(c => c.ClassName.ToLower() == className.ToLower());
    }

    public async Task<List<Class>> GetAllClass()
    {
        try
        {
            string ClassJson = await GetClass();
            ClassResponse ClassResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);
            return ClassResponse.data;
        }
        catch (Exception ex)
        {
            string ClassJson = await _ClassDAL.LoadLocalData();
            if (!string.IsNullOrEmpty(ClassJson))
            {
                ClassResponse ClassResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);
                return ClassResponse.data;
            }
            Console.WriteLine("Error fetching Class from API and local data", ex);
            return null;
        }
    }

    public async Task<List<Class>> LoadNegativeIDClasses()
    {
        try
        {
            List<Class> ClassJson = await _ClassDAL.LoadNegativeIDClasses();
            if (ClassJson != null)
                return ClassJson;
            return null;
        }
        catch (Exception ex)
        {

            Console.WriteLine("Error fetching Class negative from local data", ex);
            return null;
        }
    }


    public async Task<List<Class>> GetClassByUserID(int userID)
    {
        var lstClass = await GetAllClass();
        if(lstClass != null)
        return lstClass.FindAll(c => c.UserID == userID).ToList();
        return null;
    }

    public async Task<string> GetClass()
    {
        try
        {

            // Get Class from server
            string ClassJson = await _ClassDAL.GetAllClass();
            // Save Class and last update time to local database
            await _ClassDAL.SaveLocalData(ClassJson);
            return ClassJson;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Class from BLL", ex);
            return null;
        }
    }
    public async Task<List<Class>> GetClassByDateRange(DateTime startTime, DateTime endTime)
    {
        try
        {
            string ClasssJson = await _ClassDAL.GetClasssByDateRange(startTime, endTime);
            ClassResponse ClassResponse = JsonConvert.DeserializeObject<ClassResponse>(ClasssJson);
            return ClassResponse.data;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Classs by date range in BLL: " + ex.Message);
            Console.WriteLine("Error fetching Classs by date range in BLL.", ex);
            return null;
        }
    }

    public async Task DeleteClasssByClassID(int ClassID)
    {
        if (string.IsNullOrEmpty(ClassID.ToString()))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(ClassID));
        }

        try
        {
            // Call the DeleteStudentLocal method from the DAL
            await _ClassDAL.DeleteClassLocalByClassID(ClassID);
            Console.WriteLine($"Student with ID {ClassID} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student in BLL: {ex.Message}");
            Console.WriteLine($"Error deleting student with ID {ClassID} in BLL.", ex);
        }
    }
}
