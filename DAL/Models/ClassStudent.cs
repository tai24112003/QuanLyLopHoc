using System.Collections.Generic;

public class ClassStudent
{
    public int ClassID { get; set; }
    public string StudentID { get; set; }
    public string LastTime { get; set; }
    public Student Student { get; set; }

}

public class ClassStudentResponse
{
    public List<ClassStudent> data { get; set; }
}