using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class ClassStudentDAL
{
    private readonly IDataService _dataService;

    public ClassStudentDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetAllClassStudents()
    {
        try
        {
            string ClassStudentsJson = await _dataService.GetAsync("class_student");
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
   
    public async Task<string> InsertClassStudent(List<ClassStudent> student)
    {
        try
        {
            string studentJson = JsonConvert.SerializeObject(student);
            string responseJson = await _dataService.PostAsync("class_student/", studentJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting ClassStudent in DAL", ex);
            return null;
        }
    }
    public async Task<string> GetClassStudentsByIDAPI(int ID)
    {
        try
        {
            string ClassStudentsJson = await _dataService.GetAsync("class_student/"+ID);
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public ClassStudentResponse GetClassStudentsByClassID(int classID)
    {
        // Kiểm tra nếu classID là âm thì lấy danh sách sinh viên có ID âm

        string query = "SELECT cs.ClassID, cs.StudentID, s.FirstName, s.LastName, s.LastTime " +
                       "FROM Classes_Student AS cs " +
                       "INNER JOIN Students AS s ON cs.StudentID = s.StudentID " +
                       "WHERE cs.ClassID = @ClassID";

        OleDbParameter[] parameters = new OleDbParameter[]
        {
        new OleDbParameter("@ClassID", classID)
        };

        // Lấy dữ liệu từ database
        DataTable dataTable = DataProvider.GetDataTable(query, parameters);

        // Khởi tạo ClassStudentResponse
        ClassStudentResponse response = new ClassStudentResponse
        {
            data = new List<ClassStudent>()
        };

        // Kiểm tra nếu không có dữ liệu thì trả về danh sách rỗng
        if (dataTable == null || dataTable.Rows.Count == 0)
        {
            return response; // Danh sách rỗng
        }

        // Chuyển đổi từ DataTable sang danh sách ClassStudent
        foreach (DataRow row in dataTable.Rows)
        {
            ClassStudent classStudent = new ClassStudent
            {
                ClassID = Convert.ToInt32(row["ClassID"]),
                StudentID = row["StudentID"].ToString(),
                LastTime = row["LastTime"].ToString(), // Cập nhật LastTime từ bảng Class_Student
                Student = new Student
                {
                    StudentID = row["StudentID"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    LastTime = row["LastTime"].ToString() // Thông tin LastTime từ bảng Students
                }
            };

            response.data.Add(classStudent);
        }

        return response;
    }


    public void SaveLocalData(string ClassStudentsJson)
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

    public string LoadLocalData()
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

    public async Task DeleteClassStudentLocal(int ClassID)
    {
        if (string.IsNullOrEmpty(ClassID.ToString()))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(ClassID));
        }

        try
        {
            string query = "DELETE FROM Classes_Student WHERE ClassID = @ClassID";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@ClassID", ClassID)
            };

            // Using Task.Run to make RunNonQuery asynchronous
            bool result = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            if (!result)
            {
                Console.WriteLine($"No ClassStudent found with ID: {ClassID}, or an error occurred during deletion.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception with the relevant student ID information
            Console.WriteLine($"Error deleting ClassStudent with ID {ClassID} in DAL: {ex.Message}");
            Console.WriteLine($"Error deleting ClassStudent with ID {ClassID} in DAL.", ex);
        }
    }
    public async Task DeleteClassStudentLocalByStudentID(string StudentID)
    {
        if (string.IsNullOrEmpty(StudentID.ToString()))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(StudentID));
        }

        try
        {
            string query = "DELETE FROM Classes_Student WHERE StudentID = @StudentID";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@StudentID", StudentID)
            };

            // Using Task.Run to make RunNonQuery asynchronous
            bool result = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            if (!result)
            {
                Console.WriteLine($"No ClassStudent found with ID: {StudentID}, or an error occurred during deletion.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception with the relevant student ID information
            Console.WriteLine($"Error deleting ClassStudent with ID {StudentID} in DAL: {ex.Message}");
            Console.WriteLine($"Error deleting ClassStudent with ID {StudentID} in DAL.", ex);
        }
    }
}
