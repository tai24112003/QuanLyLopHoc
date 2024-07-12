using System;
using System.Collections.Generic;

public class Class
{
    public int ClassID { get; set; }
    public string ClassName { get; set; }
    public int UserID { get; set; }

    public string LastTime { get; set; }
}
public class ClassResponse
{
    public List<Class> data { get; set; }
}
