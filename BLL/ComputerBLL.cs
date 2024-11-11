using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class ComputerBLL
{
    private readonly ComputerDAL _ComputerDAL;

    public ComputerBLL(ComputerDAL ComputerDAL)
    {
        _ComputerDAL = ComputerDAL ?? throw new ArgumentNullException(nameof(ComputerDAL));
    }

    public List<Computer> ParseComputers(string json)
    {
        try
        {
            // Deserialize JSON to ComputerResponse object
            var ComputerResponse = JsonConvert.DeserializeObject<ComputerResponse>(json);
                return ComputerResponse.data; // Return the list of Computer objects
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error parsing JSON: " + ex.Message);
            return new List<Computer>();
        }
    }

    public async Task<List<Computer>> GetComputersByID(string id)
    {
        try
        {
            Console.WriteLine("Load API");

            // Get data from the server
            string ComputersJson = await _ComputerDAL.GetComputerByComputerID(id);
            List<Computer> lstComputer = ParseComputers(ComputersJson);

            if (lstComputer.Count > 0)
            {
                _ComputerDAL.SaveLocalData(ComputersJson); // Save data to local database if API call is successful
                return lstComputer;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Computers by ID from BLL, loading local data: " + ex.Message);

            // Load data from local database when there's an error
            string ComputersJson = _ComputerDAL.LoadLocalData();
            List<Computer> lstComputer = ParseComputers(ComputersJson);

            if (lstComputer.Count > 0)
            {
                return lstComputer;
            }
            Console.WriteLine("Error fetching Computers by ID from BLL and no local data available", ex);
            return null;
        }
    }

    

    public async Task<List<Computer>> GetComputerByDateRange(DateTime startTime, DateTime endTime)
    {
        try
        {
            string computersJson = await _ComputerDAL.GetComputersByDateRange(startTime, endTime);
            ComputerResponse computerResponse = JsonConvert.DeserializeObject<ComputerResponse>(computersJson);
            return computerResponse.data;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching computers by date range in BLL: " + ex.Message);
            Console.WriteLine("Error fetching computers by date range in BLL.", ex);
            return null;
        }
    }

    public async Task<bool> InsertOrUpdateComputers(List<Computer> computersToSync)
    {
        if (computersToSync == null || computersToSync.Count == 0)
        {
            throw new ArgumentException("List of computers to sync cannot be null or empty", nameof(computersToSync));
        }

        foreach (var computer in computersToSync)
        {
            bool exists = await _ComputerDAL.CheckIfComputerExists(computer.ID); // Hàm kiểm tra tồn tại

            if (exists)
            {
                // Cập nhật máy tính
                bool updated = await _ComputerDAL.UpdateComputer(computer);
                if (!updated)
                {
                    return false; // Nếu cập nhật thất bại
                }
            }
            else
            {
                //// Chèn máy tính mới
                //bool inserted = await _ComputerDAL.InsertComputer(computer);
                //if (!inserted)
                //{
                //    return false; // Nếu chèn thất bại
                //}
                Console.WriteLine("May moi");
            }
        }

        return true; // Trả về true nếu tất cả các máy tính đã được xử lý thành công
    }
    
}
