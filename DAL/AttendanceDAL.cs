using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Threading.Tasks;
using System;

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
        
            string AttendancesJson = JsonConvert.SerializeObject(Attendances);
            string responseJson = await _dataService.PostAsync("attendance/", AttendancesJson);
            return responseJson;
        
    }

    public async Task<string> DeleteAttendanceBySessionID(int sessionID)
    {
        try
        {
            string responseJson = await _dataService.DeleteAsync($"attendance/deleteBySessionID/{sessionID}");
            return responseJson;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting attendance by session ID in DAL", ex);
            return null;
        }
    }

    public async Task InsertAttendanceLocal(int sessionId, List<Attendance> Attendances)
    {
        // Đảm bảo sessionID là số âm (nếu cần)
        int sessionID = sessionId < 0 ? sessionId : sessionId * -1;

        // Xóa các bản ghi có SessionID trùng với sessionID
        string deleteQuery = "DELETE FROM Attendance WHERE SessionID = @SessionID";
        OleDbParameter[] deleteParameters = new OleDbParameter[]
        {
        new OleDbParameter("@SessionID", sessionID)
        };

        bool deleteSuccess = await Task.Run(() => DataProvider.RunNonQuery(deleteQuery, deleteParameters));
        if (!deleteSuccess)
        {
            Console.WriteLine("Failed to delete previous attendance records.");
            return; // Dừng nếu không xóa được các bản ghi cũ
        }

        // Chèn các bản ghi mới
        foreach (Attendance attendance in Attendances)
        {
            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@StudentID", attendance.StudentID), // Tham số StudentID phải đặt đầu tiên
            new OleDbParameter("@SessionID", sessionID),
            new OleDbParameter("@Present", attendance.Present),
            };

            string insertQuery = "INSERT INTO Attendance (StudentID, SessionID, Present) " +
                                 "VALUES (@StudentID, @SessionID, @Present)";

            bool insertSuccess = await Task.Run(() => DataProvider.RunNonQuery(insertQuery, parameters));
            if (!insertSuccess)
            {
                Console.WriteLine("Failed to insert attendance into database.");
            }
        }
    }


    public async Task<List<Attendance>> LoadAttendancesLocal(int classID)
    {
        List<Attendance> Attendances = new List<Attendance>();
        string query = "SELECT Attendance.AttendanceID, Attendance.StudentID, Attendance.SessionID, Attendance.Present " +
                       "FROM Attendance INNER JOIN Class_Sessions ON Attendance.SessionID = Class_Sessions.SessionID " +
                       "WHERE Class_Sessions.ClassID = @ClassID;";
        OleDbParameter parameter = new OleDbParameter("@ClassID", classID);

        DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, new OleDbParameter[] { parameter }));

        if (dataTable != null)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Attendance attendance = new Attendance
                {
                    AttendanceID = int.Parse(row["AttendanceID"].ToString()),
                    StudentID = row["StudentID"].ToString(),
                    SessionID = int.Parse(row["SessionID"].ToString()),
                    Present = row["Present"].ToString()
                };
                Attendances.Add(attendance);
            }
        }

        return Attendances;
    }

    public async Task<bool> DeleteAttendanceLocal(int sessionID)
    {
        try
        {
            string query = "DELETE FROM Attendance WHERE SessionID = @SessionID";
            OleDbParameter parameter = new OleDbParameter("@SessionID", sessionID);

            return await Task.Run(() => DataProvider.RunNonQuery(query, new OleDbParameter[] { parameter }));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting attendance locally: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateAttendanceLocal(Attendance attendance)
    {
        try
        {
            string query = "UPDATE Attendance SET Present = @Present WHERE AttendanceID = @AttendanceID";
            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@Present", attendance.Present),
                new OleDbParameter("@AttendanceID", attendance.AttendanceID)
            };

            return await Task.Run(() => DataProvider.RunNonQuery(query, parameters));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating attendance locally: " + ex.Message);
            return false;
        }
    }

    public async Task<List<Attendance>> GetAttendanceBySessionID(int sessionID)
    {
        List<Attendance> attendances = new List<Attendance>();
        string query = "SELECT AttendanceID, StudentID, SessionID, Present FROM Attendance WHERE SessionID = @SessionID;";
        OleDbParameter parameter = new OleDbParameter("@SessionID", sessionID);

        DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, new OleDbParameter[] { parameter }));

        if (dataTable != null)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Attendance attendance = new Attendance
                {
                    AttendanceID = int.Parse(row["AttendanceID"].ToString()),
                    StudentID = row["StudentID"].ToString(),
                    SessionID = int.Parse(row["SessionID"].ToString()),
                    Present = row["Present"].ToString()
                };
                attendances.Add(attendance);
            }
        }

        return attendances;
    }

    public async Task<List<Attendance>> GetAttendanceBySessionIDNegative()
    {
        List<Attendance> attendances = new List<Attendance>();
        string query = "SELECT AttendanceID, StudentID, SessionID, Present FROM Attendance WHERE SessionID  < 0;";

        DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

        if (dataTable != null)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Attendance attendance = new Attendance
                {
                    AttendanceID = int.Parse(row["AttendanceID"].ToString()),
                    StudentID = row["StudentID"].ToString(),
                    SessionID = int.Parse(row["SessionID"].ToString()),
                    Present = row["Present"].ToString()
                };
                attendances.Add(attendance);
            }
        }

        return attendances;
    }

}
