using System;
using System.Collections.Generic;

public class LastTimeUpdateResponse
{
    public List<LastTimeUpdateData> data { get; set; }
}

public class LastTimeUpdateData
{
    public string lastTimeUpdateUser { get; set; }
    public string lastTimeUpdateSubject { get; set; }
    public string lastTimeUpdateClass { get; set; }
}