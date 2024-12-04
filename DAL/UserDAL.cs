using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Net.Http;
using System.Threading.Tasks;

public class UserDAL
{
    private readonly IDataService _dataService;

    public UserDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetUsersByCanTeacbAsync()
    {
        try
        {
            string usersJson = await _dataService.GetAsync("user/getUserCanTeach");
            return usersJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<string> GetLastTimeUpdateFromDBAsync()
    {
        try
        {
            string lastTimeUpdateJson = await _dataService.GetAsync("setting/getSetting");
            return lastTimeUpdateJson;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Last time update API endpoint not found.", ex);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching last time update from API.", ex);
            return null;
        }
    }

    public async Task SaveLocalUserAccessDataAsync(string usersJson)
    {
        try
        {
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);

            if (userResponse == null || userResponse.Data == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                foreach (var user in userResponse.Data)
                {
                    // Step 1: Check if the user already exists in the database
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE id = @userId";
                    OleDbParameter[] checkParameters = new OleDbParameter[]
                    {
                    new OleDbParameter("@userId", user.id)
                    };

                    // Use the RunScalar method to get the count of users with the same id
                    int existingUserCount = Convert.ToInt32(DataProvider.RunScalar(checkQuery, checkParameters));

                    if (existingUserCount > 0)
                    {
                        // Step 2: If the user exists, update their details
                        string updateQuery = "UPDATE users SET " +
                                             "email = @Email, name = @Name, phone = @Phone, password = @Password, role = @Role " +
                                             "WHERE id = @userId";

                        OleDbParameter[] updateParameters = new OleDbParameter[]
                        {
                        new OleDbParameter("@userId", user.id),
                        new OleDbParameter("@Email", user.email),
                        new OleDbParameter("@Name", user.name),
                        new OleDbParameter("@Phone", user.phone),
                        new OleDbParameter("@Password", user.password),
                        new OleDbParameter("@Role", user.role)
                        };

                        DataProvider.RunNonQuery(updateQuery, updateParameters);
                    }
                    else
                    {
                        // Step 3: If the user does not exist, insert a new record
                        string insertQuery = "INSERT INTO users (id, email, name, phone, password, role) " +
                                             "VALUES (@userId, @Email, @Name, @Phone, @Password, @Role)";

                        OleDbParameter[] insertParameters = new OleDbParameter[]
                        {
                        new OleDbParameter("@userId", user.id),
                        new OleDbParameter("@Email", user.email),
                        new OleDbParameter("@Name", user.name),
                        new OleDbParameter("@Phone", user.phone),
                        new OleDbParameter("@Password", user.password),
                        new OleDbParameter("@Role", user.role)
                        };

                        DataProvider.RunNonQuery(insertQuery, insertParameters);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving user access data: " + ex.Message);
        }
    }


    public async Task<string> LoadLocalDataAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                string query = "SELECT Users.[id], Users.[email], Users.[phone], Users.[password], Users.[role], Users.[name] FROM Users WHERE Users.[role] NOT LIKE 'BT'";
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
                        id = int.Parse(row["id"].ToString()),
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
        });
    }

    public async Task<User> GetUserByNameAsync(string name)
    {
        return await Task.Run(() =>
        {
            try
            {
                string query = "SELECT Users.[id], Users.[email], Users.[phone], Users.[password], Users.[role], Users.[name] FROM Users WHERE Users.[name] = @name";

                OleDbParameter[] parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@name", name)
                };

                DataTable dataTable = DataProvider.GetDataTable(query, parameters);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    Console.WriteLine("No data found in the users table.");
                    return null;
                }

                DataRow row = dataTable.Rows[0];
                User user = new User
                {
                    id = int.Parse(row["id"].ToString()),
                    email = row["email"].ToString(),
                    name = row["name"].ToString(),
                    phone = row["phone"].ToString(),
                    password = row["password"].ToString(),
                    role = row["role"].ToString()
                };

                return user;
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
        });
    }
}
