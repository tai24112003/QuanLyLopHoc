// SessionComputerDAL.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
            string responseJson = await _dataService.PostAsync("session_computer/", sessionComputersJson);
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

    public async Task InsertSessionComputerLocal(int sessionId, List<SessionComputer> sessionComputers)
    {
        int sessionID = sessionId < 0 ? sessionId : sessionId * -1;  
        foreach (SessionComputer sessionComputer in sessionComputers)
        {
            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@SessionID", sessionID),
            new OleDbParameter("@ComputerName", sessionComputer.ComputerName),
            new OleDbParameter("@RAM", sessionComputer.RAM),
            new OleDbParameter("@HHD", sessionComputer.HDD),
            new OleDbParameter("@CPU", sessionComputer.CPU),
            new OleDbParameter("@MouseConnected", sessionComputer.MouseConnected),
            new OleDbParameter("@KeyboardConnected", sessionComputer.KeyboardConnected),
            new OleDbParameter("@MonitorConnected", sessionComputer.MonitorConnected),
            new OleDbParameter("@MismatchInfo", sessionComputer.MismatchInfo),
            new OleDbParameter("@RepairNote", sessionComputer.RepairNote),
            new OleDbParameter("@StudentID", sessionComputer.StudentID),
            };

            string query = "INSERT INTO `Session_Computer` (`SessionID`, `ComputerID`, `RAM`, `HDD`, `CPU`, " +
                           "`MouseConnected`, `KeyboardConnected`, `MonitorConnected`, `MismatchInfo`, `RepairNote`, `StudentID`) " +
                           "VALUES (@SessionID, @ComputerID, @RAM, @HHD, @CPU, " +
                           "@MouseConnected, @KeyboardConnected, @MonitorConnected, @MismatchInfo, @RepairNote, @StudentID)";

            bool success = DataProvider.RunNonQuery(query, parameters);
            if (!success)
            {
                throw new Exception("Failed to insert session computer into database.");
            }
        }
    }

    public List<SessionComputer> LoadSessionComputersLocal(int sessionId)
    {
        List<SessionComputer> sessionComputers = new List<SessionComputer>();
        string query = "SELECT * FROM `Session_Computer` WHERE `SessionID` = @SessionID";
        OleDbParameter parameter = new OleDbParameter("@SessionID", sessionId);

        DataTable dataTable = DataProvider.GetDataTable(query, new OleDbParameter[] { parameter });

        if (dataTable != null)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                SessionComputer sessionComputer = new SessionComputer
                {
                    SessionID = (int)row["SessionID"],
                    ComputerName = row["ComputerName"].ToString(),
                    RAM = row["RAM"].ToString(),
                    HDD = row["HHD"].ToString(),
                    CPU = row["CPU"].ToString(),
                    MouseConnected = Convert.ToBoolean(row["MouseConnected"]),
                    KeyboardConnected = Convert.ToBoolean(row["KeyboardConnected"]),
                    MonitorConnected = Convert.ToBoolean(row["MonitorConnected"]),
                    MismatchInfo = row["MismatchInfo"].ToString(),
                    RepairNote = row["RepairNote"].ToString(),
                    StudentID = row["StudentID"].ToString()
                };
                sessionComputers.Add(sessionComputer);
            }
        }

        return sessionComputers;
    }
}
