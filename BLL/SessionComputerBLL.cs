using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class SessionComputerBLL
{
    private readonly SessionComputerDAL _sessionComputerDAL;

    public SessionComputerBLL(SessionComputerDAL sessionComputerDAL)
    {
        _sessionComputerDAL = sessionComputerDAL ?? throw new ArgumentNullException(nameof(sessionComputerDAL));
    }

    public async Task<List<SessionComputer>> InsertSessionComputer(int sessionId, List<SessionComputer> sessionComputers)
    {
        try
        {
           string jsonSessionComputer= await _sessionComputerDAL.InsertSessionComputer(sessionComputers);
            SessionComputerResponse ClassResponse = JsonConvert.DeserializeObject<SessionComputerResponse>(jsonSessionComputer);
            return ClassResponse.data;


        }
        catch (Exception ex)
        {

            Console.WriteLine("Error inserting session computer on server: " + ex.Message);

            List<SessionComputer> missingSessionComputers = await _sessionComputerDAL.InsertSessionComputerLocal(sessionId, sessionComputers);

            return missingSessionComputers;
        }
    }
    

    public async Task<List<SessionComputer>> GetSessionComputersBySessionID(int sessionID)
    {
        // Gọi hàm DAL để lấy thông tin máy tính theo SessionID
        List<SessionComputer> computersJson = await _sessionComputerDAL.GetSessionComputersBySessionID(sessionID);
        if (computersJson == null)
        {
            return null;
        }

        return computersJson; // Trả về dữ liệu máy tính dưới dạng JSON
    }

    public async Task<List<SessionComputer>> GetSessionComputersBySessionIDNegative()
    {
        // Gọi hàm DAL để lấy thông tin máy tính theo SessionID
        List<SessionComputer> computersJson = await _sessionComputerDAL.GetSessionComputersBySessionIDNegative();
        if (computersJson == null)
        {
            return null;
        }

        return computersJson; // Trả về dữ liệu máy tính dưới dạng JSON
    }



    public async Task<bool> DeleteSessionComputersBySessionID(int sessionID)
    {
        // Gọi hàm DAL để xóa máy tính theo SessionID
        return await _sessionComputerDAL.DeleteSessionComputersBySessionID(sessionID);
    }


}
