using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class SessionBLL
{
    private readonly SessionDAL _SessionDAL;

    public SessionBLL(SessionDAL SessionDAL)
    {
        _SessionDAL = SessionDAL ?? throw new ArgumentNullException(nameof(SessionDAL));
    }
    public async Task<List<Session>> GetAllSessions()
    {
        try
        {
            string SessionsJson = await _SessionDAL.LoadLocal();
            if (!string.IsNullOrEmpty(SessionsJson))
            {
                SessionResponse SessionResponse = JsonConvert.DeserializeObject<SessionResponse>(SessionsJson);
                return SessionResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine("Error fetching Sessions from API and local data", ex);
            return null;

        }
    }

    public async Task<List<Session>> GetAllSessionsAPI()
    {
        try
        {
            string SessionsJson = await _SessionDAL.GetSession();
            if (!string.IsNullOrEmpty(SessionsJson))
            {
                SessionResponse SessionResponse = JsonConvert.DeserializeObject<SessionResponse>(SessionsJson);
                await _SessionDAL.SaveLocal(SessionsJson);
                return SessionResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        {

            Console.WriteLine("Error fetching Sessions from API and local data", ex);
            return null;

        }
    }

}
