using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ComputerDAL
{
    private readonly IDataService _dataService;

    public ComputerDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> GetComputerByComputerID(int ComputerID)
    {
        try
        {
            string ComputerJson = await _dataService.GetAsync($"computer/{ComputerID}");
            return ComputerJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


}
