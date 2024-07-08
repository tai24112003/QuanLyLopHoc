using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ClassBLL
{
    private readonly ClassDAL _ClassDAL;
    private readonly string localFilePath = "localClass.json";

    public ClassBLL(ClassDAL ClassDAL)
    {
        _ClassDAL = ClassDAL ?? throw new ArgumentNullException(nameof(ClassDAL));
    }
    public async Task<Class> InsertClass(Class classSession)
    {
        try
        {
            string responseJson = await _ClassDAL.InsertClass(classSession);
            var insertedSession = JsonConvert.DeserializeObject<Class>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting class session in BLL: " + ex.Message);
            throw new Exception("Error inserting class session in BLL", ex);
        }
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
                // Load Class from local file
                Console.WriteLine("load local");
                return LoadLocalData();
            }
            else
            {
                Console.WriteLine("load api");

                // Get Class from server
                string ClassJson = await _ClassDAL.GetAllClass();
                // Save Class and last update time to local file
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
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.LastTimeUpdateClass;
        }
        return null;
    }

    private void SaveLocalData(string ClassJson, DateTime lastUpdateTime)
    {
        var localData = new LocalDataResponse
        {
            ClassJson = ClassJson,
            LastTimeUpdateClass = lastUpdateTime
        };
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localData));
    }

    private string LoadLocalData()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.ClassJson;
        }
        return null;
    }
}
