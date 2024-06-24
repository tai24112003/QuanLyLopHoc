using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class UserBLL
{
    private readonly UserDAL _userDAL;

    public UserBLL(UserDAL userDAL)
    {
        _userDAL = userDAL ?? throw new ArgumentNullException(nameof(userDAL));
    }


    public DateTime GetLastTimeUpdate(string type)
    {
        // Implement logic to get last time update from local storage
        string filePath = $"{type}_LastTimeUpdate.txt";
        if (File.Exists(filePath))
        {
            string lastTimeUpdateStr = File.ReadAllText(filePath);
            return DateTime.Parse(lastTimeUpdateStr);
        }
        return DateTime.MinValue;
    }

    public void SaveLastTimeUpdate(string type, DateTime lastTimeUpdate)
    {
        // Implement logic to save last time update to local storage
        string filePath = $"{type}_LastTimeUpdate.txt";
        File.WriteAllText(filePath, lastTimeUpdate.ToString());
    }

    public List<User> LoadLocalUsers()
    {
        // Implement logic to load users from local storage
        string filePath = "Users.json";
        if (File.Exists(filePath))
        {
            string usersJson = File.ReadAllText(filePath);
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);
            return userResponse.Data;
        }
        return new List<User>();
    }

    public void SaveLocalUsers(List<User> users)
    {
        // Implement logic to save users to local storage
        string filePath = "Users.json";
        string usersJson = JsonConvert.SerializeObject(users);
        File.WriteAllText(filePath, usersJson);
    }

    public async Task<List<User>> GetUsersListByRole(string role)
    {
        try
        {
            DateTime lastTimeUpdateLocal = GetLastTimeUpdate("User");
            DateTime lastTimeUpdateDb = await GetLastTimeUpdateFromDB("User");

            if (lastTimeUpdateLocal >= lastTimeUpdateDb)
            {
                // Load local data
                return LoadLocalUsers();
            }
            else
            {
                // Fetch from API
                string usersJson = await _userDAL.GetUsersByRole(role);
                var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);

                // Save local data and update last time update
                SaveLocalUsers(userResponse.Data);
                SaveLastTimeUpdate("User", lastTimeUpdateDb);

                return userResponse.Data;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching users by role from BLL", ex);
        }
    }

    private async Task<DateTime> GetLastTimeUpdateFromDB(string type)
    {

        try
        {

            DateTime lastTimeUpdate = await _userDAL.GetLastTimeUpdateFromDB(type);
            return lastTimeUpdate;

            // For now, returning a sample DateTime.
            //return DateTime.Now; // Replace with actual database query
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting last time update from DB", ex);
        }
    }
}
