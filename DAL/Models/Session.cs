using System;
using System.Collections.Generic;

public class Session
{
    public string ID { get; set; }
    public string Display { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }

}


public class SessionResponse
{
    public List<Session> data { get; set; }
}