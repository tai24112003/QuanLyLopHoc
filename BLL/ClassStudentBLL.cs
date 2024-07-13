using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Newtonsoft.Json;

public class ClassStudentBLL
{
    private readonly ClassStudentDAL _ClassStudentDAL;
    private readonly StudentDAL _StudentDAL;

    public ClassStudentBLL(ClassStudentDAL ClassStudentDAL, StudentDAL StudentDAL)
    {
        _ClassStudentDAL = ClassStudentDAL ?? throw new ArgumentNullException(nameof(ClassStudentDAL));
        _StudentDAL = StudentDAL ?? throw new ArgumentNullException(nameof(StudentDAL));
    }

    public async Task<ClassStudent> InsertClassStudent(List<ClassStudent> classSession)
    {
        try
        {
            string responseJson = await _ClassStudentDAL.InsertClassStudent(classSession);
            var insertedSession = JsonConvert.DeserializeObject<ClassStudent>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting ClassStudent in BLL: " + ex.Message);

            // Save to local if insertion fails
            var classResponse = new ClassStudentResponse { data = classSession };
            string classJson = JsonConvert.SerializeObject(classResponse);
            SaveLocalData(classJson);

            throw new Exception("Error inserting ClassStudent in BLL. Data saved locally.", ex);
        }
    }

    public async Task<List<ClassStudent>> GetAllClassStudents()
    {
        try
        {
            string ClassStudentsJson = await GetClassStudents();
            ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
            return ClassStudentResponse.data;
        }
        catch (Exception ex)
        { 
            string ClassStudentsJson = LoadLocalData();
            if (!string.IsNullOrEmpty(ClassStudentsJson))
            {
                ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
                return ClassStudentResponse.data;
            }
            throw new Exception("Error fetching ClassStudents from API and local data", ex);
        }
    }

    public async Task<List<ClassStudent>> GetClassStudentsByClassID(int classID)
    {
        List<ClassStudent> allClassStudents = await GetAllClassStudents();
        return allClassStudents.Where(cs => cs.ClassID == classID).ToList();
    }

    public async Task<string> GetClassStudents()
    {
        try
        {
            // Get ClassStudents from server
            string ClassStudentsJson = await _ClassStudentDAL.GetAllClassStudents();
            // Save ClassStudents and last update time to local database
            SaveLocalData(ClassStudentsJson);
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassStudents from BLL", ex);
        }
    }

    public async Task<List<ClassStudent>> GetStudentsByClassID(int classID)
    {
        List<ClassStudent> allStudents = await GetAllClassStudents();
        return allStudents.Where(cs => cs.ClassID == classID).ToList();
    }

    private void SaveLocalData(string ClassStudentsJson)
    {
        var classStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);

        foreach (var classStudent in classStudentResponse.data)
        {
            string query = "INSERT INTO `classes_student` (`ClassID`, `StudentID`) VALUES (@ClassID, @StudentID)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@ClassID", classStudent.ClassID),
                new OleDbParameter("@StudentID", classStudent.StudentID),
                // Add other parameters as needed
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    private string LoadLocalData()
    {
        try
        {
            string query = "SELECT ClassID, StudentID FROM classes_student";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<ClassStudent> classStudents = new List<ClassStudent>();
            foreach (DataRow row in dataTable.Rows)
            {
                ClassStudent classStudent = new ClassStudent
                {
                    ClassID = int.Parse(row["ClassID"].ToString()),
                    StudentID = row["StudentID"].ToString(),
                    // Set other properties as needed
                };
                classStudents.Add(classStudent);
            }

            ClassStudentResponse classStudentResponse = new ClassStudentResponse { data = classStudents };
            return JsonConvert.SerializeObject(classStudentResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }

    public async Task<List<ClassStudent>> GetClassStudentsByID(int id)
    {
        try
        {
            string ClassStudentsJson = await GetClassStudentsByID1(id);
            ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
            return ClassStudentResponse.data;
        }
        catch (Exception ex)
        {
            string ClassStudentsJson = LoadLocalData();
            if (!string.IsNullOrEmpty(ClassStudentsJson))
            {
                ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
                return ClassStudentResponse.data;
            }
            throw new Exception("Error fetching ClassStudents from API and local data", ex);
        }
    }
    public async Task<string> GetClassStudentsByID1(int id)
    {
        try
        {
            // Get ClassStudents from server
            string ClassStudentsJson = await _ClassStudentDAL.GetClassStudentsByID(id);
            // Save ClassStudents and last update time to local database
            SaveLocalData(ClassStudentsJson);

            
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassStudents from BLL", ex);
        }
    }

    public int ProcessPoint()
    {
        string answerJson = File.ReadAllText(@"D:\0306211215\0306211215-anwser.json");
        string examJson = File.ReadAllText(@"D:\exam.json");
        List<Answer> answers = JsonConvert.DeserializeObject<List<Answer>>(answerJson);
        Exam exam = JsonConvert.DeserializeObject<Exam>(examJson);
        int correctAnswerCount = CompareAnswers(exam, answers);

        Console.WriteLine($"Number of correct answers: {correctAnswerCount}");
        return correctAnswerCount;
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
