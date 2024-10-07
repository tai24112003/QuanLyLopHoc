//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.OleDb;
//using System.Threading.Tasks;

//public class SessionBLL
//{
//    private readonly SessionDAL _SessionDAL;

//    public SessionBLL(SessionDAL SessionDAL)
//    {
//        _SessionDAL = SessionDAL ?? throw new ArgumentNullException(nameof(SessionDAL));
//    }
//    public async Task<List<Session>> GetAllSessions()
//    {
//        try
//        {
//            string SessionsJson = await GetSessions();
//            SessionResponse SessionResponse = JsonConvert.DeserializeObject<SessionResponse>(SessionsJson);
//            return SessionResponse.data;
//        }
//        catch (Exception ex)
//        {
//            string SessionsJson = _SessionDAL.LoadLocalData();
//            if (!string.IsNullOrEmpty(SessionsJson))
//            {
//                SessionResponse SessionResponse = JsonConvert.DeserializeObject<SessionResponse>(SessionsJson);
//                return SessionResponse.data;
//            }
//            throw new Exception("Error fetching Sessions from API and local data", ex);
//        }
//    }


//}
