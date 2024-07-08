using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ClassSubjectBLL
{
    private readonly ClassSubjectDAL _ClassSubjectDAL;
    private readonly string localFilePath = "localClassSubjects.json";

    public ClassSubjectBLL(ClassSubjectDAL ClassSubjectDAL)
    {
        _ClassSubjectDAL = ClassSubjectDAL ?? throw new ArgumentNullException(nameof(ClassSubjectDAL));
    }
    public async Task<ClassSubject> InsertClassSubject(ClassSubject classSession)
    {
        try
        {
            string responseJson = await _ClassSubjectDAL.InsertClassSubject(classSession);
            var insertedSession = JsonConvert.DeserializeObject<ClassSubject>(responseJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting ClassSubject in BLL: " + ex.Message);
            throw new Exception("Error inserting ClassSubject in BLL", ex);
        }
    }
    public async Task<List<ClassSubject>> GetAllClassSubjects()
    {
        try
        {
            string ClassSubjectsJson = await GetClassSubjects();
            ClassSubjectResponse ClassSubjectResponse = JsonConvert.DeserializeObject<ClassSubjectResponse>(ClassSubjectsJson);
            return ClassSubjectResponse.data;
        }
        catch (Exception ex)
        {
            string ClassSubjectsJson = LoadLocalData();
            if (!string.IsNullOrEmpty(ClassSubjectsJson))
            {
                ClassSubjectResponse ClassSubjectResponse = JsonConvert.DeserializeObject<ClassSubjectResponse>(ClassSubjectsJson);
                return ClassSubjectResponse.data;
            }
            throw new Exception("Error fetching ClassSubjects from API and local data", ex);
        }
    }
    public async Task<List<ClassSubject>> GetClassSubjectsByClassID(int classID)
    {
        List<ClassSubject> allClassSubjects = await GetAllClassSubjects();
        return allClassSubjects.Where(cs => cs.ClassID == classID).ToList();
    }
    public async Task<string> GetClassSubjects()
    {
        try
        {


            if (File.Exists(localFilePath))
            {
                // Load ClassSubjects from local file
                Console.WriteLine("load local");
                return LoadLocalData();
            }
            else
            {
                Console.WriteLine("load api");

                // Get ClassSubjects from server
                string ClassSubjectsJson = await _ClassSubjectDAL.GetAllClassSubjects();
                // Save ClassSubjects and last update time to local file
                SaveLocalData(ClassSubjectsJson);
                return ClassSubjectsJson;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching ClassSubjects from BLL", ex);
        }
    }

    private void SaveLocalData(string ClassSubjectsJson)
    {
        var localData = new LocalDataResponse
        {
            ClassSubjectsJson = ClassSubjectsJson,
        };
        File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localData));
    }

    private string LoadLocalData()
    {
        if (File.Exists(localFilePath))
        {
            var localData = File.ReadAllText(localFilePath);
            var localResponse = JsonConvert.DeserializeObject<LocalDataResponse>(localData);
            return localResponse.ClassSubjectsJson;
        }
        return null;
    }
}
