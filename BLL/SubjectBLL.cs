using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SubjectBLL
{
    private readonly SubjectDAL _subjectDAL;
    private readonly string localFilePath = "localSubjects.json";

    public SubjectBLL(SubjectDAL subjectDAL)
    {
        _subjectDAL = subjectDAL ?? throw new ArgumentNullException(nameof(subjectDAL));
    }

    public async Task<List<Subject>> GetAllSubjects()
    {
        try
        {
            string subjectsJson = await GetSubjects();
            SubjectResponse subjectResponse = JsonConvert.DeserializeObject<SubjectResponse>(subjectsJson);
            return subjectResponse.data;
        }
        catch (Exception ex)
        {
            string subjectsJson = LoadLocalData();
            if (!string.IsNullOrEmpty(subjectsJson))
            {
                SubjectResponse subjectResponse = JsonConvert.DeserializeObject<SubjectResponse>(subjectsJson);
                return subjectResponse.data;
            }
            Console.WriteLine("Error fetching subjects from API and local data", ex);
            return null;
        }
    }
    public async Task<Subject> InsertSubject(Subject classSession)
    {
        try
        {
            string responseJson = await _subjectDAL.InsertSubject(classSession);
            var insertedSession = JsonConvert.DeserializeObject<SubjectResponse>(responseJson);
            return insertedSession.data[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting Subject in BLL: " + ex.Message);
            Console.WriteLine("Error inserting Subject in BLL", ex);
            return null;
        }
    }

    public async Task<string> GetSubjects()
    {

        Console.WriteLine("load api");

        // Get subjects from server
        string subjectsJson = await _subjectDAL.GetAllSubjects();
        // Save subjects and last update time to local file
        SaveLocalData(subjectsJson);
        return subjectsJson;



    }

    private DateTime? GetLocalLastTimeUpdate()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.LastTimeUpdateSubject;
        }
        return null;
    }

    private void SaveLocalData(string subjectsJson)
    {
        var localData = new LocalDataResponse
        {
            SubjectsJson = subjectsJson,
        };
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localData));
    }

    private string LoadLocalData()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.SubjectsJson;
        }
        return null;
    }
}
