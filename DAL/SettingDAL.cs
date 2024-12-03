using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class SettingDAL
{
    private readonly IDataService _dataService;

    public SettingDAL(IDataService dataService)
    {
        _dataService = dataService;
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
            Console.WriteLine("Last time update API endpoint not found.", ex);
            return null;
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine("Error fetching last time update from API.", ex);
            return null;
        }
    }
    public async Task<string> GetLastTimeUpdateFromLocal()
    {
        try
        {
            string query = "SELECT * FROM setting";
            DataTable dataTable = await Task.Run(() => DataProvider.GetDataTable(query, null));

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            // Chỉ lấy dòng đầu tiên của bảng setting
            DataRow row = dataTable.Rows[0];
            Setting setting = new Setting
            {
                lastTimeUpdateStudent = row["lastTimeUpdateStudent"].ToString(),
                lastTimeUpdateClass = row["lastTimeUpdateClass"].ToString(),
                lastTimeUpdateComputer = row["lastTimeUpdateComputer"].ToString()
            };

            // Chuyển đổi setting thành JSON
            string jsonResult = JsonConvert.SerializeObject(setting);
            return jsonResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from database: " + ex.Message);
            return null;
        }
    }
    public async Task<bool> UpdateLastTimeUpdateStudent(string newValue)
    {
        try
        {
            string query = "UPDATE setting SET lastTimeUpdateStudent = @newValue";
            
            OleDbParameter[] parameters = { new OleDbParameter("@newValue", newValue) };

            bool rowsAffected = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            return true; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating lastTimeUpdateStudent: " + ex.Message);
            return false;
        }
    }
    public async Task<bool> UpdateLastTimeUpdateClass(string newValue)
    {
        try
        {
            string query = "UPDATE setting SET lastTimeUpdateClass = @newValue";

            OleDbParameter[] parameters = { new OleDbParameter("@newValue", newValue) };

            bool rowsAffected = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            return true; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating lastTimeUpdateClass: " + ex.Message);
            return false;
        }
    }
    public async Task<bool> UpdateLastTimeUpdateComputer(string newValue)
    {
        try
        {
            string query = "UPDATE setting SET lastTimeUpdateComputer = @newValue";
            OleDbParameter[] parameters = { new OleDbParameter("@newValue", newValue) };


            bool rowsAffected = await Task.Run(() => DataProvider.RunNonQuery(query, parameters));

            return true; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating lastTimeUpdateComputer: " + ex.Message);
            return false;
        }
    }


}

