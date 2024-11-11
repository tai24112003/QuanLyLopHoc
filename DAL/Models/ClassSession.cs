using System;
using System.Collections.Generic;

public class ClassSession
{
    public int SessionID { get; set; }

    public int ClassID { get; set; }
    public int Session { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }

    public int user_id { get; set; }

    public string RoomID { get; set; }

    public int AttendanceID { get; set; }
    public string StudentID { get; set; }
    public string Present { get; set; }
    public string Remarks { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string LastTime { get; set; }
}


public class ClassSessionResponse
{
    public string status { get; set; }
    public List<ClassSession> data { get; set; }
}