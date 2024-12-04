using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
            string classJson = await _dataService.GetAsync("class");
            return classJson;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching all classes", ex);
            return null;
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
            Console.WriteLine("Error inserting class in DAL", ex);
            return null;
        }
    }

    public async Task SaveLocalData(string classJson)
    {
        try
        {
            var classResponse = JsonConvert.DeserializeObject<ClassResponse>(classJson);

            foreach (var classSession in classResponse.data)
            {
                // Kiểm tra nếu ClassID đã tồn tại
                string checkQuery = "SELECT COUNT(*) FROM `classes` WHERE `ClassID` = @ClassID";
                OleDbParameter[] checkParams = new OleDbParameter[]
                {
                new OleDbParameter("@ClassID", classSession.ClassID)
                };

                int count = (int)await Task.Run(() => Convert.ToInt32(DataProvider.RunScalar(checkQuery, checkParams)));

                if (count > 0)
                {
                    // Nếu tồn tại, cập nhật
                    string updateQuery = "UPDATE `classes` SET `ClassName` = @ClassName, `UserID` = @UserID, `LastTime` = @LastTime WHERE `ClassID` = @ClassID";

                    OleDbParameter[] updateParams = new OleDbParameter[]
                    {
                    new OleDbParameter("@ClassName", classSession.ClassName),
                    new OleDbParameter("@UserID", classSession.UserID),
                    new OleDbParameter("@LastTime", classSession.LastTime),
                    new OleDbParameter("@ClassID", classSession.ClassID)
                    };

                    bool updated = await Task.Run(() => DataProvider.RunNonQuery(updateQuery, updateParams));

                    if (!updated)
                    {
                        Console.WriteLine($"Failed to update class session with ClassID: {classSession.ClassID}");
                    }
                }
                else
                {
                    // Nếu không tồn tại, thêm mới
                    string insertQuery = "INSERT INTO `classes` (`ClassID`, `ClassName`, `UserID`, `LastTime`) VALUES (@ClassID, @ClassName, @UserID, @LastTime)";

                    OleDbParameter[] insertParams = new OleDbParameter[]
                    {
                    new OleDbParameter("@ClassID", classSession.ClassID),
                    new OleDbParameter("@ClassName", classSession.ClassName),
                    new OleDbParameter("@UserID", classSession.UserID),
                    new OleDbParameter("@LastTime", classSession.LastTime)
                    };

                    bool inserted = await Task.Run(() => DataProvider.RunNonQuery(insertQuery, insertParams));

                    if (!inserted)
                    {
                        Console.WriteLine($"Failed to insert class session with ClassID: {classSession.ClassID}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving local data: " + ex.Message);
        }
    }


    public async Task<List<Class>> LoadNegativeIDClasses()
    {
        try
        {
            string query = "SELECT * FROM Classes WHERE ClassID < 0";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Class> negativeClasses = new List<Class>();
            foreach (DataRow row in dataTable.Rows)
            {
                Class classSession = new Class
                {
                    ClassID = int.Parse(row["ClassID"].ToString()),
                    ClassName = row["ClassName"].ToString(),
                    LastTime = row["LastTime"].ToString(),
                    UserID = int.Parse(row["UserID"].ToString())
                };
                negativeClasses.Add(classSession);
            }

            return negativeClasses;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading classes with negative IDs", ex);
            return null;
        }
    }
    public async Task<string> GetClasssByDateRange(DateTime startTime, DateTime endTime)
    {
        try
        {
            // Chuyển đổi ngày thành định dạng chuỗi để gửi qua query string
            string formattedStartTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss");
            string formattedEndTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss");

            // Tạo URL với query string
            string url = $"class/getClassBetween?startDate={formattedStartTime}&endDate={formattedEndTime}";

            // Gọi API và lấy dữ liệu dưới dạng JSON
            string ClasssJson = await _dataService.GetAsync(url);

            return ClasssJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<string> LoadLocalData()
    {
        try
        {
            string query = "SELECT ClassID, ClassName, UserID FROM classes";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

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
            Console.WriteLine("Error loading local data", ex);
            return null;
        }
    }

    public async Task DeleteClassLocalByClassID(int ClassID)
    {
        if (string.IsNullOrEmpty(ClassID.ToString()))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(ClassID));
        }

        try
        {
            string query = "DELETE FROM Classes WHERE ClassID = @ClassID";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
            new OleDbParameter("@ClassID", ClassID)
            };

            // Using Task.Run to make RunNonQuery asynchronous
            bool result = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            if (!result)
            {
                Console.WriteLine($"No Class found with ID: {ClassID}, or an error occurred during deletion.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception with the relevant student ID information
            Console.WriteLine($"Error deleting Class with ID {ClassID} in DAL: {ex.Message}");
            Console.WriteLine($"Error deleting Class with ID {ClassID} in DAL.", ex);
        }
    }
}
