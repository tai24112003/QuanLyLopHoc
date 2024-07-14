
using DAL.Models;
using System.Collections.Generic;

public class Submission
{
    public string code { get; set; }
    public List<Answer> questions { get; set; }
    public string StudentID { get; set; }
    public double Score { get; set; }
    public int SessionID { get; set; }
}