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
            return await _dataService.GetAsync("student");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching all students in DAL.", ex);
            return null;
        }
    }

    public async Task<string> InsertListStudent(List<Student> student)
    {
        try
        {
            string studentJson = JsonConvert.SerializeObject(student);
            return await _dataService.PostAsync("student/", studentJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting students in DAL.", ex);
            return null;
        }
    }

    public async Task<string> GetStudentsUpdatedBetween()
    {
        try
        {
            return await _dataService.GetAsync("student/");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching updated students in DAL.", ex);
            return null;
        }
    }

    public async Task SaveLocalData(string studentsJson)
    {
        try
        {
            var studentResponse = JsonConvert.DeserializeObject<StudentResponse>(studentsJson);

            foreach (var student in studentResponse.data)
            {
                string query = "INSERT INTO `students` (`StudentID`, `FirstName`, `LastName`, `LastTime`) VALUES (@StudentID, @FirstName, @LastName, @LastTime)";

                OleDbParameter[] parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@StudentID", student.StudentID),
                    new OleDbParameter("@FirstName", student.FirstName),
                    new OleDbParameter("@LastName", student.LastName),
                    new OleDbParameter("@LastTime", student.LastTime),
                };

                await Task.Run(() => DataProvider.RunNonQuery(query, parameters));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving local data in DAL.", ex);
        }
    }

    public async Task<string> LoadLocalData()
    {
        try
        {
            string query = "SELECT * FROM students";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            var students = dataTable.AsEnumerable().Select(row => new Student
            {
                StudentID = row["StudentID"].ToString(),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
            }).ToList();

            var studentResponse = new StudentResponse { data = students };
            return JsonConvert.SerializeObject(studentResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access in DAL.", ex);
            return null;
        }
    }

    public async Task<string> GetStudentsWithNegativeID()
    {
        try
        {
            string query = "SELECT * FROM Students WHERE StudentID LIKE '-%'";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            var students = dataTable.AsEnumerable().Select(row => new Student
            {
                StudentID = row["StudentID"].ToString(),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                LastTime = row["LastTime"].ToString(),
            }).ToList();

            var studentResponse = new StudentResponse { data = students };
            return JsonConvert.SerializeObject(studentResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching students with negative IDs in DAL.", ex);
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
            OleDbParameter[] parameters = { new OleDbParameter("@StudentID", studentId) };

            bool result = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            if (!result)
            {
                Console.WriteLine($"No student found with ID: {studentId}, or an error occurred during deletion.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student with ID {studentId} in DAL.", ex);
        }
    }

    public async Task<string> GetStudentsUpdatedBetween(DateTime startTime, DateTime endTime)
    {
        try
        {
            string formattedStartTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss");
            string formattedEndTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss");
            string url = $"student/getBetween?startTime={formattedStartTime}&endTime={formattedEndTime}";

            return await _dataService.GetAsync(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching students by time range in DAL.", ex);
            return null;
        }
    }
}
