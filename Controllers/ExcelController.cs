using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;

public class ExcelController
{
    private readonly LocalDataHandler _localDataHandler;
    private readonly ClassSessionBLL _classSessionBLL;
    private readonly StudentBLL _studentBLL;
    private readonly SubjectBLL _subjectBLL;
    private readonly ClassBLL _classBLL;
    private readonly UserBLL _userBLL;
    private readonly ClassStudentBLL _classStudentBLL;

    public ExcelController(LocalDataHandler localDataHandler,ClassStudentBLL classStudentBLL, ClassSessionBLL classSessionBLL, StudentBLL studentBLL, SubjectBLL subjectBLL, ClassBLL classBLL, UserBLL userBLL)
    {
        _localDataHandler = localDataHandler;
        _classSessionBLL = classSessionBLL;
        _studentBLL = studentBLL;
        _subjectBLL = subjectBLL;
        _classBLL = classBLL;
        _userBLL = userBLL;
        _classStudentBLL = classStudentBLL;
    }

    public async Task<bool> AddDataFromExcel(ExcelData excelData)
    {
        try
        {
            // Add Teacher
            //var teacher = new User { name = excelData.TeacherName, role = "GV" };
            //var addedTeacher = await _userBLL.in(teacher);

            // Add Subject
            var subject = new Subject { name = excelData.SubjectName };
            var addedSubject = await _subjectBLL.InsertSubject(subject);

            // Add Class
            var classEntity = new Class { ClassName = excelData.ClassName };
            var addedClass = await _classBLL.InsertClass(classEntity);

            // Add Students
            var lstStudent= await _studentBLL.InsertStudent(excelData.Students);
            // Add ClassStudent
            List<ClassStudent> students = new List<ClassStudent>();
            foreach (var student in excelData.Students) {
                var classStudent = new ClassStudent { ClassID = int.Parse(addedClass.ClassID),
                StudentID=student.StudentID};
                students.Add(classStudent);
            }
            await _classStudentBLL.InsertClassStudent(students);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding data from Excel: " + ex.Message);
            SaveLocalExcelData(excelData);
            return false;
        }
    }

    private void SaveLocalExcelData(ExcelData excelData)
    {
        string localExcelDataFilePath = "localExcelData.json";

        List<ExcelData> localData;
        if (File.Exists(localExcelDataFilePath))
        {
            var jsonData = File.ReadAllText(localExcelDataFilePath);
            localData = JsonConvert.DeserializeObject<List<ExcelData>>(jsonData) ?? new List<ExcelData>();
        }
        else
        {
            localData = new List<ExcelData>();
        }

        localData.Add(excelData);

        File.WriteAllText(localExcelDataFilePath, JsonConvert.SerializeObject(localData));
    }

    public List<ExcelData> LoadLocalExcelData()
    {
        string localExcelDataFilePath = "localExcelData.json";

        if (File.Exists(localExcelDataFilePath))
        {
            var localData = File.ReadAllText(localExcelDataFilePath);
            return JsonConvert.DeserializeObject<List<ExcelData>>(localData) ?? new List<ExcelData>();
        }

        return new List<ExcelData>();
    }

    public void DeleteLocalExcelData()
    {
        string localExcelDataFilePath = "localExcelData.json";

        if (File.Exists(localExcelDataFilePath))
        {
            File.Delete(localExcelDataFilePath);
        }
    }

    public async Task ProcessLocalData()
    {
        var localData = LoadLocalExcelData();

        foreach (var excelData in localData)
        {
            var success = await AddDataFromExcel(excelData);
            if (success)
            {
                Console.WriteLine("Successfully added local data to the database.");
            }
            else
            {
                Console.WriteLine("Failed to add local data to the database.");
            }
        }

        // If all local data is successfully added, delete the local file
        DeleteLocalExcelData();
    }
}
