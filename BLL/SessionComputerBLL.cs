using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class SessionComputerBLL
{
    private readonly SessionComputerDAL _sessionComputerDAL;
    private readonly string localSessionComputersFilePath = "localSessionComputers.json";

    public SessionComputerBLL(SessionComputerDAL sessionComputerDAL)
    {
        _sessionComputerDAL = sessionComputerDAL ?? throw new ArgumentNullException(nameof(sessionComputerDAL));
    }

    public async Task InsertSessionComputers(int sessionId, List<SessionComputer> sessionComputers)
    {
        try
        {
            await _sessionComputerDAL.InsertSessionComputer(sessionComputers);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting session computer in BLL: " + ex.Message);
            SaveLocalSessionComputers(sessionId,sessionComputers);
            Console.WriteLine("Error inserting session computer in BLL: " + ex);

            throw new Exception("Error inserting session computer in BLL", ex);
        }
    }

    public void SaveLocalSessionComputers(int sessionID,List<SessionComputer> sessionComputers)
    {
        var localSessionComputers = LoadLocalSessionComputers();
        int sessionId = sessionID; // Assuming all computers have the same SessionID

        if (!localSessionComputers.ContainsKey(sessionId))
        {
            localSessionComputers[sessionId] = new List<SessionComputer>();
        }
        localSessionComputers[sessionId].AddRange(sessionComputers);
        File.WriteAllText(localSessionComputersFilePath, JsonConvert.SerializeObject(localSessionComputers));
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
}
