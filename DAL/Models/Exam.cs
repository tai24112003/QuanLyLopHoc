using System;
using System.Collections.Generic;
using DAL.Models;

public class Exam
{
    public int id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public int questionCount { get; set; }
    public Subject subject { get; set; }
    public List<Question> questions { get; set; }

}
public class ExamResponse
{
    public Exam data { get; set; }
}


