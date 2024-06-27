// SessionComputerBLL.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class SessionComputerBLL
{
    private readonly SessionComputerDAL _sessionComputerDAL;
    private readonly string localFilePath = "localSessionComputers.json";

    public SessionComputerBLL(SessionComputerDAL sessionComputerDAL)
    {
        _sessionComputerDAL = sessionComputerDAL ?? throw new ArgumentNullException(nameof(sessionComputerDAL));
    }

    public async Task<string> InsertSessionComputers(int sessionID, List<SessionComputer> sessionComputers)
    {
        try
        {
            // Delete existing session computers by session ID
            await _sessionComputerDAL.DeleteSessionComputerBySessionID(sessionID);

            // Insert new session computers using DAL
            string responseJson = await _sessionComputerDAL.InsertSessionComputer(sessionComputers);

            // Save session computers to local file
            SaveLocalSessionComputers(sessionID, sessionComputers);

            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting session computers in BLL", ex);
        }
    }

    private void SaveLocalSessionComputers(int sessionID, List<SessionComputer> sessionComputers)
    {
        // Load existing session computers
        var sessions = LoadLocalSessionComputers();

        // Add new session computers
        sessions[sessionID] = sessionComputers;

        // Save sessions to local file
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(sessions));
    }

    private Dictionary<int, List<SessionComputer>> LoadLocalSessionComputers()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            return JsonConvert.DeserializeObject<Dictionary<int, List<SessionComputer>>>(localData) ?? new Dictionary<int, List<SessionComputer>>();
        }

        return new Dictionary<int, List<SessionComputer>>();
    }
}
