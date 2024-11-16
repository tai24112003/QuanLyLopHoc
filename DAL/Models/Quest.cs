using System;
using System.Collections.Generic;
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
            string[] parts = questString.Split(new string[] {"q-" },StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                string[] keyValue = part.Split(new string[] { "q:" }, StringSplitOptions.RemoveEmptyEntries);
                string key = keyValue[0];
                string value = keyValue.Length > 0 ? keyValue[1] : string.Empty;

                switch (key)
                {
                    case "index":
                        Index=int.Parse(value);
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
                            .Select((item) => new Result(item))
                            .ToList(); // Assuming ParseResults parses and sets results list
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
            foreach (var item in Results) {
                rs += "result@";
                rs += item.GetResultString();
            }
            return rs;
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
    }

    public class StudentAnswer
    {
        public string StudentID{ get; set; }
        public List<int> SelectResultsId{ get; set; }
        public int TimeDoQuest { get; set; }

        private void Initialize(string studentId="",  int timeDoQuest=-1)
        {
            StudentID = studentId;
            SelectResultsId = new List<int>();
            TimeDoQuest = timeDoQuest;
        }
        public StudentAnswer()
        {
            Initialize();
        }
        public StudentAnswer(string studentId = "", int timeDoQuest = -1)
        {
            Initialize(studentId,  timeDoQuest);
        }
    
        public string GetAnswerString()
        {
            string rs = "";
            rs += $"-studentId:{StudentID}-timeDoQuest:{TimeDoQuest}-selectResultsId:{string.Join(",",SelectResultsId)}";
            return rs;
        }
    }

    public class StudentScore
    {
        public string StudentId { get; set; }
        public int Score { get; set; }
        public StudentScore()
        {
            StudentId = "";
            Score = 0;
        }

        public StudentScore(string studentCoreString)
        {
            string[] parts = studentCoreString.Split(new string[] { "sts-" }, StringSplitOptions.RemoveEmptyEntries);
            StudentId = parts[0].Split(new string[] { "sts:" },StringSplitOptions.RemoveEmptyEntries)[1];
            Score = int.Parse(parts[1].Split(new string[] { "sts:" }, StringSplitOptions.RemoveEmptyEntries)[1]);
        }

        public string GetString()
        {
            return $"sts-studentIdsts:{StudentId}sts-scorests:{Score}";
        }

        public string GetTopString(int top)
        {
            return $"top {top}: {StudentId} điểm: {Score}";
        }
    }
}
