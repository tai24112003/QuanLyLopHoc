using System.Collections.Generic;

public class Computer
{
    public string RoomID { get; set; }

    public string ComputerName { get; set; }
    public string RAM { get; set; }
    public string HHD { get; set; }
    public string CPU { get; set; }
   
}


public class ComputerResponse
{
    public string Status;
    public List<Computer> data { get; set; }
}