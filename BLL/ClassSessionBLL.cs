// ClassSessionBLL.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ClassSessionBLL
{
    private readonly ClassSessionDAL _classSessionDAL;
    private readonly string localFilePath = "localClassSessions.json";

    public ClassSessionBLL(ClassSessionDAL classSessionDAL)
    {
        _classSessionDAL = classSessionDAL ?? throw new ArgumentNullException(nameof(classSessionDAL));
    }

    public async Task<string> InsertClassSession(ClassSession classSession)
    {
        try
        {
            // Insert class session using DAL
            string responseJson = await _classSessionDAL.InsertClassSession(classSession);

            // Save class session to local file
            SaveLocalClassSession(classSession);

            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class session in BLL", ex);
        }
    }

    private void SaveLocalClassSession(ClassSession classSession)
    {
        // Load existing sessions
        var sessions = LoadLocalClassSessions();

        // Add new session
        sessions.Add(classSession);

        // Save sessions to local file
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(sessions));
    }

    private List<ClassSession> LoadLocalClassSessions()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            return JsonConvert.DeserializeObject<List<ClassSession>>(localData) ?? new List<ClassSession>();
        }

        return new List<ClassSession>();
    }
}
