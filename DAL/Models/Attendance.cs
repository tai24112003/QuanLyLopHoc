
using System.Collections.Generic;

public class Attendance
{
    public int AttendanceID { get; set; }

    public int StudentID { get; set; }
    public int SessionID { get; set; }
    public bool Present { get; set; }

    public string Remarks { get; set; }

}


public class AttendanceResponse
{
    public List<Attendance> data { get; set; }
}

