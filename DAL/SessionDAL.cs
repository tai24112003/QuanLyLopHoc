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
                string query = "INSERT INTO `sessions` (`ID`, `StartTime`, `EndTime`) VALUES (@ID, @StartTime, @EndTime)";

                OleDbParameter[] parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@ID", session.ID),
                    new OleDbParameter("@StartTime", session.StartTime),
                    new OleDbParameter("@EndTime", session.EndTime)
                };

                bool success = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));
                if (!success)
                {
                    Console.WriteLine("Failed to insert session locally");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving session data locally", ex);
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
