using System.Collections.Generic;

public class ExcelData
{
    public string ClassName { get; set; }
    public string SubjectName { get; set; }
    public string TeacherName { get; set; }
    public List<Student> Students { get; set; } = new List<Student>();
}