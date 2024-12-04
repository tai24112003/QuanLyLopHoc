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
        try
        {
            var roomResponse = JsonConvert.DeserializeObject<RoomResponse>(RoomsJson);

            foreach (var room in roomResponse.data)
            {
                // Step 1: Check if the room already exists
                string checkQuery = "SELECT COUNT(*) FROM `Rooms` WHERE `RoomID` = @RoomID";
                OleDbParameter[] checkParameters = new OleDbParameter[]
                {
                new OleDbParameter("@RoomID", room.RoomID)
                };

                int existingRoomCount = Convert.ToInt32(DataProvider.RunScalar(checkQuery, checkParameters));

                if (existingRoomCount > 0)
                {
                    // Step 2: If the room exists, update it
                    string updateQuery = "UPDATE `Rooms` SET `RoomName` = @RoomName, `NumberOfComputers` = @NumberOfComputers, " +
                                         "`StandardRAM` = @StandardRAM, `StandardHDD` = @StandardHDD, " +
                                         "`StandardCPU` = @StandardCPU, `Status` = @Status " +
                                         "WHERE `RoomID` = @RoomID";

                    OleDbParameter[] updateParameters = new OleDbParameter[]
                    {
                    new OleDbParameter("@RoomID", room.RoomID),
                    new OleDbParameter("@RoomName", room.RoomName),  // Add RoomName parameter
                    new OleDbParameter("@NumberOfComputers", room.NumberOfComputers),
                    new OleDbParameter("@StandardRAM", room.StandardRAM),
                    new OleDbParameter("@StandardHDD", room.StandardHDD),
                    new OleDbParameter("@StandardCPU", room.StandardCPU),
                    new OleDbParameter("@Status", room.Status)
                    };

                    DataProvider.RunNonQuery(updateQuery, updateParameters);
                }
                else
                {
                    // Step 3: If the room does not exist, insert the new data
                    string insertQuery = "INSERT INTO `Rooms` (`RoomID`, `RoomName`, `NumberOfComputers`, `StandardRAM`, `StandardHDD`, `StandardCPU`, `Status`) " +
                                         "VALUES (@RoomID, @RoomName, @NumberOfComputers, @StandardRAM, @StandardHDD, @StandardCPU, @Status)";

                    OleDbParameter[] insertParameters = new OleDbParameter[]
                    {
                    new OleDbParameter("@RoomID", room.RoomID),
                    new OleDbParameter("@RoomName", room.RoomName),  // Add RoomName parameter
                    new OleDbParameter("@NumberOfComputers", room.NumberOfComputers),
                    new OleDbParameter("@StandardRAM", room.StandardRAM),
                    new OleDbParameter("@StandardHDD", room.StandardHDD),
                    new OleDbParameter("@StandardCPU", room.StandardCPU),
                    new OleDbParameter("@Status", room.Status),
                    };

                    DataProvider.RunNonQuery(insertQuery, insertParameters);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving room data: " + ex.Message);
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
