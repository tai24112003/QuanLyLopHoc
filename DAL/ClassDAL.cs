using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

public class ClassDAL
{
    private readonly IDataService _dataService;

    public ClassDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetAllClass()
    {
        try
        {
            string ClassJson = await _dataService.GetAsync("class");
            return ClassJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> InsertClass(Class classSession)
    {
        try
        {
            string classJson = JsonConvert.SerializeObject(classSession);
            string responseJson = await _dataService.PostAsync("class/", classJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class in DAL", ex);
        }
    }
    public async Task<string> GetLastTimeUpdateFromDB()
    {
        try
        {
            string lastTimeUpdateJson = await _dataService.GetAsync("setting/getSetting");
            return lastTimeUpdateJson;
        }
        catch (HttpRequestException ex)
        {
            // Handle 404 error (Not Found)
            throw new Exception("Last time update API endpoint not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception("Error fetching last time update from API.", ex);
        }
    }
    public void SaveLocalData(string ClassJson)
    {
        var classResponse = JsonConvert.DeserializeObject<ClassResponse>(ClassJson);

        foreach (var classSession in classResponse.data)
        {
            string query = "INSERT INTO `classes` (`ClassID`, `ClassName`, `UserID`, ) VALUES (@ClassID, @ClassName, @UserID)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@ClassID", classSession.ClassID),
                new OleDbParameter("@ClassName", classSession.ClassName),
                new OleDbParameter("@UserID", classSession.UserID),
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }
    public string LoadNegativeIDClasses()
    {
        try
        {
            // Bước 1: Lấy danh sách các lớp có ID âm từ cơ sở dữ liệu
            string query = "SELECT ClassID, ClassName, UserID FROM Classes WHERE ClassID < 0";
            DataTable dataTable = DataProvider.GetDataTable(query, null); // Thực thi câu lệnh SQL

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null; // Nếu không có dữ liệu trả về null
            }

            // Bước 2: Tạo danh sách các lớp từ dữ liệu bảng
            List<Class> negativeClasses = new List<Class>();
            foreach (DataRow row in dataTable.Rows)
            {
                Class classSession = new Class
                {
                    ClassID = int.Parse(row["ClassID"].ToString()),  // Lấy ClassID
                    ClassName = row["ClassName"].ToString(),         // Lấy tên lớp
                    UserID = int.Parse(row["UserID"].ToString())     // Lấy UserID
                };
                negativeClasses.Add(classSession); // Thêm lớp vào danh sách
            }

            // Bước 3: Đóng gói dữ liệu vào ClassResponse và chuyển đổi thành JSON
            ClassResponse classResponse = new ClassResponse { data = negativeClasses };
            return JsonConvert.SerializeObject(classResponse); // Chuyển dữ liệu thành JSON và trả về
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message); // Xử lý lỗi
            return null; // Trả về null nếu có lỗi xảy ra
        }
    }

    public string LoadLocalData()
    {
        try
        {
            string query = "SELECT ClassID, ClassName, UserID FROM classes";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Class> classes = new List<Class>();
            foreach (DataRow row in dataTable.Rows)
            {
                Class classSession = new Class
                {
                    ClassID = int.Parse(row["ClassID"].ToString()),
                    ClassName = row["ClassName"].ToString(),
                    UserID = int.Parse(row["UserID"].ToString())
                };
                classes.Add(classSession);
            }

            ClassResponse classResponse = new ClassResponse { data = classes };
            return JsonConvert.SerializeObject(classResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }
}
