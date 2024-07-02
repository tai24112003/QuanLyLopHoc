using System;
using System.Collections.Generic;

public class ClassSession
{
    public int SessionID { get; set; }

    public string ClassName { get; set; }
    public int Session { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public int user_id { get; set; }

    public string RoomID { get; set; }
}


public class ResponseClassSession
{
    public string status { get; set; }
    public List<ClassSession> data { get; set; }
}