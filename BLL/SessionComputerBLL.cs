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

    public async Task InsertSessionComputer(int sessionId, List<SessionComputer> sessionComputers)
    {
        try
        {
            await _sessionComputerDAL.InsertSessionComputer(sessionComputers);
        }
        catch (Exception ex)
        {

            await _sessionComputerDAL.InsertSessionComputerLocal(sessionId,sessionComputers);
            Console.WriteLine("Error inserting session computer in BLL: " + ex.Message);
        }
    }

    
}
