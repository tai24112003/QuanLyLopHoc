using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class RoomBLL
{
    private readonly RoomDAL _RoomDAL;

    public RoomBLL(RoomDAL RoomDAL)
    {
        _RoomDAL = RoomDAL ?? throw new ArgumentNullException(nameof(RoomDAL));
    }

    public List<Room> ParseRooms(string json)
    {
        try
        {
            // Deserialize JSON to RoomResponse object
            var roomResponse = JsonConvert.DeserializeObject<RoomResponse>(json);

            if (roomResponse != null && roomResponse.Status == "success")
            {
                return roomResponse.data; // Return the list of Room objects
            }
            else
            {
                // Handle cases when there's no data or status is not success
                return new List<Room>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error parsing JSON: " + ex.Message);
            return new List<Room>();
        }
    }

    public async Task<Room> GetRoomsByName(string id)
    {
        try
        {
            Console.WriteLine("load api");

            // Get data from the server
            string RoomsJson = await _RoomDAL.GetRoomByRoomName(id);
            List<Room> lstRoom = ParseRooms(RoomsJson);

            if (lstRoom.Count > 0)
            {
                SaveLocalData(RoomsJson); // Save data to local database if API call is successful
                return lstRoom[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Rooms by id from BLL, loading local data: " + ex.Message);

            // Load data from local database when there's an error
            string RoomsJson = LoadLocalData();
            List<Room> lstRoom = ParseRooms(RoomsJson);

            if (lstRoom.Count > 0)
            {
                return lstRoom[0];
            }
            throw new Exception("Error fetching Rooms by id from BLL and no local data available", ex);
        }
    }

    private void SaveLocalData(string RoomsJson)
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

    private string LoadLocalData()
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
