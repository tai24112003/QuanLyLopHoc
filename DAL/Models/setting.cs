using System;
using System.Collections.Generic;

public class LastTimeUpdateResponse
{
    public List<LastTimeUpdateData> data { get; set; }
}

public class LastTimeUpdateData
{
    public DateTime lastTimeUpdateUser { get; set; }
}