using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class StudentBLL
{
    private readonly StudentDAL _StudentDAL;

    public StudentBLL(StudentDAL StudentDAL)
    {
        _StudentDAL = StudentDAL ?? throw new ArgumentNullException(nameof(StudentDAL));
    }

    public async Task<StudentResponse> InsertStudent(List<Student> classSession)
    {
        try
        {
            string responseJson = await _StudentDAL.InsertListStudent(classSession);
            var insertedSession = JsonConvert.DeserializeObject<StudentResponse>(responseJson);
            await InsertStudentLocal(classSession);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting Student in BLL: " + ex.Message);
            foreach (var _classSession in classSession)
            {
                if (!_classSession.StudentID.StartsWith("-"))
                    _classSession.StudentID = "-" + _classSession.StudentID;
            }
            // Save to local if insertion fails
            await InsertStudentLocal(classSession);

            Console.WriteLine("Error inserting Student in BLL. Data saved locally.", ex);
            return null;
        }
    }

    public async Task InsertStudentLocal(List<Student> classSession)
    {
        try
        {
            var studentResponse = new StudentResponse { data = classSession };
            string studentJson = JsonConvert.SerializeObject(studentResponse);
            await _StudentDAL.SaveLocalData(studentJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting Student in BLL: " + ex.Message);
            Console.WriteLine("Error inserting Student in BLL. Data saved locally.", ex);
        }
    }

    public async Task<List<Student>> GetAllStudents()
    {
        try
        {
            string StudentsJson = await GetStudents();
            StudentResponse StudentResponse = JsonConvert.DeserializeObject<StudentResponse>(StudentsJson);
            return StudentResponse.data;
        }
        catch (Exception ex)
        {
            string StudentsJson = await _StudentDAL.LoadLocalData();
            if (!string.IsNullOrEmpty(StudentsJson))
            {
                StudentResponse StudentResponse = JsonConvert.DeserializeObject<StudentResponse>(StudentsJson);
                return StudentResponse.data;
            }
            Console.WriteLine("Error fetching Students from API and local data", ex);
            return null;
        }
    }

    public async Task<List<Student>> LoadNegativeIDStudentes()
    {
        try
        {
            string StudentJson = await _StudentDAL.GetStudentsWithNegativeID();
            if (!string.IsNullOrEmpty(StudentJson))
            {
                StudentResponse StudentResponse = JsonConvert.DeserializeObject<StudentResponse>(StudentJson);
                return StudentResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Class negative from local data", ex);
            return null;
        }
    }

    public async Task<string> GetStudents()
    {
        try
        {
            // Get Students from server
            string StudentsJson = await _StudentDAL.GetAllStudents();
            // Save Students and last update time to local database
            await _StudentDAL.SaveLocalData(StudentsJson);
            return StudentsJson;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Students from BLL", ex);
            return null;
        }
    }

    public async Task DeleteStudent(string studentId)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(studentId));
        }

        try
        {
            await _StudentDAL.DeleteStudentLocal(studentId);
            Console.WriteLine($"Student with ID {studentId} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student in BLL: {ex.Message}");
            Console.WriteLine($"Error deleting student with ID {studentId} in BLL.", ex);
        }
    }

    public async Task<List<Student>> GetStudentsByDateRange(DateTime startTime, DateTime endTime)
    {
        try
        {
            string studentsJson = await _StudentDAL.GetStudentsUpdatedBetween(startTime, endTime);
            StudentResponse studentResponse = JsonConvert.DeserializeObject<StudentResponse>(studentsJson);
            return studentResponse.data;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching students by date range in BLL: " + ex.Message);
            Console.WriteLine("Error fetching students by date range in BLL.", ex);
            return null;
        }
    }
    public async Task<StudentResponse> UpdateStudent(List<Student> students)
    {
        try
        {
            string responseJson = await _StudentDAL.UpdateListStudent(students);
            var updatedStudents = JsonConvert.DeserializeObject<StudentResponse>(responseJson);
            await InsertStudentLocal(students);
            return updatedStudents;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating Student in BLL: " + ex.Message);

            // Handle local updates if API fails
            await InsertStudentLocal(students);

            Console.WriteLine("Data saved locally due to update error.", ex);
            return null;
        }
    }

}
