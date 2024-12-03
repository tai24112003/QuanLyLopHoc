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
            // Get data from the server
            string RoomsJson = await _RoomDAL.GetRoomByRoomName(id);
            if (RoomsJson == null) return null;
            List<Room> lstRoom = ParseRooms(RoomsJson);

            if (lstRoom.Count > 0)
            {
                _RoomDAL.SaveLocalData(RoomsJson); // Save data to local database if API call is successful
                return lstRoom[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Rooms by id from BLL, loading local data: " + ex.Message);

            // Load data from local database when there's an error
            string RoomsJson = _RoomDAL.LoadLocalData();
            List<Room> lstRoom = ParseRooms(RoomsJson);

            if (lstRoom.Count > 0)
            {
                return lstRoom[0];
            }
            Console.WriteLine("Error fetching Rooms by id from BLL and no local data available", ex);
            return null;
        }
    }

    public async Task UpdateRoom(string id,Room room)
    {
        try
        {
            // Get data from the server
            string RoomsJson = JsonConvert.SerializeObject(room);
             await _RoomDAL.UpdateRoom(id,RoomsJson); 
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Rooms by id from BLL, loading local data: " + ex.Message);


        }
    }


}
