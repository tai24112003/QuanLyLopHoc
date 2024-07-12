using System;
using System.Collections.Generic;

public class LastTimeUpdateResponse
{
    public List<LastTimeUpdateData> data { get; set; }
}

public class LastTimeUpdateData
{
    public string lastTimeUpdateStudent { get; set; }
    public string lastTimeUpdateClass { get; set; }
}