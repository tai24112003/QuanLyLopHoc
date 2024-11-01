using System;
using System.Collections.Generic;

public class SettingResponse
{
    public List<Setting> data { get; set; }
}

public class Setting
{
    public string lastTimeUpdateStudent { get; set; }
    public string lastTimeUpdateComputer { get; set; }
    public string lastTimeUpdateClass { get; set; }
}