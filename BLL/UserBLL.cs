using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class UserBLL
{
    private readonly UserDAL _userDAL;

    public UserBLL(UserDAL userDAL)
    {
        _userDAL = userDAL ?? throw new ArgumentNullException(nameof(userDAL));
    }

    public async Task<List<User>> GetListUser(string role)
    {
        try
        {
            string usersJson = await GetUsersByRole(role);
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);
            return userResponse.Data;
        }
        catch (Exception ex)
        {
            string usersJson = LoadLocalData();
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);
            return userResponse.Data;
            throw new Exception("Error fetching user list by role from BLL", ex);
        }
    }

    public async Task<string> GetUsersByRole(string role)
    {
        try
        {
            //// Check local file for last update time
            //DateTime? localLastUpdateTime = GetLocalLastTimeUpdate();

            //// Get last update time from server
            //string lastTimeUpdateJson = await _userDAL.GetLastTimeUpdateFromDB();
            //var lastTimeUpdateResponse = JsonConvert.DeserializeObject<LastTimeUpdateResponse>(lastTimeUpdateJson);
            //DateTime serverLastUpdateTime;
            //DateTime.TryParseExact(lastTimeUpdateResponse.data[0].lastTimeUpdateUser, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out serverLastUpdateTime);

            //if (localLastUpdateTime.HasValue && localLastUpdateTime.Value >= serverLastUpdateTime)
            //{
            //    // Load users from local file
            //    Console.WriteLine("load local");
            //    return LoadLocalData();
            //}
            //else
            //{
                Console.WriteLine("load api user");

                // Get users from server
                string usersJson = await _userDAL.GetUsersByRole(role);
                SaveLocalUserAccessData(usersJson);

                // Save users and last update time to local file
                return usersJson;
            //}
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching users by role from BLL", ex);
        }
    }

    public void SaveLocalUserAccessData(string usersJson)
    {
        var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);

        foreach (var user in userResponse.Data)
        {
            string query = "INSERT INTO `users` (`id`, `email`, `name`, `phone`, `password`, `role`) VALUES (@userId, @Email, @Name, @Phone, @Password, @Role)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@userId", user.user_id),
                new OleDbParameter("@Email", user.email),
                new OleDbParameter("@Name", user.name),
                new OleDbParameter("@Phone", user.phone),
                new OleDbParameter("@Password", user.password),
                new OleDbParameter("@Role", user.role)
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    public async Task<List<User>> GetUsersByRoleFromAPI(string role)
    {
        string lastTimeUpdateJson = await _userDAL.GetLastTimeUpdateFromDB();
        var lastTimeUpdateResponse = JsonConvert.DeserializeObject<LastTimeUpdateResponse>(lastTimeUpdateJson);
        DateTime serverLastUpdateTime;
        DateTime.TryParseExact(lastTimeUpdateResponse.data[0].lastTimeUpdateUser, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out serverLastUpdateTime);

        string usersJson = await _userDAL.GetUsersByRole(role);
        // Save users and last update time to local file
        SaveLocalData(usersJson, serverLastUpdateTime);

        var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);
        return userResponse.Data;
    }

    //private DateTime? GetLocalLastTimeUpdate()
    //{
    //    string query = "SELECT * FROM Setting";
    //    DataTable dataTable = DataProvider.GetDataTable(query, null);

    //    if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0]["lastTimeUpdateUser"] != DBNull.Value)
    //    {
    //        return Convert.ToDateTime(dataTable.Rows[0]["lastTimeUpdateUser"]);
    //    }

    //    return null;
    //}

    private void SaveLocalData(string usersJson, DateTime lastUpdateTime)
    {
        var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);

        foreach (var user in userResponse.Data)
        {
            string query = "INSERT INTO `users` (`id`, `email`, `name`, `phone`, `password`, `role`) VALUES (@userId, @Email, @Name, @Phone, @Password, @Role, @LastUpdate)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@userId", user.user_id),
                new OleDbParameter("@Email", user.email),
                new OleDbParameter("@Name", user.name),
                new OleDbParameter("@Phone", user.phone),
                new OleDbParameter("@Password", user.password),
                new OleDbParameter("@Role", user.role),
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    private string LoadLocalData()
    {
        try
        {
            string query = "SELECT Users.[id], Users.[email], Users.[phone], Users.[password], Users.[role], Users.[name], Users.[last_update] FROM Users where   Users.[role] not like 'BT'";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                Console.WriteLine("No data found in the users table.");
                return null;
            }

            List<User> users = new List<User>();
            foreach (DataRow row in dataTable.Rows)
            {
                User user = new User
                {
                    user_id = int.Parse(row["id"].ToString()),
                    email = row["email"].ToString(),
                    name = row["name"].ToString(),
                    phone = row["phone"].ToString(),
                    password = row["password"].ToString(),
                    role = row["role"].ToString()
                };
                users.Add(user);
            }

            UserResponse userResponse = new UserResponse { Data = users };
            return JsonConvert.SerializeObject(userResponse);
        }
        catch (OleDbException ex)
        {
            Console.WriteLine("OleDbException: " + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }

}
