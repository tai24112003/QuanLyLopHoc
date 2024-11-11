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
        private void Initialize(int index, string contentSuffix = "")
        {
            Index = index;
            Content = $"Câu hỏi {Index + 1}{contentSuffix}";
            Type = QuestType.SingleSeclect;
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
        public Quest(int index)
        {
            Initialize(index);
        }

        // Hàm khởi tạo sao chép với đối tượng khác
        public Quest(Quest another)
        {
            Initialize(another.Index + 1, " new duplicate");
            Type = another.Type;
            CountDownTime = another.CountDownTime;
          //  NumOfResults = another.NumOfResults;
        }

        public Quest(string questString, int index)
        {
            Index=index;
            string[] parts = questString.Split('-').Skip(1).ToArray();
            foreach (var part in parts)
            {
                string[] keyValue = part.Split(':');
                string key = keyValue[0];
                string value = keyValue.Length > 0 ? keyValue[1] : string.Empty;

                switch (key)
                {
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
                        Results = Results = value.Split(new string[] { "result@" }, StringSplitOptions.None)
                            .Select((item) => new Result(item))
                            .ToList(); // Assuming ParseResults parses and sets results list
                        break;
                }
            }
        }

        public List<Result> GetResultsCorrect() =>  Results.Where(result=>result.IsCorrect).ToList();
        public bool CheckNumResults() => Results.Count==4||Results.Count==6;
       
        public bool CheckSingleType()
        {
            if (!GetResultsCorrect().Any()|| Type != QuestType.SingleSeclect)
                return false;
            return Type==QuestType.SingleSeclect&& GetResultsCorrect().Count==1;
        }

        public string GetQuestString()
        {
            string rs = "";
            rs += $"quest@-questContent:{Content}-type:{Type.Id}-time:{CountDownTime}-results:";
            foreach (var item in Results) {
                rs += item.GetResultString();
            }
            return rs;
        }

        public int GetNumStudentDo() =>  StudentAnswers.Count; 
    }

    public class StudentAnswer
    {
        public string StudentID{ get; set; }
        public int SelectResultID{ get; set; }
        public int TimeDoQuest { get; set; }

        private void Initialize(string studentId="", int selectResultId=-1, int timeDoQuest=-1)
        {
            StudentID = studentId;
            SelectResultID = selectResultId;
            TimeDoQuest = timeDoQuest;
        }
        public StudentAnswer()
        {
            Initialize();
        }
        public StudentAnswer(string studentId = "", int selectResultId = -1, int timeDoQuest = -1)
        {
            Initialize(studentId, selectResultId, timeDoQuest);
        }
        public StudentAnswer(string answerString)
        {
            Initialize();
            string[] parts = answerString.Split('-').Skip(1).ToArray();
            StudentID=parts[0].Split(':')[1];
            SelectResultID = Int32.Parse(parts[1].Split(':')[1]);
            TimeDoQuest = Int32.Parse(parts[2].Split(':')[1]);
        }

        public string GetAnswerString()
        {
            string rs = "";
            rs += $"-studentId:{StudentID}-selectResultID:{SelectResultID}-timeDoQuest:{TimeDoQuest}";
            return rs;
        }
    }
}
