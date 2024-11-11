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
            string[] parts = testString.Split('-').Skip(1).ToArray();

            Title = parts[0]; // Lấy tiêu đề bài kiểm tra
            MaxPoint = int.Parse(parts[1]); // Lấy điểm tối đa
            // Tạo danh sách các câu hỏi (Quests) từ chuỗi
            Quests = parts[2].Split(new string[] { "quest@" }, StringSplitOptions.RemoveEmptyEntries)
                .Select((questString,index) =>new Quest(questString, index)).ToList();
        }
        public int GetTimeOfTest()
        {
            return Quests.Sum(quest => quest.CountDownTime);
        }
        public string GetTestString()
        {
            string rs = "";
            rs += $"-titleExam:{Title}-point:{MaxPoint}-quests:";
            foreach (Quest quest in Quests) {
                rs += quest.GetQuestString();
            }
            return rs;
        }

        public void ResetCountReady() => NumStudentsReady = 0;
       
    }
}
