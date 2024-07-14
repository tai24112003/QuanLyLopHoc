// AttendanceDAL.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class AttendanceDAL
{
    private readonly IDataService _dataService;

    public AttendanceDAL(IDataService dataService)
    {
        _dataService = dataService;
    }
    public async Task<List<ClassSession>> GetAttendanceByClassID(int classID)
    {
        var response = await _dataService.GetAsync($"attendance/{classID}");
        var apiResponse = JsonConvert.DeserializeObject<ClassSessionResponse>(response);

        return apiResponse.status == "success" ? apiResponse.data : new List<ClassSession>();
    }
    public async Task<string> InsertAttendance(List<Attendance> Attendances)
    {
        try
        {
            string AttendancesJson = JsonConvert.SerializeObject(Attendances);
            string responseJson = await _dataService.PostAsync("attendance/", AttendancesJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting session computers in DAL", ex);
        }
    }

    public async Task<string> DeleteAttendanceBySessionID(int sessionID)
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

    public async Task InsertAttendanceLocal(int sessionId, List<Attendance> Attendances)
    {
        int sessionID = sessionId < 0 ? sessionId : sessionId * -1;
        foreach (Attendance Attendance in Attendances)
        {
            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@SessionID", sessionID),
            new OleDbParameter("@AttendanceID", Attendance.AttendanceID),
            new OleDbParameter("@Present", Attendance.Present),
            new OleDbParameter("@StudentID", Attendance.StudentID),
            };

            string query = "INSERT INTO `Attandance` (`AttendanceID`, `StudentID`, `SessionID`, `Present` " +
                           "VALUES (@AttendanceID, @StudentID, @SessionID, @Present";

            bool success = DataProvider.RunNonQuery(query, parameters);
            if (!success)
            {
                throw new Exception("Failed to insert session computer into database.");
            }
        }
    }

    public List<Attendance> LoadAttendancesLocal(int classID)
    {
        List<Attendance> Attendances = new List<Attendance>();
        string query = "SELECT Attendance.AttendanceID, Attendance.StudentID, Attendance.SessionID, Attendance.Present FROM(Attendance INNER JOIN Class_Sessions ON Attendance.SessionID = Class_Sessions.SessionID) WHERE Class_Sessions.ClassID = @ClassID; ";
        OleDbParameter parameter = new OleDbParameter("@ClassID", classID);

        DataTable dataTable = DataProvider.GetDataTable(query, new OleDbParameter[] { parameter });

        if (dataTable != null)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Attendance Attendance = new Attendance
                {
                    StudentID = row["StudentID"].ToString(),
                    SessionID = int.Parse(row["SessionID"].ToString()),
                    Present = row["Present"].ToString()
                };
                Attendances.Add(Attendance);
            }
        }

        return Attendances;
    }
}
