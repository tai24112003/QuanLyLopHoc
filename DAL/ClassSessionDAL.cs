using Newtonsoft.Json;
using System;
using System.Data.OleDb;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class ClassSessionDAL
{
    private readonly IDataService _dataService;

    public ClassSessionDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> InsertClassSession(ClassSession classSession)
    {
        try
        {
            string classSessionJson = JsonConvert.SerializeObject(classSession);
            string responseJson = await _dataService.PostAsync("class_session/", classSessionJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting class session in DAL", ex);
        }
    }

    public async Task<string> getClassSessionByClassID(int ID)
    {
        try
        {
            string ClassStudentsJson = await _dataService.GetAsync("class_session/" + ID);
            return ClassStudentsJson;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void UpdateClassSessionWithNewClassID(int oldClassID, int newClassID)
    {
        string query = $"UPDATE Class_Session SET ClassID = @newClassID WHERE ClassID = @oldClassID";
        OleDbParameter[] parameters = new OleDbParameter[]
    {
        new OleDbParameter("@newClassID", newClassID),
        new OleDbParameter("@oldClassID", oldClassID),
    };

        DataProvider.RunNonQuery(query, parameters);
    }
    public void DeleteClassStudentsByClassID(int classID)
    {
        string query = $"DELETE FROM Class_Student WHERE ClassID = {classID}";

        OleDbParameter[] parameters = new OleDbParameter[]
        {
        new OleDbParameter("@ClassID", classID),
        };

        DataProvider.RunNonQuery(query, parameters);
    }

}

