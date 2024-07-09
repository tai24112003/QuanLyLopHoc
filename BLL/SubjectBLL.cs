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
            throw new Exception("Error fetching subjects from API and local data", ex);
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
            throw new Exception("Error inserting Subject in BLL", ex);  
        }
    }

    public async Task<string> GetSubjects()
    {
        try
        {
            // Check local file for last update time
            DateTime? localLastUpdateTime = GetLocalLastTimeUpdate();

            // Get last update time from server
            string lastTimeUpdateJson = await _subjectDAL.GetLastTimeUpdateFromDB();
            var lastTimeUpdateResponse = JsonConvert.DeserializeObject<LastTimeUpdateResponse>(lastTimeUpdateJson);
            DateTime serverLastUpdateTime;
            DateTime.TryParseExact(lastTimeUpdateResponse.data[0].lastTimeUpdateSubject, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out serverLastUpdateTime);

            if (localLastUpdateTime.HasValue && localLastUpdateTime.Value >= serverLastUpdateTime)
            {
                // Load subjects from local file
                Console.WriteLine("load local");
                return LoadLocalData();
            }
            else
            {
                Console.WriteLine("load api");

                // Get subjects from server
                string subjectsJson = await _subjectDAL.GetAllSubjects();
                // Save subjects and last update time to local file
                SaveLocalData(subjectsJson, serverLastUpdateTime);
                return subjectsJson;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching subjects from BLL", ex);
        }
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

    private void SaveLocalData(string subjectsJson, DateTime lastUpdateTime)
    {
        var localData = new LocalDataResponse
        {
            SubjectsJson = subjectsJson,
            LastTimeUpdateSubject = lastUpdateTime
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
