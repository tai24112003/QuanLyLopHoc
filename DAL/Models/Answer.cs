using System;
using System.Collections.Generic;

public class Answer
{
    public int STT { get; set; }
    public int ID { get; set; }
    public string Text { get; set; }
    public List<string> answer { get; set; }
    public List<string> opton { get; set; }
    public string Type { get; set; }
    public int idxList { get; set; }
    public int idxSub { get; set; }

    public string StudentID { get; set; }
    public int SessionID { get; set; }
    public bool IsConnrect { get; set; }
}

