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
            throw new Exception("Error inserting ClassStudent in DAL", ex);
        }
    }
    public async Task<string> GetClassStudentsByID(int ID)
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
    public List<ClassStudent> GetClassStudentsByClassID(int classID)
    {
        // Kiểm tra nếu classID là âm thì lấy danh sách sinh viên có ID âm
        int effectiveClassID = classID < 0 ? classID : classID * -1;

        string query = "SELECT * FROM Class_Student WHERE ClassID = @ClassID";

        OleDbParameter[] parameters = new OleDbParameter[]
        {
        new OleDbParameter("@ClassID", effectiveClassID)
        };

        // Lấy dữ liệu từ database
        DataTable dataTable = DataProvider.GetDataTable(query, parameters);

        // Kiểm tra nếu không có dữ liệu thì trả về danh sách rỗng
        if (dataTable == null || dataTable.Rows.Count == 0)
        {
            return new List<ClassStudent>();
        }

        // Chuyển đổi từ DataTable sang danh sách ClassStudent
        List<ClassStudent> classStudents = new List<ClassStudent>();
        foreach (DataRow row in dataTable.Rows)
        {
            ClassStudent classStudent = new ClassStudent
            {
                ClassID = Convert.ToInt32(row["ClassID"]),
                StudentID = row["StudentID"].ToString(),
            };

            classStudents.Add(classStudent);
        }

        return classStudents;
    }



}
