using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class UserBLL
{
    private readonly UserDAL _userDAL;
    private readonly string localFilePath = "localUser.json";

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
            // Check local file for last update time
            DateTime? localLastUpdateTime = GetLocalLastTimeUpdate();

            // Get last update time from server
            string lastTimeUpdateJson = await _userDAL.GetLastTimeUpdateFromDB();
            var lastTimeUpdateResponse = JsonConvert.DeserializeObject<LastTimeUpdateResponse>(lastTimeUpdateJson);
            DateTime serverLastUpdateTime;
             DateTime.TryParseExact(lastTimeUpdateResponse.data[0].lastTimeUpdateUser, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out serverLastUpdateTime);


            if (localLastUpdateTime.HasValue && localLastUpdateTime.Value >= serverLastUpdateTime)
            {
                // Load users from local file
                Console.WriteLine("load local");
                return LoadLocalData();
            }
            else
            {
                Console.WriteLine("load api");

                // Get users from server
                string usersJson = await _userDAL.GetUsersByRole(role);
                // Save users and last update time to local file
                SaveLocalData(usersJson, serverLastUpdateTime);
                return usersJson;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching users by role from BLL", ex);
        }
    }

    private DateTime? GetLocalLastTimeUpdate()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.LastTimeUpdateUser;
        }
        return null;
    }

    private void SaveLocalData(string usersJson, DateTime lastUpdateTime)
    {
        var localData = new LocalDataResponse
        {
            UsersJson = usersJson,
            LastTimeUpdateUser = lastUpdateTime
        };
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localData));
    }

    private string LoadLocalData()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.UsersJson;
        }
        return null;
    }
}

