using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;

public class ExcelController
{
    private readonly ClassSessionBLL _classSessionBLL;
    private readonly StudentBLL _studentBLL;
    private readonly SubjectBLL _subjectBLL;
    private readonly ClassBLL _classBLL;
    private readonly UserBLL _userBLL;
    private readonly ClassStudentBLL _classStudentBLL;

    public ExcelController(LocalDataHandler localDataHandler, ClassStudentBLL classStudentBLL, ClassSessionBLL classSessionBLL, StudentBLL studentBLL, SubjectBLL subjectBLL, ClassBLL classBLL, UserBLL userBLL)
    {
        _classSessionBLL = classSessionBLL;
        _studentBLL = studentBLL;
        _subjectBLL = subjectBLL;
        _classBLL = classBLL;
        _userBLL = userBLL;
        _classStudentBLL = classStudentBLL;
    }

    public async Task<ExcelData> AddDataFromExcel(ExcelData excelData)
    {
        try
        {
            // Add Teacher

            var user = await _userBLL.getUserByName(excelData.TeacherName);
            DateTime currentDate = DateTime.Now;
            string formattedDateTime = currentDate.ToString("dd/MM/yyyy HH:mm:ss");

            // Add Class
            var classEntity = new Class { ClassName = excelData.ClassName, UserID=user.id,  LastTime = formattedDateTime };
            var addedClass = await _classBLL.InsertClass(classEntity);

            // Add Students
            // Add ClassStudent
            List<ClassStudent> students = new List<ClassStudent>();
            foreach (var student in excelData.Students)
            {
                student.LastTime = formattedDateTime;
                var classStudent = new ClassStudent
                {
                    ClassID = addedClass.ClassID,
                    StudentID = student.StudentID,
                    LastTime = formattedDateTime
                };
                students.Add(classStudent);
            }
            var lstStudent = await _studentBLL.InsertStudent(excelData.Students);

            await _classStudentBLL.InsertClassStudent(students);
            return excelData;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding data from Excel: " + ex.Message);
            return excelData;
        }
    }

  
}
