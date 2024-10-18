using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class ClassSessionBLL
{
    private readonly ClassSessionDAL _classSessionDAL;

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
            Random random = new Random();
            int ssId=random.Next()*-1;
            classSession.SessionID= ssId;
            List<ClassSession> classSessions=new List<ClassSession>();
            classSessions.Add(classSession);
            // Save to local database if insertion fails
            _classSessionDAL.SaveLocalData(classSessions);

            throw new Exception("Error inserting class session in BLL. Data saved locally.", ex);
        }
    }

    
    public async Task<List<ClassSession>> GetClassSessionsByID(int id)
    {
        try
        {
            string ClassSessionsJson = await GetClassSessionsByID1(id);
            ClassSessionResponse ClassSessionResponse = JsonConvert.DeserializeObject<ClassSessionResponse>(ClassSessionsJson);
            return ClassSessionResponse.data;
        }
        catch (Exception ex)
        {
            string ClassSessionsJson = _classSessionDAL.LoadLocalData();
            if (!string.IsNullOrEmpty(ClassSessionsJson))
            {
                ClassSessionResponse ClassSessionResponse = JsonConvert.DeserializeObject<ClassSessionResponse>(ClassSessionsJson);
                return ClassSessionResponse.data.FindAll(e=>e.ClassID==id);
            }
            throw new Exception("Error fetching ClassSessions from API and local data", ex);
        }
    }
    public async Task<string> GetClassSessionsByID1(int id)
    {
        try
        {
            // Get ClassSessions from server
            string ClassSessionsJson = await _classSessionDAL.getClassSessionByClassID(id);
            // Save ClassSessions and last update time to local database
            ClassSessionResponse ClassSessionResponse = JsonConvert.DeserializeObject<ClassSessionResponse>(ClassSessionsJson);
            _classSessionDAL.SaveLocalData(ClassSessionResponse.data);


            return ClassSessionsJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassSessions from BLL", ex);
        }
    }

    public async Task UpdateClassSessionClassID(int oldClassID, int newClassID)
    {
        try
        {
            await _classSessionDAL.UpdateClassSessionWithNewClassID(oldClassID, newClassID);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
