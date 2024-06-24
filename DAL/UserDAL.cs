using System;
using System.Threading.Tasks;

public class UserDAL
{
    private readonly IDataService _dataService;

    public UserDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetUsersByRole(string role)
    {
        try
        {
            string usersJson = await _dataService.GetAsync($"user/{role}");
            return usersJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
