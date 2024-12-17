﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Quest
    {
        public int Index { get; set; }
        public string Content { get; set; }
        public QuestType Type { get; set; }
        public int CountDownTime { get; set; }
        public byte[] ImageData { get; set; }
        public List<Result> Results { get; set; }
        public List<StudentAnswer> StudentAnswers { get; set; }
        private void Initialize(int index, int type=0, string contentSuffix = "")
        {
            Index = index;
            int indexT = contentSuffix != "" ? 0 : 1;
            Content = $"Câu hỏi {Index + indexT}{contentSuffix}";
            Type = QuestType.GetQuestType(type);
            CountDownTime = 20;


            Results = new List<Result>();
            for (int i = 0; i < 4; i++)
            {
                Results.Add(new Result(i));
            }

            StudentAnswers=new List<StudentAnswer>();
        }

        // Hàm khởi tạo mặc định
        public Quest()
        {
            Initialize(0);
        }

        // Hàm khởi tạo với tham số index
        public Quest(int index, int type=0)
        {
            Initialize(index, type);
        }

        // Hàm khởi tạo sao chép với đối tượng khác
        public Quest(Quest another, int index)
        {
            Initialize(index, 0," new duplicate");
            Type = another.Type;
            CountDownTime = another.CountDownTime;
            Results = new List<Result>(another.Results);
        }

        public Quest(string questString)
        {
            string[] parts = questString.Split(new string[] { "q-" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                string[] keyValue = part.Split(new string[] { "q:" }, StringSplitOptions.RemoveEmptyEntries);
                string key = keyValue[0];
                string value = keyValue.Length > 1 ? keyValue[1] : string.Empty;

                switch (key)
                {
                    case "index":
                        Index = int.TryParse(value, out int index) ? index : 0;
                        break;

                    case "questContent":
                        Content = value;
                        break;

                    case "type":
                        Type = QuestType.GetQuestType(int.TryParse(value, out int typeId) ? typeId : 0);
                        break;

                    case "time":
                        CountDownTime = int.TryParse(value, out int time) ? time : 0;
                        break;

                    case "results":
                        Results = value.Split(new string[] { "result@" }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(item => new Result(item))
                                       .ToList();
                        break;

                    case "image":
                        // Giải nén dữ liệu hình ảnh từ Base64
                        byte[] compressedImageData = Convert.FromBase64String(value);
                        ImageData = DecompressImage(compressedImageData);
                        break;
                }
            }
        }

        public List<Result> GetResultsCorrect() =>  Results.Where(result=>result.IsCorrect).ToList();
        public void DropCorrectResult()
        {
            foreach (Result item in Results)
            {
                item.IsCorrect = false;
            }
        }
        public string GetQuestString()
        {
            string rs = "";
            rs += $"q-indexq:{Index}q-questContentq:{Content}q-typeq:{Type.Id}q-timeq:{CountDownTime}q-resultsq:";

            // Chuyển đổi danh sách kết quả
            foreach (var item in Results)
            {
                rs += "result@";
                rs += item.GetResultString();
            }

            // Chuyển đổi hình ảnh (nếu có) thành chuỗi Base64
            if (ImageData != null && ImageData.Length > 0)
            {
                byte[] compressedImage = CompressImage(ImageData); // Nén dữ liệu hình ảnh
                string compressedImageBase64 = Convert.ToBase64String(compressedImage); // Chuyển sang Base64
                rs += $"q-imageq:{compressedImageBase64}"; // Thêm hình ảnh nén dưới dạng Base64
            }

            return rs;
        }
        private byte[] CompressImage(byte[] imageData)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(imageData, 0, imageData.Length);
                }
                return outputStream.ToArray();
            }
        }
        private byte[] DecompressImage(byte[] compressedImageData)
        {
            using (var inputStream = new MemoryStream(compressedImageData))
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
        }
        public int GetNumStudentDo() =>  StudentAnswers.Count;
        public List<StudentAnswer> GetStudentsAnsweredCorrectly() {
            List<Result> correctAnswer = this.GetResultsCorrect();

            return StudentAnswers.Where(item=> CheckCorrectAnswer(item, correctAnswer)).ToList();
        }
        private bool CheckCorrectAnswer(StudentAnswer answer, List<Result> correctAnswer)
        {
            if (answer.SelectResultsId.Count != correctAnswer.Count)
                return false;

            List<int> correctAnswersId = correctAnswer.Select(item => item.Id).ToList();

            answer.SelectResultsId.Sort();
            correctAnswersId.Sort();

            return answer.SelectResultsId.SequenceEqual(correctAnswersId);
        }
        public bool CheckCorrectAnswer(string studentId)
        {
            StudentAnswer student= StudentAnswers.Where(item => item.StudentID == studentId).FirstOrDefault();
            if (student==null)
                return false;

            List<int> correctAnswersId = this.GetResultsCorrect().Select(item => item.Id).ToList();

            student.SelectResultsId.Sort();
            correctAnswersId.Sort();

            return student.SelectResultsId.SequenceEqual(correctAnswersId);
        }
        public StudentAnswer GetFastestStudent()
        {
            return GetStudentsAnsweredCorrectly().OrderBy(student => student.TimeDoQuest).FirstOrDefault();
        }
        public int GetNumStudentSelectByResult(Result result)
        {
            int rs = 0;
            foreach (StudentAnswer item in StudentAnswers)
            {
                rs+=item.SelectResultsId.Contains(result.Id) ? 1 :0;
            }
            return rs;
        }

        public int CreateIndexResultInQuest()
        {
            int rs = 0;
            var sortedRs = Results.OrderBy(q => q.Id).ToList();
            foreach (Result q in sortedRs)
            {
                if (q.Id == rs)
                {
                    rs++; // Tăng rs nếu giá trị Index hiện tại đã được sử dụng
                    continue;
                }

                break; // Thoát khỏi vòng lặp nếu rs chưa được sử dụng
            }
            return rs; // Trả về giá trị rs nhỏ nhất chưa được sử dụng
        }
    }

    public class StudentAnswer
    {
        public string StudentID{ get; set; }
        public string StudentName{ get; set; }
        public List<int> SelectResultsId{ get; set; }
        public int TimeDoQuest { get; set; }

        private void Initialize(string studentId="",  int timeDoQuest=-1)
        {
            StudentID = studentId;
            StudentName="";
            SelectResultsId = new List<int>();
            TimeDoQuest = timeDoQuest;
        }
        public StudentAnswer()
        {
            Initialize();
        }
        public string GetAnswerString()
        {
            string rs = "";
            rs += $"-studentId:{StudentID}-studentName:{StudentName}-timeDoQuest:{TimeDoQuest}-selectResultsId:{string.Join(",",SelectResultsId)}";
            return rs;
        }
    }

    public class StudentScore
    {
        public int Top { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public double Score { get; set; }
        public int NumCorrect { get; set; }
        public StudentScore()
        {
            StudentId = "";
            StudentName = "";
            Score = 0.0;
            Top = 0;
            NumCorrect = 0;
        }

        public string GetTopNumCorrectString()
        {
            return $"top {Top}: {StudentId} - {StudentName} câu đúng: {NumCorrect}";
        }
        public string GetTopScoreString()
        {
            return $"top {Top}: {StudentId} điểm: {Score}";
        }
    }
}
