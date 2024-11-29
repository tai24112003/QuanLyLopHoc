using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class RoomDAL
{
    private readonly IDataService _dataService;

    public RoomDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetRoomByRoomName(string roomID)
    {
        try
        {
            string roomJson = await _dataService.GetAsync($"room/{roomID}");
            return roomJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task UpdateRoom(string roomID,string roomjson)
    {
        try
        {
            string roomJson = await _dataService.PutAsync($"room/{roomID}",roomjson);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void SaveLocalData(string RoomsJson)
    {
        var roomResponse = JsonConvert.DeserializeObject<RoomResponse>(RoomsJson);

        foreach (var room in roomResponse.data)
        {
            string query = "INSERT INTO `Rooms` (`RoomID`, `NumberOfComputers`, `StandardRAM`, `StandardHDD`, `StandardCPU`, `Status`) VALUES (@RoomID, @NumberOfComputers, @StandardRAM, @StandardHDD, @StandardCPU, @Status)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@RoomID", room.RoomID),
                new OleDbParameter("@NumberOfComputers", room.NumberOfComputers),
                new OleDbParameter("@StandardRAM", room.StandardRAM),
                new OleDbParameter("@StandardHDD", room.StandardHDD),
                new OleDbParameter("@StandardCPU", room.StandardCPU),
                new OleDbParameter("@Status", room.Status),
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    public string LoadLocalData()
    {
        try
        {
            string query = "SELECT * FROM Rooms";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Room> rooms = new List<Room>();
            foreach (DataRow row in dataTable.Rows)
            {
                Room room = new Room
                {
                    RoomID = int.Parse(row["RoomID"].ToString()),
                    RoomName = row["RoomName"].ToString(),
                    NumberOfComputers = int.Parse(row["NumberOfComputers"].ToString()),
                    StandardRAM = row["StandardRAM"].ToString(),
                    StandardHDD = row["StandardHDD"].ToString(),
                    StandardCPU = row["StandardCPU"].ToString(),
                    Status = row["Status"].ToString(),
                };
                rooms.Add(room);
            }

            RoomResponse roomResponse = new RoomResponse { Status = "success", data = rooms };
            return JsonConvert.SerializeObject(roomResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }

}
