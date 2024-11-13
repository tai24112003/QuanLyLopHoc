using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Test
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public List<Quest> Quests { get; set; }
        public double MaxPoint { get; set; }
        public int NumStudentsReady { get; set; }
        private void Initialize(int index)
        {
            Index = index;
            Title = $"Bài Kiểm Tra {Index + 1}";
            MaxPoint = 10;
            NumStudentsReady = 0;
            Quests = new List<Quest>
            {
                new Quest()
            };
        }
        public Test()
        {   
            Initialize(0);
        }

        public Test(int index)
        {
            Initialize(index);
        }

        public Test(string testString)
        {
            string[] parts = testString.Split(new string[] { "t-" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in parts)
            {
                string[] keyValue = item.Split(new string[] {"t:"},StringSplitOptions.RemoveEmptyEntries);
                string key = keyValue[0];
                string value = keyValue[1];

                switch (key)
                {
                    case "titleExam":
                        Title = value;
                        break;
                    case "point":
                        MaxPoint = int.TryParse(value, out int typeId) ? typeId : 0;
                        break;
                    case "quests":
                        Quests=value.Split(new string[] { "quest@" }, StringSplitOptions.RemoveEmptyEntries)
                            .Select((questString, index) => new Quest(questString, index)).ToList();
                        break;
                }
            }
        }
        public int GetTimeOfTest()
        {
            int restTime = (Quests.Count - 1) * 2;
            return Quests.Sum(quest => quest.CountDownTime)+restTime;
        }
        public string GetTestString()
        {
            string rs = "";
            rs += $"t-titleExamt:{Title}t-pointt:{MaxPoint}t-questst:";
            foreach (Quest quest in Quests) {
                rs += quest.GetQuestString();
            }
            return rs;
        }

        public void ResetCountReady() => NumStudentsReady = 0;
       
        public int GetNumStudentDo()
        {
            return Quests.OrderByDescending(item =>item.GetNumStudentDo()).FirstOrDefault()?.GetNumStudentDo()??0;
        }

        public int ScoringForStudent(string studentId)
        {
            int rs = 0;
            foreach (Quest item in Quests)
            {
                if(item.CheckCorrectAnswer(studentId))
                    rs++;
            }

            return rs;
        }
    }
}
