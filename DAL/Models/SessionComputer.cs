﻿public class SessionComputer
{
    public int ID { get; set; }
    public int SessionID { get; set; }

    public string ComputerName { get; set; }
    public string RAM { get; set; }
    public string HHD { get; set; }
    public string CPU { get; set; }
    public bool MouseConnected { get; set; }
    public bool KeyboardConnected{ get; set; }
    public bool MonitorConnected { get; set; }
    public string MismatchInfo { get; set; }
    public string RepairNote { get; set; }
    public string StudentID { get; set; }
}
