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
            Console.WriteLine("Error inserting class session in BLL: " + ex.Message);
            throw new Exception("Error inserting class session in BLL", ex);
        }
    }

    private async Task<Class> GetClassByName(string className)
    {
        var lstClass = await GetAllClass();
        return lstClass.FirstOrDefault(c => c.ClassName.Equals(className, StringComparison.OrdinalIgnoreCase));
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
            string ClassJson = LoadLocalData();
            if (!string.IsNullOrEmpty(ClassJson))
            {
                ClassResponse ClassResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);
                return ClassResponse.data;
            }
            throw new Exception("Error fetching Class from API and local data", ex);
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
                SaveLocalData(ClassJson);
                return ClassJson;
            
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching Class from BLL", ex);
        }
    }


    private void SaveLocalData(string ClassJson)
    {
        var classResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);

        foreach (var classSession in classResponse.data)
        {
            string query = "INSERT INTO `classes` (`ClassID`, `ClassName`, `UserID`, `LastTime`) VALUES (@ClassID, @ClassName, @UserID)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@ClassID", classSession.ClassID),
                new OleDbParameter("@ClassName", classSession.ClassName),
                new OleDbParameter("@UserID", classSession.UserID),
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    private string LoadLocalData()
    {
        try
        {
            string query = "SELECT ClassID, ClassName, UserID FROM classes";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Class> classes = new List<Class>();
            foreach (DataRow row in dataTable.Rows)
            {
                Class classSession = new Class
                {
                    ClassID = int.Parse(row["ClassID"].ToString()),
                    ClassName = row["ClassName"].ToString(),
                    UserID = int.Parse(row["UserID"].ToString())
                };
                classes.Add(classSession);
            }

            ClassResponse classResponse = new ClassResponse { data = classes };
            return JsonConvert.SerializeObject(classResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }
}
