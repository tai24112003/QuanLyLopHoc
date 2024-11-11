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
            string sessionComputersJson = JsonConvert.SerializeObject(sessionComputers);
            string responseJson = await _dataService.PostAsync("session_computer/", sessionComputersJson);
            return responseJson;
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
            Console.WriteLine("Error deleting session computers by session ID in DAL", ex);
            return null;
        }
    }

    public async Task InsertSessionComputerLocal(int sessionId, List<SessionComputer> sessionComputers)
    {
        // Đảm bảo sessionID là số âm (nếu cần)
        int sessionID = sessionId < 0 ? sessionId : sessionId * -1;

        // Xóa các bản ghi có SessionID trùng với sessionID
        string deleteQuery = "DELETE FROM Session_Computer WHERE SessionID = @SessionID";
        OleDbParameter[] deleteParameters = new OleDbParameter[]
        {
        new OleDbParameter("@SessionID", sessionID)
        };

        bool deleteSuccess = await Task.Run(() => DataProvider.RunNonQuery(deleteQuery, deleteParameters));
        if (!deleteSuccess)
        {
            Console.WriteLine("Failed to delete previous session computer records.");
            return; // Dừng nếu không xóa được các bản ghi cũ
        }

        // Chèn các bản ghi mới
        foreach (SessionComputer sessionComputer in sessionComputers)
        {
            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@SessionID", sessionID),
            new OleDbParameter("@ComputerID", sessionComputer.ComputerID),
            new OleDbParameter("@RAM", sessionComputer.RAM),
            new OleDbParameter("@HDD", sessionComputer.HDD),
            new OleDbParameter("@CPU", sessionComputer.CPU),
            new OleDbParameter("@MouseConnected", sessionComputer.MouseConnected),
            new OleDbParameter("@KeyboardConnected", sessionComputer.KeyboardConnected),
            new OleDbParameter("@MonitorConnected", sessionComputer.MonitorConnected),
            new OleDbParameter("@MismatchInfo", sessionComputer.MismatchInfo),
            new OleDbParameter("@StudentID", sessionComputer.StudentID),
            };

            string insertQuery = "INSERT INTO `Session_Computer` (`SessionID`, `ComputerID`, `RAM`, `HDD`, `CPU`, " +
                                 "`MouseConnected`, `KeyboardConnected`, `MonitorConnected`, `MismatchInfo`, `StudentID`) " +
                                 "VALUES (@SessionID, @ComputerID, @RAM, @HDD, @CPU, " +
                                 "@MouseConnected, @KeyboardConnected, @MonitorConnected, @MismatchInfo, @StudentID)";

            bool insertSuccess = await Task.Run(() => DataProvider.RunNonQuery(insertQuery, parameters));
            if (!insertSuccess)
            {
                Console.WriteLine("Failed to insert session computer into database.");
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
                    StudentID = row["StudentID"].ToString()
                };
                sessionComputers.Add(sessionComputer);
            }
        }

        return sessionComputers;
    }

    public async Task<List<SessionComputer>> GetSessionComputersBySessionID(int sessionID)
    {
        try
        {
            // Tạo câu truy vấn để lấy thông tin máy tính theo SessionID
            string query = "SELECT * FROM Session_Computer WHERE SessionID = @SessionID";
            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@SessionID", sessionID)
            };

            // Thực thi câu truy vấn và lấy dữ liệu
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, parameters));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null; // Không có dữ liệu
            }

            // Chuyển đổi DataTable thành danh sách thông tin máy tính
            List<SessionComputer> computers = new List<SessionComputer>();
            foreach (DataRow row in dataTable.Rows)
            {
                SessionComputer sessionComputer = new SessionComputer
                {
                    SessionID = (int)row["SessionID"],
                    ComputerID = int.Parse(row["ComputerID"].ToString()),
                    RAM = row["RAM"].ToString(),
                    HDD = row["HDD"].ToString(),
                    CPU = row["CPU"].ToString(),
                    MouseConnected = Convert.ToBoolean(row["MouseConnected"]),
                    KeyboardConnected = Convert.ToBoolean(row["KeyboardConnected"]),
                    MonitorConnected = Convert.ToBoolean(row["MonitorConnected"]),
                    MismatchInfo = row["MismatchInfo"].ToString(),
                    StudentID = row["StudentID"].ToString()
                };
                computers.Add(sessionComputer);
            }

            // Chuyển đổi danh sách máy tính thành JSON
            return computers;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching session computers: " + ex.Message);
            return null;
        }
    }

    public async Task<bool> DeleteSessionComputersBySessionID(int sessionID)
    {
        try
        {
            // Tạo câu truy vấn để xóa máy tính theo SessionID
            string query = "DELETE FROM Session_Computer WHERE SessionID = @SessionID";
            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@SessionID", sessionID)
            };

            // Thực thi câu truy vấn và trả về số dòng bị ảnh hưởng
            bool rowsAffected = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));
            return rowsAffected; // Trả về true nếu có ít nhất một dòng bị xóa
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting session computers: " + ex.Message);
            return false;
        }
    }

    public async Task<List<SessionComputer>> GetSessionComputersBySessionIDNegative()
    {
        try
        {
            // Tạo câu truy vấn để lấy thông tin máy tính theo SessionID
            string query = "SELECT * FROM Session_Computer WHERE SessionID <0";
           

            // Thực thi câu truy vấn và lấy dữ liệu
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null; // Không có dữ liệu
            }

            // Chuyển đổi DataTable thành danh sách thông tin máy tính
            List<SessionComputer> computers = new List<SessionComputer>();
            foreach (DataRow row in dataTable.Rows)
            {
                SessionComputer sessionComputer = new SessionComputer
                {
                    SessionID = (int)row["SessionID"],
                    ComputerID = int.Parse(row["ComputerID"].ToString()),
                    RAM = row["RAM"].ToString(),
                    HDD = row["HDD"].ToString(),
                    CPU = row["CPU"].ToString(),
                    MouseConnected = Convert.ToBoolean(row["MouseConnected"]),
                    KeyboardConnected = Convert.ToBoolean(row["KeyboardConnected"]),
                    MonitorConnected = Convert.ToBoolean(row["MonitorConnected"]),
                    MismatchInfo = row["MismatchInfo"].ToString(),
                    StudentID = row["StudentID"].ToString()
                };
                computers.Add(sessionComputer);
            }

            // Chuyển đổi danh sách máy tính thành JSON
            return computers;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching session computers: " + ex.Message);
            return null;
        }
    }
}
