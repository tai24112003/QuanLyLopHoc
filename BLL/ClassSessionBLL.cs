using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;

public class ClassSessionBLL
{
    private readonly ClassSessionDAL _classSessionDAL;
    private readonly string localClassSessionsFilePath = "localClassSessions.json";

    public ClassSessionBLL(ClassSessionDAL classSessionDAL)
    {
        _classSessionDAL = classSessionDAL ?? throw new ArgumentNullException(nameof(classSessionDAL));
    }

    public async Task<ClassSession> InsertClassSession(ClassSession classSession)
    {
        try
        {
            string responseJson = await _classSessionDAL.InsertClassSession(classSession);
            var insertedSession = JsonConvert.DeserializeObject<ClassSession>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting class session in BLL: " + ex.Message);
            throw new Exception("Error inserting class session in BLL", ex);
        }
    }

}
