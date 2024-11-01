// AnswerDAL.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class SubmisstionDAL
{
    private readonly IDataService _dataService;

    public SubmisstionDAL(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> InsertAnswer(List<Submission> Answers)
    {
        try
        {
            string AnswersJson = JsonConvert.SerializeObject(Answers);
            string responseJson = await _dataService.PostAsync("submisstion/", AnswersJson);
            return responseJson;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting submisstion in DAL", ex);
            return null;
        }
    }

    public async Task<string> DeleteAnswerBySessionID(int sessionID)
    {
        try
        {
            string responseJson = await _dataService.DeleteAsync($"answer/deleteBySessionID/{sessionID}");
            return responseJson;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting session computers by session ID in DAL", ex);
            return null;
        }
    }

    //public async Task InsertAnswerLocal(int sessionId, List<Answer> Answers)
    //{
    //    int sessionID = sessionId < 0 ? sessionId : sessionId * -1;
    //    foreach (Answer Answer in Answers)
    //    {
    //        OleDbParameter[] parameters = new OleDbParameter[]
    //        {
    //        new OleDbParameter("@SessionID", sessionID),
    //        new OleDbParameter("@ComputerName", Answer.ComputerName),
    //        new OleDbParameter("@RAM", Answer.RAM),
    //        new OleDbParameter("@HHD", Answer.HDD),
    //        new OleDbParameter("@CPU", Answer.CPU),
    //        new OleDbParameter("@MouseConnected", Answer.MouseConnected),
    //        new OleDbParameter("@KeyboardConnected", Answer.KeyboardConnected),
    //        new OleDbParameter("@MonitorConnected", Answer.MonitorConnected),
    //        new OleDbParameter("@MismatchInfo", Answer.MismatchInfo),
    //        new OleDbParameter("@RepairNote", Answer.RepairNote),
    //        new OleDbParameter("@StudentID", Answer.StudentID),
    //        };

    //        string query = "INSERT INTO `answer` (`SessionID`, `ComputerID`, `RAM`, `HDD`, `CPU`, " +
    //                       "`MouseConnected`, `KeyboardConnected`, `MonitorConnected`, `MismatchInfo`, `RepairNote`, `StudentID`) " +
    //                       "VALUES (@SessionID, @ComputerID, @RAM, @HHD, @CPU, " +
    //                       "@MouseConnected, @KeyboardConnected, @MonitorConnected, @MismatchInfo, @RepairNote, @StudentID)";

    //        bool success = DataProvider.RunNonQuery(query, parameters);
    //        if (!success)
    //        {
    //            Console.WriteLine("Failed to insert session computer into database.");
    //        }
    //    }
    //}

    //public List<Answer> LoadAnswersLocal(int sessionId)
    //{
    //    List<Answer> Answers = new List<Answer>();
    //    string query = "SELECT * FROM `answer` WHERE `SessionID` = @SessionID";
    //    OleDbParameter parameter = new OleDbParameter("@SessionID", sessionId);

    //    DataTable dataTable = DataProvider.GetDataTable(query, new OleDbParameter[] { parameter });

    //    if (dataTable != null)
    //    {
    //        foreach (DataRow row in dataTable.Rows)
    //        {
    //            Answer Answer = new Answer
    //            {
    //                SessionID = (int)row["SessionID"],
    //                ComputerName = row["ComputerName"].ToString(),
    //                RAM = row["RAM"].ToString(),
    //                HDD = row["HHD"].ToString(),
    //                CPU = row["CPU"].ToString(),
    //                MouseConnected = Convert.ToBoolean(row["MouseConnected"]),
    //                KeyboardConnected = Convert.ToBoolean(row["KeyboardConnected"]),
    //                MonitorConnected = Convert.ToBoolean(row["MonitorConnected"]),
    //                MismatchInfo = row["MismatchInfo"].ToString(),
    //                RepairNote = row["RepairNote"].ToString(),
    //                StudentID = row["StudentID"].ToString()
    //            };
    //            Answers.Add(Answer);
    //        }
    //    }

    //    return Answers;
    //}
}
