using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;

public class ClassSessionBLL
{
    private readonly ClassSessionDAL _classSessionDAL;
    private readonly string localClassSessionsFilePath = "localClassSessions.json";

    public ClassSessionBLL(ClassSessionDAL classSessionDAL)
    {
        _classSessionDAL = classSessionDAL ?? throw new ArgumentNullException(nameof(classSessionDAL));
    }

    public async Task<ClassSession> InsertClassSession(ClassSession classSession)
    {
        try
        {
            string responseJson = await _classSessionDAL.InsertClassSession(classSession);
            var insertedSession = JsonConvert.DeserializeObject<ClassSession>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting class session in BLL: " + ex.Message);
            SaveLocalClassSession(classSession);
            throw new Exception("Error inserting class session in BLL", ex);
        }
    }

    private void SaveLocalClassSession(ClassSession classSession)
    {
        var localSessions = LoadLocalClassSessions();
        localSessions.Add(classSession);
        File.WriteAllText(localClassSessionsFilePath, JsonConvert.SerializeObject(localSessions));
    }

    private List<ClassSession> LoadLocalClassSessions()
    {
        if (File.Exists(localClassSessionsFilePath))
        {
            var localData = File.ReadAllText(localClassSessionsFilePath);
            return JsonConvert.DeserializeObject<List<ClassSession>>(localData) ?? new List<ClassSession>();
        }

        return new List<ClassSession>();
    }
}
