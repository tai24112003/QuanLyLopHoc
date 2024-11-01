using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SettingBLL
{
    private readonly SettingDAL _SettingDAL;

    public SettingBLL(SettingDAL SettingDAL)
    {
        _SettingDAL = SettingDAL ?? throw new ArgumentNullException(nameof(SettingDAL));
    }
    public async Task<Setting> GetSettingServer()
    {
        try
        {
            string ClassJson = await _SettingDAL.GetLastTimeUpdateFromDB();
           var settingResponse = JsonConvert.DeserializeObject<SettingResponse>(ClassJson);

            return settingResponse.data[0]; 
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Class from API and local data", ex);
            return null;
        }
    }

    public async Task<Setting> GetSettingLocal()
    {
        try
        {
            string ClassJson = await _SettingDAL.GetLastTimeUpdateFromLocal();
            Setting ClassResponse = JsonConvert.DeserializeObject<Setting>(ClassJson);
            return ClassResponse;
        }
        catch (Exception ex)
        {

            Console.WriteLine("Error fetching Class from API and local data", ex);
            return null;
        }
    }

    public async Task<bool> UpdateLastTimeUpdateStudent(string newValue)
    {
        try
        {
            return await _SettingDAL.UpdateLastTimeUpdateStudent(newValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating lastTimeUpdateStudent", ex);
            return false;
        }
    }
    public async Task<bool> UpdateLastTimeUpdateClass(string newValue)
    {
        try
        {
            return await _SettingDAL.UpdateLastTimeUpdateClass(newValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating lastTimeUpdateStudent", ex);
            return false;
        }
    }
    public async Task<bool> UpdateLastTimeUpdateComputer(string newValue)
    {
        try
        {
            return await _SettingDAL.UpdateLastTimeUpdateComputer(newValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating lastTimeUpdateComputer", ex);
            return false;
        }
    }

}

