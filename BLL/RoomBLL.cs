using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class RoomBLL
{
    private readonly RoomDAL _RoomDAL;
    private readonly string localFilePath = "localDataRoom.json";

    public RoomBLL(RoomDAL RoomDAL)
    {
        _RoomDAL = RoomDAL ?? throw new ArgumentNullException(nameof(RoomDAL));
    }

    public List<Room> ParseRooms(string json)
    {
        try
        {
            // Deserialize JSON thành đối tượng RoomResponse
            var roomResponse = JsonConvert.DeserializeObject<RoomResponse>(json);

            if (roomResponse != null && roomResponse.Status == "success")
            {
                return roomResponse.Data; // Trả về danh sách các đối tượng Room
            }
            else
            {
                // Xử lý khi không có dữ liệu hoặc trạng thái không thành công
                return new List<Room>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error parsing JSON: " + ex.Message);
            return new List<Room>();
        }
    }

    public async Task<Room> GetRoomsByID(string id)
    {
        try
        {
            Console.WriteLine("load api");

            // Lấy dữ liệu từ server
            string RoomsJson = await _RoomDAL.GetRoomByRoomID(id);
            List<Room> lstRoom = ParseRooms(RoomsJson);

            if (lstRoom.Count > 0)
            {
                SaveLocalData(RoomsJson); // Lưu dữ liệu vào tệp tin cục bộ nếu lấy từ API thành công
                return lstRoom[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Rooms by id from BLL, loading local data: " + ex.Message);

            // Tải dữ liệu từ tệp tin cục bộ khi có lỗi
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
        var localData = new LocalDataRoomResponse
        {
            RoomJson = RoomsJson,
        };
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localData));
    }

    private string LoadLocalData()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataRoomResponse>(localData);
            return localResponse.RoomJson;
        }
        return null;
    }
}
