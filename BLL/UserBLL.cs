using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserBLL
{
    private readonly UserDAL _userDAL;

    public UserBLL(UserDAL userDAL)
    {
        _userDAL = userDAL ?? throw new ArgumentNullException(nameof(userDAL));
    }

    public async Task<string> GetUsersByRole(string role)
    {
        try
        {
            string usersJson = await _userDAL.GetUsersByRole(role);
            return usersJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching users by role from BLL", ex);
        }
    }

    public async Task<List<User>> GetUsersListByRole(string role)
    {
        try
        {
            string usersJson = await _userDAL.GetUsersByRole(role);
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);
            return userResponse.Data;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching users by role from BLL", ex);
        }
    }
}
