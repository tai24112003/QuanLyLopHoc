using System;
using System.Collections.Generic;

public class Room
{
    public int RoomID { get; set; }
    public string RoomName { get; set; }
    public int NumberOfComputers { get; set; }
    public string StandardRAM { get; set; }
    public string StandardHDD { get; set; }
    public string StandardCPU { get; set; }
    public string Status { get; set; }


}

public class RoomResponse
{
    public string Status { get; set; }
    public List<Room> data { get; set; }
}

public class LocalDataRoomResponse
{
    public string RoomJson { get; set; }
}