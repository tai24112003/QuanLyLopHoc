using System.Collections.Generic;

public class SessionComputer
{
    public int SessionID { get; set; }
    public int ComputerID { get; set; }

    public string ComputerName { get; set; }
    public string RAM { get; set; }
    public string HDD { get; set; }
    public string CPU { get; set; }
    public bool MouseConnected { get; set; }
    public bool KeyboardConnected{ get; set; }
    public bool MonitorConnected { get; set; }
    public string MismatchInfo { get; set; }
    public string RepairNote { get; set; }
    public string StudentID { get; set; }
}


public class SessionComputerResponse
{
    public List<SessionComputer> data { get; set; }

}
