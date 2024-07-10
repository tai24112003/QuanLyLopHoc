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
            // Check local file for last update time
            DateTime? localLastUpdateTime = GetLocalLastTimeUpdate();

            // Get last update time from server
            string lastTimeUpdateJson = await _ClassDAL.GetLastTimeUpdateFromDB();
            var lastTimeUpdateResponse = JsonConvert.DeserializeObject<LastTimeUpdateResponse>(lastTimeUpdateJson);
            DateTime serverLastUpdateTime;
            DateTime.TryParseExact(lastTimeUpdateResponse.data[0].lastTimeUpdateClass, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out serverLastUpdateTime);

            if (localLastUpdateTime.HasValue && localLastUpdateTime.Value >= serverLastUpdateTime)
            {
                // Load Class from local database
                Console.WriteLine("load local");
                return LoadLocalData();
            }
            else
            {
                Console.WriteLine("load api");

                // Get Class from server
                string ClassJson = await _ClassDAL.GetAllClass();
                // Save Class and last update time to local database
                SaveLocalData(ClassJson, serverLastUpdateTime);
                return ClassJson;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching Class from BLL", ex);
        }
    }

    private DateTime? GetLocalLastTimeUpdate()
    {
        string query = "SELECT MAX(LastTime) AS LastUpdate FROM classes";
        DataTable dataTable = DataProvider.GetDataTable(query, null);

        if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0]["LastUpdate"] != DBNull.Value)
        {
            return Convert.ToDateTime(dataTable.Rows[0]["LastUpdate"]);
        }

        return null;
    }

    private void SaveLocalData(string ClassJson, DateTime lastUpdateTime)
    {
        var classResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);

        foreach (var classSession in classResponse.data)
        {
            string query = "INSERT INTO `classes` (`ClassID`, `ClassName`, `UserID`, `last_update`) VALUES (@ClassID, @ClassName, @UserID, @LastUpdate)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@ClassID", classSession.ClassID),
                new OleDbParameter("@ClassName", classSession.ClassName),
                new OleDbParameter("@UserID", classSession.UserID),
                new OleDbParameter("@LastUpdate", lastUpdateTime)
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
