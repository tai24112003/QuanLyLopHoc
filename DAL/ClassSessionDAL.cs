using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class ClassSessionDAL
{
    private readonly IDataService _dataService;

    public ClassSessionDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> InsertClassSession(ClassSession classSession)
    {
        try
        {
            string classSessionJson = JsonConvert.SerializeObject(classSession);
            return await _dataService.PostAsync("class_session/", classSessionJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting class session in DAL", ex);
            return null;
        }
    }

    public async Task<string> GetClassSessionByClassID(int ID)
    {
        try
        {
            return await _dataService.GetAsync("class_session/" + ID);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching class session by ClassID", ex);
            return null;
        }
    }

    public async Task UpdateClassSessionWithNewClassID(int oldClassID, int newClassID)
    {
        try
        {
            string query = "UPDATE Class_Sessions SET ClassID = @newClassID WHERE ClassID = @oldClassID";
            OleDbParameter[] parameters =
            {
                new OleDbParameter("@newClassID", newClassID),
                new OleDbParameter("@oldClassID", oldClassID)
            };

            bool result = await Task.Run(() => DataProvider.RunNonQuery(query, parameters) );
            if (!result)
            {
                Console.WriteLine($"No ClassSession found with ID: {oldClassID}, or an error occurred during update.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating ClassSession with new ClassID", ex);
        }
    }

    public async Task SaveLocalDataAsync(List<ClassSession> classSessions)
    {
        try
        {
            foreach (var classSession in classSessions)
            {
                string query = "INSERT INTO `Class_Sessions` (`SessionID`, `ClassID`, `RoomID`, `StartTime`, `EndTime`, `UserID`, `Session`) VALUES (@SessionID, @ClassID, @RoomID, @StartTime, @EndTime, @UserID, @Session)";

                OleDbParameter[] parameters =
                {
                    new OleDbParameter("@SessionID", classSession.SessionID),
                    new OleDbParameter("@ClassID", classSession.ClassID),
                    new OleDbParameter("@RoomID", classSession.RoomID),
                    new OleDbParameter("@StartTime", classSession.StartTime),
                    new OleDbParameter("@EndTime", classSession.EndTime),
                    new OleDbParameter("@UserID", classSession.user_id),
                    new OleDbParameter("@Session", classSession.Session)
                };

                bool success = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));
                if (!success)
                {
                    Console.WriteLine("Failed to save class session locally.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving local class session data", ex);
        }
    }

    public async Task<string> LoadLocalDataAsync()
    {
        try
        {
            string query = "SELECT SessionID, ClassID, RoomID, StartTime, EndTime, user_id FROM Class_Sessions";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<ClassSession> classSessions = new List<ClassSession>();
            foreach (DataRow row in dataTable.Rows)
            {
                ClassSession classSession = new ClassSession
                {
                    SessionID = int.Parse(row["SessionID"].ToString()),
                    ClassID = int.Parse(row["ClassID"].ToString()),
                    RoomID = row["RoomID"].ToString(),
                    StartTime = (row["StartTime"].ToString()),
                    EndTime = (row["EndTime"].ToString()),
                    user_id = int.Parse(row["user_id"].ToString())
                };
                classSessions.Add(classSession);
            }

            ClassSessionResponse response = new ClassSessionResponse { data = classSessions };
            return JsonConvert.SerializeObject(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading local data", ex);
            return null;
        }
    }

    public async Task<List<ClassSession>> LoadNegativeIDClassSessionsAsync()
    {
        try
        {
            string query = "SELECT *  FROM Class_Sessions WHERE SessionID < 0";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<ClassSession> negativeClassSessions = new List<ClassSession>();
            foreach (DataRow row in dataTable.Rows)
            {
                ClassSession classSession = new ClassSession
                {
                    SessionID = int.Parse(row["SessionID"].ToString()),
                    ClassID = int.Parse(row["ClassID"].ToString()),
                    RoomID = row["RoomID"].ToString(),
                    StartTime = (row["StartTime"].ToString()),
                    EndTime = (row["EndTime"].ToString()),
                    user_id = int.Parse(row["UserID"].ToString()),
                    Session = int.Parse(row["Session"].ToString())
                };
                negativeClassSessions.Add(classSession);
            }

            return negativeClassSessions;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading class sessions with negative IDs", ex);
            return null;
        }
    }
}
