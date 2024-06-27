// SessionComputerDAL.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class SessionComputerDAL
{
    private readonly IDataService _dataService;

    public SessionComputerDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> InsertSessionComputer(List<SessionComputer> sessionComputers)
    {
        try
        {
            string sessionComputersJson = JsonConvert.SerializeObject(sessionComputers);
            string responseJson = await _dataService.PostAsync("session_computer/insert", sessionComputersJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting session computers in DAL", ex);
        }
    }

    public async Task<string> DeleteSessionComputerBySessionID(int sessionID)
    {
        try
        {
            string responseJson = await _dataService.DeleteAsync($"session_computer/deleteBySessionID/{sessionID}");
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting session computers by session ID in DAL", ex);
        }
    }
}
