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

            if (ComputerResponse != null && ComputerResponse.Status == "success")
            {
                return ComputerResponse.data; // Return the list of Computer objects
            }
            else
            {
                // Handle cases when there's no data or status is not success
                return new List<Computer>();
            }
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
                SaveLocalData(ComputersJson); // Save data to local database if API call is successful
                return lstComputer;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching Computers by ID from BLL, loading local data: " + ex.Message);

            // Load data from local database when there's an error
            string ComputersJson = LoadLocalData();
            List<Computer> lstComputer = ParseComputers(ComputersJson);

            if (lstComputer.Count > 0)
            {
                return lstComputer;
            }
            throw new Exception("Error fetching Computers by ID from BLL and no local data available", ex);
        }
    }

    private void SaveLocalData(string ComputersJson)
    {
        var ComputerResponse = JsonConvert.DeserializeObject<ComputerResponse>(ComputersJson);

        if (ComputerResponse == null || ComputerResponse.data == null)
        {
            return;
        }

        foreach (var Computer in ComputerResponse.data)
        {
            string query = "INSERT INTO Computers (RoomID, ComputerName, RAM, HHD, CPU) VALUES (@RoomID, @ComputerName, @RAM, @HHD, @CPU)";

            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("@RoomID", Computer.RoomID),
                new OleDbParameter("@ComputerName", Computer.ComputerName),
                new OleDbParameter("@RAM", Computer.RAM),
                new OleDbParameter("@HHD", Computer.HHD),
                new OleDbParameter("@CPU", Computer.CPU),
            };

            DataProvider.RunNonQuery(query, parameters);
        }
    }

    private string LoadLocalData()
    {
        try
        {
            string query = "SELECT RoomID, ComputerName, RAM, HHD, CPU FROM Computers";
            DataTable dataTable = DataProvider.GetDataTable(query, null);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            List<Computer> Computers = new List<Computer>();
            foreach (DataRow row in dataTable.Rows)
            {
                Computer Computer = new Computer
                {
                    RoomID = row["RoomID"].ToString(),
                    ComputerName = row["ComputerName"].ToString(),
                    RAM = row["RAM"].ToString(),
                    HHD = row["HHD"].ToString(),
                    CPU = row["CPU"].ToString(),
                };
                Computers.Add(Computer);
            }

            ComputerResponse ComputerResponse = new ComputerResponse { Status = "success", data = Computers };
            return JsonConvert.SerializeObject(ComputerResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data from Access: " + ex.Message);
            return null;
        }
    }
}
