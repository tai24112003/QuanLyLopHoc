using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;

public class LocalDataHandler
{
    private readonly ClassSessionBLL _classSessionBLL;
    private readonly SessionComputerBLL _sessionComputerBLL;
    private readonly string localClassSessionsFilePath = "localClassSessions.json";
    private readonly string localSessionComputersFilePath = "localSessionComputers.json";

    public LocalDataHandler(ClassSessionBLL classSessionBLL, SessionComputerBLL sessionComputerBLL)
    {
        _classSessionBLL = classSessionBLL ?? throw new ArgumentNullException(nameof(classSessionBLL));
        _sessionComputerBLL = sessionComputerBLL ?? throw new ArgumentNullException(nameof(sessionComputerBLL));
    }

    public async Task<bool> SaveLocalDataToDatabase()
    {
        try
        {
            // Save local class sessions
            var classSessions = LoadLocalClassSessions();
            foreach (var classSession in classSessions)
            {
                await _classSessionBLL.InsertClassSession(classSession);
            }

            // Save local session computers
            var sessionComputers = LoadLocalSessionComputers();
            foreach (var kvp in sessionComputers)
            {
                await _sessionComputerBLL.InsertSessionComputers(kvp.Key, kvp.Value);
            }

            // Delete local files after successful save
            DeleteLocalFiles();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving local data to database: " + ex.Message);
            return false;
        }
    }

    public void SaveLocalSessionId(int sessionId)
    {
        File.WriteAllText("localSessionId.txt", sessionId.ToString());
    }

    public void SaveLocalClassSession(ClassSession classSession)
    {
        var classSessions = LoadLocalClassSessions();
        classSessions.Add(classSession);
        File.WriteAllText(localClassSessionsFilePath, JsonConvert.SerializeObject(classSessions));
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

    private Dictionary<int, List<SessionComputer>> LoadLocalSessionComputers()
    {
        if (File.Exists(localSessionComputersFilePath))
        {
            var localData = File.ReadAllText(localSessionComputersFilePath);
            return JsonConvert.DeserializeObject<Dictionary<int, List<SessionComputer>>>(localData) ?? new Dictionary<int, List<SessionComputer>>();
        }

        return new Dictionary<int, List<SessionComputer>>();
    }

    private void DeleteLocalFiles()
    {
        if (File.Exists(localClassSessionsFilePath))
        {
            File.Delete(localClassSessionsFilePath);
        }

        if (File.Exists(localSessionComputersFilePath))
        {
            File.Delete(localSessionComputersFilePath);
        }

        if (File.Exists("localSessionId.txt"))
        {
            File.Delete("localSessionId.txt");
        }
    }
}
