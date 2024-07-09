using System.Collections.Generic;

public class ExcelData
{
    public int ClassID { get; set; }
    public string ClassName { get; set; }
    public string TeacherName { get; set; }
    public List<Student> Students { get; set; } = new List<Student>();
}