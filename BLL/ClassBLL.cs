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
            var existingClass = await GetClassByName(classSession.ClassName);
            if (existingClass != null)
            {
                // Class already exists, return the existing class
                return existingClass;
            }

            // Class does not exist, insert new class
            string responseJson = await _ClassDAL.InsertClass(classSession);
            var insertedClass = JsonConvert.DeserializeObject<Class>(responseJson);
            return insertedClass;
        }
        catch (Exception ex)
        {
            Random random = new Random();
            var classId = random.Next() * -1;
            classSession.ClassID = classId;
            var classList = new List<Class> { classSession };
            var classResponse = new ClassResponse { data = classList };
            string classJson = JsonConvert.SerializeObject(classResponse);
            _ClassDAL.SaveLocalData(classJson);

            Console.WriteLine("Error inserting class session in BLL: " + ex.Message);
            return classSession;
            throw new Exception("Error inserting class session in BLL", ex);
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
            string ClassJson = _ClassDAL.LoadLocalData();
            if (!string.IsNullOrEmpty(ClassJson))
            {
                ClassResponse ClassResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);
                return ClassResponse.data;
            }
            throw new Exception("Error fetching Class from API and local data", ex);
        }
    }

    public List<Class> LoadNegativeIDClasses()
    {
        try
        {
            string ClassJson = _ClassDAL.LoadNegativeIDClasses();
            if (!string.IsNullOrEmpty(ClassJson))
            {
                ClassResponse ClassResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);
                return ClassResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        {
            
            throw new Exception("Error fetching Class negative from local data", ex);
            return null;
        }
    }
   

    public async Task<List<Class>> GetClassByUserID(int userID)
    {
        var lstClass = await GetAllClass();
        return lstClass.FindAll(c => c.UserID == userID).ToList();
    }

    public async Task<string> GetClass()
    {
        try
        {

            // Get Class from server
            string ClassJson = await _ClassDAL.GetAllClass();
            // Save Class and last update time to local database
            _ClassDAL.SaveLocalData(ClassJson);
            return ClassJson;

        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching Class from BLL", ex);
        }
    }



}
