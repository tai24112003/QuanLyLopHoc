using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StudentDAL
{
    private readonly IDataService _dataService;

    public StudentDAL(IDataService dataService)
    {
        _dataService = dataService;
    }
    public async Task<string> GetAllStudents()
    {
        try
        {
            string ClassJson = await _dataService.GetAsync("student");
            return ClassJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> InsertListStudent(List<Student> student)
    {
        try
        {
            string studentJson = JsonConvert.SerializeObject(student);
            string responseJson = await _dataService.PostAsync("student/", studentJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting Student in DAL", ex);
        }
    }
    public void SaveLocalData(string StudentsJson)
    {
        var studentResponse = JsonConvert.DeserializeObject<StudentResponse>(StudentsJson);

        foreach (var student in studentResponse.data)
        {
            string query = "INSERT INTO `students` (`StudentID`, `FirstName`, `LastName`,`LastTime`) VALUES (@StudentID, @FirstName, @LastName,@LastTime)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@StudentID", student.StudentID),
                new OleDbParameter("@FirstName", student.FirstName),
                new OleDbParameter("@LastName", student.LastName),
                new OleDbParameter("@LastTime", student.LastTime),
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    public string LoadLocalData()
    {
        try
        {
            string query = "SELECT * FROM students";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Student> students = new List<Student>();
            foreach (DataRow row in dataTable.Rows)
            {
                Student student = new Student
                {
                    StudentID = row["StudentID"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                };
                students.Add(student);
            }

            StudentResponse studentResponse = new StudentResponse { data = students };
            return JsonConvert.SerializeObject(studentResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }

   
    public string GetStudentsWithNegativeID()
    {
        try
        {
            string query = "SELECT * FROM Students WHERE StudentID < 0";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Student> students = new List<Student>();
            foreach (DataRow row in dataTable.Rows)
            {
                Student student = new Student
                {
                    StudentID = row["StudentID"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                };
                students.Add(student);
            }

            StudentResponse studentResponse = new StudentResponse { data = students };
            return JsonConvert.SerializeObject(studentResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }

    public async Task DeleteStudentLocal(string studentId)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(studentId));
        }

        try
        {
            string query = "DELETE FROM Students WHERE StudentID = @StudentID";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@StudentID", studentId)
            };

            // Using Task.Run to make RunNonQuery asynchronous
            bool result = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            if (!result)
            {
                throw new Exception($"No student found with ID: {studentId}, or an error occurred during deletion.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception with the relevant student ID information
            Console.WriteLine($"Error deleting student with ID {studentId} in DAL: {ex.Message}");
            throw new Exception($"Error deleting student with ID {studentId} in DAL.", ex);
        }
    }

}
