using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;


public class ExamBLL
{
    private readonly ExamDAL _ExamDAL;

    public ExamBLL(ExamDAL ExamDAL)
    {
        _ExamDAL = ExamDAL ?? throw new ArgumentNullException(nameof(ClassDAL));
    }

    public async Task<Exam> GetExam(int id)
    {
        try
        {

            string ExamJson = await _ExamDAL.getExam(id);
            var data = JsonConvert.DeserializeObject<ExamResponse>(ExamJson);
            return data.data;

        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching Exam from BLL", ex);
        }
    }

    public async Task<List<Exam>> GetListExam()
    {
        try
        {

            string ExamJson = await _ExamDAL.getListExam();
            var data = JsonConvert.DeserializeObject<ListExam>(ExamJson);
            return data.data;

        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching Exam from BLL", ex);
        }
    }

    public static void WriteToJsonFile(object obj, string filePath)
    {
        try
        {
            // Chuyển đổi đối tượng thành chuỗi JSON
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            // Ghi chuỗi JSON vào tập tin
            File.WriteAllText(filePath, json);

            Console.WriteLine($"Chuyển đổi và ghi file JSON thành công vào đường dẫn: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
        }
    }

}
