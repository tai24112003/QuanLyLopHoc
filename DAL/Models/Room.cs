using System;
using System.Collections.Generic;

public class Room
{
    public string RoomID { get; set; }
    public int NumberOfComputers { get; set; }
    public string StandardRAM { get; set; }
    public string StandardHDD { get; set; }
    public string StandardCPU { get; set; }
    public string Status { get; set; }
    public string lastTimeUpdateRoom { get; set; }


}

public class RoomResponse
{
    public string Status { get; set; }
    public List<Room> Data { get; set; }
}

public class LocalDataRoomResponse
{
    public string RoomJson { get; set; }
}