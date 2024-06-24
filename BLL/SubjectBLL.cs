using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class SubjectBLL
{
    private readonly SubjectDAL _subjectDAL;

    public SubjectBLL(SubjectDAL subjectDAL)
    {
        _subjectDAL = subjectDAL ?? throw new ArgumentNullException(nameof(subjectDAL));
    }

    public async Task<List<Subject>> GetAllSubjects()
    {
        try
        {
            string subjectsJson = await _subjectDAL.GetAllSubjects();
            SubjectResponse subjectResponse = JsonConvert.DeserializeObject<SubjectResponse>(subjectsJson);
            return subjectResponse.data;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching subjects from API", ex);
        }
    }
}