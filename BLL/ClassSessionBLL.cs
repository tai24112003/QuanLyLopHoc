using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class ClassSessionBLL
{
    private readonly ClassSessionDAL _classSessionDAL;

    public ClassSessionBLL(ClassSessionDAL classSessionDAL)
    {
        _classSessionDAL = classSessionDAL ?? throw new ArgumentNullException(nameof(classSessionDAL));
    }

    public async Task<ClassSession> InsertClassSession(ClassSession classSession)
    {
        try
        {
            string responseJson = await _classSessionDAL.InsertClassSession(classSession);
            var insertedSession = JsonConvert.DeserializeObject<ClassSession>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting class session in BLL: " + ex.Message);
            Random random = new Random();
            int ssId=random.Next()*-1;
            classSession.SessionID= ssId;
            List<ClassSession> classSessions=new List<ClassSession>();
            classSessions.Add(classSession);
            // Save to local database if insertion fails
            SaveLocalData(classSessions);

            throw new Exception("Error inserting class session in BLL. Data saved locally.", ex);
        }
    }

    private void SaveLocalData(List<ClassSession> classSessions)
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

    private string LoadLocalData()
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
    public async Task<List<ClassSession>> GetClassSessionsByID(int id)
    {
        try
        {
            string ClassSessionsJson = await GetClassSessionsByID1(id);
            ClassSessionResponse ClassSessionResponse = JsonConvert.DeserializeObject<ClassSessionResponse>(ClassSessionsJson);
            return ClassSessionResponse.data;
        }
        catch (Exception ex)
        {
            string ClassSessionsJson = LoadLocalData();
            if (!string.IsNullOrEmpty(ClassSessionsJson))
            {
                ClassSessionResponse ClassSessionResponse = JsonConvert.DeserializeObject<ClassSessionResponse>(ClassSessionsJson);
                return ClassSessionResponse.data.FindAll(e=>e.ClassID==id);
            }
            throw new Exception("Error fetching ClassSessions from API and local data", ex);
        }
    }
    public async Task<string> GetClassSessionsByID1(int id)
    {
        try
        {
            // Get ClassSessions from server
            string ClassSessionsJson = await _classSessionDAL.getClassSessionByClassID(id);
            // Save ClassSessions and last update time to local database
            ClassSessionResponse ClassSessionResponse = JsonConvert.DeserializeObject<ClassSessionResponse>(ClassSessionsJson);
            SaveLocalData(ClassSessionResponse.data);


            return ClassSessionsJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassSessions from BLL", ex);
        }
    }
}
