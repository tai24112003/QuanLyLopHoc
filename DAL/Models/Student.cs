using System;
using System.Collections.Generic;

public class Student
{
    public string StudentID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string  LastTime { get; set; }
}


public class StudentResponse
{
    public List<Student> data { get; set; }
}