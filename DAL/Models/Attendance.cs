
using System.Collections.Generic;

public class Attendance
{
    public int AttendanceID { get; set; }

    public string StudentID { get; set; }
    public int SessionID { get; set; }
    public string Present { get; set; }


}


public class AttendanceResponse
{
    public List<Attendance> data { get; set; }
}

