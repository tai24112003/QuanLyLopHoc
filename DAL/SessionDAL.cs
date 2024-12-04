using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SessionDAL
{
    private readonly IDataService _dataService;

    public SessionDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetSession()
    {
        try
        {
            string lastTimeUpdateJson = await _dataService.GetAsync("session/");
            return lastTimeUpdateJson;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Get Session API endpoint not found.", ex);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching session from API.", ex);
            return null;
        }
    }

    public async Task SaveLocal(string sessionJson)
    {
        try
        {
            var sessionResponse = JsonConvert.DeserializeObject<SessionResponse>(sessionJson);

            foreach (var session in sessionResponse.data)
            {
                // Step 1: Check if the session already exists
                string checkQuery = "SELECT COUNT(*) FROM `sessions` WHERE `ID` = @ID";
                OleDbParameter[] checkParameters = new OleDbParameter[]
                {
                new OleDbParameter("@ID", session.ID)
                };

                int existingSessionCount = await Task.Run(() =>
                    Convert.ToInt32(DataProvider.RunScalar(checkQuery, checkParameters)));

                // Step 2: Delete the existing session if it exists
                if (existingSessionCount > 0)
                {
                    string deleteQuery = "DELETE FROM `sessions` WHERE `ID` = @ID";
                    bool deleteSuccess = await Task.Run(() =>
                        DataProvider.RunNonQuery(deleteQuery, checkParameters));

                    if (!deleteSuccess)
                    {
                        Console.WriteLine($"Failed to delete existing session with ID {session.ID}");
                        continue;  // Skip to the next session if deletion fails
                    }
                }

                // Step 3: Insert the new session
                string insertQuery = "INSERT INTO `sessions` (`ID`, `StartTime`, `EndTime`) VALUES (@ID, @StartTime, @EndTime)";
                OleDbParameter[] insertParameters = new OleDbParameter[]
                {
                new OleDbParameter("@ID", session.ID),
                new OleDbParameter("@StartTime", session.StartTime),
                new OleDbParameter("@EndTime", session.EndTime)
                };

                bool insertSuccess = await Task.Run(() =>
                    DataProvider.RunNonQuery(insertQuery, insertParameters));

                if (!insertSuccess)
                {
                    Console.WriteLine($"Failed to insert session with ID {session.ID} locally");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving session data locally: " + ex.Message);
        }
    }


    public async Task<string> LoadLocal()
    {
        try
        {
             string query = "SELECT * FROM sessions ";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Session> sessions = new List<Session>();
            foreach (DataRow row in dataTable.Rows)
            {
                Session session = new Session
                {
                    ID = (row["ID"].ToString()),
                    StartTime = (row["StartTime"].ToString()),
                    EndTime = (row["EndTime"].ToString())
                };
                sessions.Add(session);
            }

            SessionResponse sessionResponse = new SessionResponse { data = sessions };
            return JsonConvert.SerializeObject(sessionResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading local session data", ex);
            return null;
        }
    }
}
