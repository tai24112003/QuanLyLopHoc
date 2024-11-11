using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;

public class UserBLL
{
    private readonly UserDAL _userDAL;

    public UserBLL(UserDAL userDAL)
    {
        _userDAL = userDAL ?? throw new ArgumentNullException(nameof(userDAL));
    }

    public async Task<List<User>> GetUserAPI()
    {
        try
        {
            string usersJson = await _userDAL.GetUsersByCanTeacbAsync();
            await _userDAL.SaveLocalUserAccessDataAsync(usersJson);
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);
            return userResponse.Data;
        }
        catch (Exception ex)
        {
           
            Console.WriteLine("Error fetching user list by role from BLL", ex);
            return null;
        }
    }
    public async Task<List<User>> GetUserLocal()
    {
        try
        {
            string usersJson = await _userDAL.LoadLocalDataAsync();
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(usersJson);
            Console.WriteLine("Error fetching user list by role from BLL");
            return userResponse.Data;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine("Error fetching user list by role from BLL", ex);
            return null;
        }
    }


   
    public async Task<User> getUserByName(string name)
    {
        User lstUser = await _userDAL.GetUserByNameAsync(name);
        return lstUser;
    }


}
