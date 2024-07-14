using DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class SubmisstionBLL
{
    private readonly SubmisstionDAL _AnswerDAL;

    public SubmisstionBLL(SubmisstionDAL AnswerDAL)
    {
        _AnswerDAL = AnswerDAL ?? throw new ArgumentNullException(nameof(AnswerDAL));
    }

    public async Task InsertAnswer(int sessionId, List<Submission> Answers)
    {
        try
        {
            await _AnswerDAL.InsertAnswer(Answers);
        }
        catch (Exception ex)
        {

            //await _AnswerDAL.InsertAnswerLocal(sessionId, Answers);
            Console.WriteLine("Error inserting submisstion in BLL: " + ex.Message);
        }
    }
    public double ProcessPoint(string mssv,Submission submission)
    {
        try
        {
            string answerJson = File.ReadAllText(@"C:\" + mssv + "\\" + mssv + "-anwser.json");
            string examJson = File.ReadAllText(@"C:\exam.json");
            List<Answer> answers = JsonConvert.DeserializeObject<List<Answer>>(answerJson);
            Exam exam = JsonConvert.DeserializeObject<Exam>(examJson);
            int correctAnswerCount = CompareAnswers(exam, answers);
            submission.questions = answers;
            submission.code = exam.code;
            Console.WriteLine($"Number of correct answers: {correctAnswerCount}");
            return (correctAnswerCount/exam.questionCount)*10.0;
        }catch (Exception ex)
        {
            Console.WriteLine("Error process points submisstion in BLL: " + ex.Message);
            return -1;
        }
    }
    public int CompareAnswers(Exam exam, List<Answer> answers)
    {
        int correctAnswers = 0;

        foreach (var question in exam.questions)
        {
            var studentAnswer = answers.FirstOrDefault(a => a.ID == question.id);
            if (studentAnswer != null && AreAnswersCorrect(question, studentAnswer.answer))
            {
                correctAnswers++;
                studentAnswer.IsConnrect = true;
            }
        }

        return correctAnswers;
    }

    private bool AreAnswersCorrect(Question question, List<string> studentAnswers)
    {
        if (studentAnswers.Count != studentAnswers.Count)
        {
            return false;
        }
        var correctAnswersMapped = question.answer.Select(a => MapOptionToLetter(question.options, a)).ToList();



        correctAnswersMapped.Sort();
        studentAnswers.Sort();

        return !correctAnswersMapped.Where((t, i) => t != studentAnswers[i]).Any();
    }

    private string MapOptionToLetter(List<string> options, string option)
    {
        int index = options.IndexOf(option);
        if (index == -1)
        {
            throw new ArgumentException($"Option '{option}' not found in options list.");
        }
        return ((char)('A' + index)).ToString();
    }

}
