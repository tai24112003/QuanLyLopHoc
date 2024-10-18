using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Http;
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
            string responseJson = await _dataService.PostAsync("class_session/", classSessionJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class session in DAL", ex);
        }
    }

    public async Task<string> getClassSessionByClassID(int ID)
    {
        try
        {
            string ClassStudentsJson = await _dataService.GetAsync("class_session/" + ID);
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task UpdateClassSessionWithNewClassID(int oldClassID, int newClassID)
    {
        try
        {
            string query = $"UPDATE Class_Session SET ClassID = @newClassID WHERE ClassID = @oldClassID";
            OleDbParameter[] parameters = new OleDbParameter[]
        {
        new OleDbParameter("@newClassID", newClassID),
        new OleDbParameter("@oldClassID", oldClassID),
        };

            bool result = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));
            if (!result)
            {
                throw new Exception($"No ClassSession found with ID: {oldClassID}, or an error occurred during update.");
            }
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public void SaveLocalData(List<ClassSession> classSessions)
    {
        try
        {
            foreach (var classSession in classSessions)
            {
                string query = "INSERT INTO `Class_Sessions` (`SessionID`, `ClassID`, `RoomID`, `StartTime`, `EndTime`, `user_id`, `Session`) VALUES (@SessionID, @ClassID, @RoomID, @StartTime, @EndTime, @UserID,@Session)";

                OleDbParameter[] parameters = new OleDbParameter[]
                {
                new OleDbParameter("@SessionID", classSession.SessionID),
                new OleDbParameter("@ClassID", classSession.ClassID),
                new OleDbParameter("@RoomID", classSession.RoomID),
                new OleDbParameter("@StartTime", classSession.StartTime),
                new OleDbParameter("@EndTime", classSession.EndTime),
                new OleDbParameter("@UserID", classSession.user_id),
                new OleDbParameter("@Session", classSession.Session),
                };

                DataProvider.RunNonQuery(query, parameters);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving data locally: " + ex.Message);
        }
    }

    public string LoadLocalData()
    {
        try
        {
            string query = "SELECT SessionID, ClassID, RoomID, StartTime, EndTime FROM Class_Sessions";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<ClassSession> Class_Sessions = new List<ClassSession>();
            foreach (DataRow row in dataTable.Rows)
            {
                ClassSession classSession = new ClassSession
                {
                    SessionID = int.Parse(row["SessionID"].ToString()),
                    ClassID = int.Parse(row["ClassID"].ToString()),
                    RoomID = row["RoomID"].ToString(),
                    StartTime = DateTime.Parse(row["StartTime"].ToString()),
                    EndTime = DateTime.Parse(row["EndTime"].ToString()),
                    user_id = int.Parse(row["user_id"].ToString()),
                    // Set other properties as needed
                };
                Class_Sessions.Add(classSession);
            }

            ClassSessionResponse classSessionResponse = new ClassSessionResponse { data = Class_Sessions };
            return JsonConvert.SerializeObject(classSessionResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }
}

