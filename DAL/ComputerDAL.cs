using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ComputerDAL
{
    private readonly IDataService _dataService;

    public ComputerDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetComputerByComputerID(string ComputerID)
    {
        try
        {
            string ComputerJson = await _dataService.GetAsync($"computer/{ComputerID}");
            return ComputerJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> GetComputersByDateRange(DateTime startTime, DateTime endTime)
    {
        try
        {
            // Chuyển đổi ngày thành định dạng chuỗi để gửi qua query string
            string formattedStartTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss");
            string formattedEndTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss");

            // Tạo URL với query string
            string url = $"computer/getComputerBetween?startDate={formattedStartTime}&endDate={formattedEndTime}";

            // Gọi API và lấy dữ liệu dưới dạng JSON
            string computersJson = await _dataService.GetAsync(url);

            return computersJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void SaveLocalData(string ComputersJson)
    {
        var ComputerResponse = JsonConvert.DeserializeObject<ComputerResponse>(ComputersJson);

        if (ComputerResponse == null || ComputerResponse.data == null)
        {
            return;
        }

        foreach (var Computer in ComputerResponse.data)
        {
            string query = "INSERT INTO Computers (RoomID, ComputerName, RAM, HDD, CPU) VALUES (@RoomID, @ComputerName, @RAM, @HHD, @CPU)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@RoomID", Computer.RoomID),
                new OleDbParameter("@ComputerName", Computer.ComputerName),
                new OleDbParameter("@RAM", Computer.RAM),
                new OleDbParameter("@HDD", Computer.HDD),
                new OleDbParameter("@CPU", Computer.CPU),
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    public string LoadLocalData()
    {
        try
        {
            string query = "SELECT * FROM Computers";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Computer> Computers = new List<Computer>();
            foreach (DataRow row in dataTable.Rows)
            {
                Computer Computer = new Computer
                {
                    RoomID = row["RoomID"].ToString(),
                    ComputerName = row["ComputerName"].ToString(),
                    RAM = row["RAM"].ToString(),
                    HDD = row["HDD"].ToString(),
                    CPU = row["CPU"].ToString(),
                    ID = int.Parse(row["ComputerID"].ToString()),
                };
                Computers.Add(Computer);
            }

            ComputerResponse ComputerResponse = new ComputerResponse { Status = "success", data = Computers };
            return JsonConvert.SerializeObject(ComputerResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }
    public async Task<bool> CheckIfComputerExists(int computerId)
    {
        string query = "SELECT COUNT(*) FROM computers WHERE ComputerID = @Id"; // Giả sử tên bảng là 'computers'
        OleDbParameter[] parameters = new OleDbParameter[]
       {
        new OleDbParameter("@Id", computerId)
       };

        int count = (int)await Task.Run(() => DataProvider.RunScalar(query, parameters));
        return count > 0; // Trả về true nếu máy tính tồn tại
    }
    public async Task<bool> UpdateComputer(Computer computer)
    {
        string query = "UPDATE computers SET ComputerName = @ComputerName, RAM = @RAM, HHD = @HHD, CPU = @CPU WHERE ComputerID = @Id";
        OleDbParameter[] parameters = new OleDbParameter[]
      {
         new OleDbParameter( "@Id", computer.ID ),
        new OleDbParameter ("@ComputerName", computer.ComputerName),
         new OleDbParameter("@RAM", computer.RAM),
         new OleDbParameter("@HHD", computer.HDD),
         new OleDbParameter("@CPU", computer.CPU)
    };

        int rowsAffected = (int)await Task.Run(() => DataProvider.RunScalar(query, parameters));
        return rowsAffected > 0; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng
    }

    



}
