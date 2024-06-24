using System;
using System.Collections.Generic;

public class Subject
{
    public int id { get; set; }
    public string name { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public List<object> Chapters { get; set; }
}

public class SubjectResponse
{
    public List<Subject> data { get; set; }
}