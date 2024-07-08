using System.Collections.Generic;

public class ClassSubject
{
    public int ClassID { get; set; }
    public int SubjectID { get; set; }
    public string SubjectName { get; set; }

}

public class ClassSubjectResponse
{
    public List<ClassSubject> data { get; set; }
}