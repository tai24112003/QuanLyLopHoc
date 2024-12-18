﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Newtonsoft.Json;

public class ClassStudentBLL
{
    private readonly ClassStudentDAL _ClassStudentDAL;
    private readonly StudentDAL _StudentDAL;

    public ClassStudentBLL(ClassStudentDAL ClassStudentDAL, StudentDAL StudentDAL)
    {
        _ClassStudentDAL = ClassStudentDAL ?? throw new ArgumentNullException(nameof(ClassStudentDAL));
        _StudentDAL = StudentDAL ?? throw new ArgumentNullException(nameof(StudentDAL));
    }

    public async Task<ClassStudent> InsertClassStudent(List<ClassStudent> classSession)
    {
        try
        {
            string responseJson = await _ClassStudentDAL.InsertClassStudent(classSession);
            var insertedSession = JsonConvert.DeserializeObject<ClassStudent>(responseJson);
            var classResponse = new ClassStudentResponse { data = classSession };
            string classJson = JsonConvert.SerializeObject(classResponse);
            _ClassStudentDAL.SaveLocalData(classJson);
            return insertedSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting ClassStudent in BLL: " + ex.Message);
            foreach (var _classSession in classSession)
            {
                if (!_classSession.StudentID.StartsWith("-"))
                    _classSession.StudentID = "-" + _classSession.StudentID;
            }
            // Save to local if insertion fails
            var classResponse = new ClassStudentResponse { data = classSession };
            string classJson = JsonConvert.SerializeObject(classResponse);
            _ClassStudentDAL.SaveLocalData(classJson);

            Console.WriteLine("Error inserting ClassStudent in BLL. Data saved locally.", ex);
            return null;
        }
    }

    public async Task<List<ClassStudent>>  GetAllClassStudents()
    {
        try
        {
            string ClassStudentsJson = _ClassStudentDAL.LoadLocalData();
            if (!string.IsNullOrEmpty(ClassStudentsJson))
            {
                ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
                return ClassStudentResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        { 
            
            Console.WriteLine("Error fetching ClassStudents from API and local data", ex);
            return null;
        }
    }

    public async Task<List<ClassStudent>> GetClassStudentsByClassID(int classID)
    {
        List<ClassStudent> allClassStudents = await GetAllClassStudents();
        return allClassStudents.Where(cs => cs.ClassID == classID).ToList();
    }
   
   

    public async Task<bool> DeleteLstClassStudentByStudentID(List<ClassStudent> classStudent)
    {
        try
        {
            // Get ClassStudents from server
            string ClassStudentsJson = await _ClassStudentDAL.DeleteClassStudentByStudentID(classStudent);
            if (!string.IsNullOrEmpty(ClassStudentsJson))
            {
                ClassStudentResponse ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
                return ClassStudentResponse.status== "success";
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching ClassStudents from BLL", ex);
            return false;
        }
    }




    public async Task<List<ClassStudent>> GetClassStudentsByID(int id)
    {
        try
        {
            ClassStudentResponse ClassStudentResponse =   _ClassStudentDAL.GetClassStudentsByClassID(id);
            if (ClassStudentResponse!=null)
            {
                return ClassStudentResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        {
            string ClassStudentsJson =  _ClassStudentDAL.LoadLocalData();
            var ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(ClassStudentsJson);
            Console.WriteLine("Error fetching ClassStudent list by role from BLL");
            return ClassStudentResponse.data;
        }
    }
    
    public async Task DeleteClassStudentsByClassID(int ClassID)
    {
        if (string.IsNullOrEmpty(ClassID.ToString()))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(ClassID));
        }

        try
        {
            // Call the DeleteStudentLocal method from the DAL
            await _ClassStudentDAL.DeleteClassStudentLocal(ClassID);
            Console.WriteLine($"Student with ID {ClassID} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student in BLL: {ex.Message}");
            Console.WriteLine($"Error deleting student with ID {ClassID} in BLL.", ex);
        }
    }

    public async Task DeleteClassStudentsByStudentID(string StudentID)
    {
        if (string.IsNullOrEmpty(StudentID))
        {
            throw new ArgumentException("StudentID cannot be null or empty.", nameof(StudentID));
        }

        try
        {
            // Call the DeleteStudentLocal method from the DAL
            await _ClassStudentDAL.DeleteClassStudentLocalByStudentID(StudentID);
            Console.WriteLine($"Student with ID {StudentID} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student in BLL: {ex.Message}");
            Console.WriteLine($"Error deleting student with ID {StudentID} in BLL.", ex);
        }
    }


    public async Task<List<ClassStudent>> GetClassStudentsByIDAPI(int id)
    {
        try
        {
            string jsoncalssStudent = await _ClassStudentDAL.GetClassStudentsByIDAPI(id);
            var ClassStudentResponse = JsonConvert.DeserializeObject<ClassStudentResponse>(jsoncalssStudent);
            _ClassStudentDAL.SaveLocalData(jsoncalssStudent);
            if (ClassStudentResponse != null)
            {
                return ClassStudentResponse.data;
            }
            return null;
        }
        catch (Exception ex)
        {
            ;
            Console.WriteLine("Error fetching ClassStudent list by role from BLL");
            return null;
        }
    }

}
