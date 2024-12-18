﻿using System;
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
        public bool IsExamining {  get; set; }
        public int RestTimeBetweenQuests { get; set; }
        public int Progress { get; set; }
        private void Initialize(int index)
        {
            Index = index;
            Title = $"Bài Kiểm Tra {Index + 1}";
            MaxPoint = 10;
            IsExamining = false ;
            RestTimeBetweenQuests = 3;
            Progress = 0;

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
            IsExamining = false;
            Quests = new List<Quest>();

            string[] parts = testString.Split(new string[] { "t-" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in parts)
            {
                string[] keyValue = item.Split(new string[] {"t:"},StringSplitOptions.RemoveEmptyEntries);
                string key = keyValue[0];
                string value = keyValue[1];

                switch (key)
                {
                    case "index":
                        Index = int.TryParse(value, out int indexO) ? indexO : 0; 
                        break;
                    case "titleExam":
                        Title = value;
                        break;
                    case "point":
                        MaxPoint = int.TryParse(value, out int maxPointO) ? maxPointO : 0;
                        break;
                    case "restTimeBetweenQuests":
                        RestTimeBetweenQuests= int.TryParse(value, out int restTimeO) ? restTimeO : 0;
                        break;
                    case "quests":
                        Quests=value.Split(new string[] { "quest@" }, StringSplitOptions.RemoveEmptyEntries)
                            .Select((questString) => new Quest(questString)).ToList();
                        break;
                }
            }
        }
        public void ResetProgress()=>Progress = 0; 
        public int GetTimeOfTest()
        {
            int restTime = (Quests.Count - 1) * (RestTimeBetweenQuests+3);
            return Quests.Sum(quest => quest.CountDownTime)+restTime;
        }
        public string GetTestString()
        {
            string rs = "";
            rs += $"t-indext:{Index}t-titleExamt:{Title}t-pointt:{MaxPoint}t-restTimeBetweenQuests:{RestTimeBetweenQuests}t-questst:";
            foreach (Quest quest in Quests) {
                rs += "quest@";
                rs += quest.GetQuestString();
            }
            return rs;
        }
        public string GetTestStringOutOfQuest()
        {
            string rs = "";
            rs += $"t-indext:{Index}t-titleExamt:{Title}t-pointt:{MaxPoint}t-restTimeBetweenQuestst:{RestTimeBetweenQuests}";
            return rs;
        }
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
        public List<StudentScore> ScoringForClass( int top=0) {
            List<StudentScore> studentScores = new List<StudentScore>();
            HashSet<string> stds = new HashSet<string>();

            foreach (Quest q in Quests)
            {
                foreach (StudentAnswer sa in q.StudentAnswers)
                {
                    stds.Add(sa.StudentID);
                }
            }

            int numQ=Quests.Count;
            double oneScore= (double)MaxPoint /numQ;
            foreach (string studentId in stds) {
                int numC = ScoringForStudent(studentId);
                studentScores.Add(new StudentScore { StudentId = studentId, NumCorrect= numC, Score= oneScore*numC });   
            }
            studentScores = studentScores.OrderByDescending(s => s.NumCorrect)
                .Select((s, index) => {
                    s.Top = index + 1;  
                    return s;
                }).ToList();
          
            // Nếu top > 0, chỉ lấy số lượng phần tử tương ứng
            if (top > 0)
            {
                studentScores = studentScores.Take(top).ToList();
            }
            return studentScores;
        }
        public int CreateIndexQuestInTest()
        {
            int rs = 0;
            var sortedQuests = Quests.OrderBy(q => q.Index).ToList();
            foreach (Quest q in sortedQuests)
            {
                if (q.Index == rs)
                {
                    rs++; // Tăng rs nếu giá trị Index hiện tại đã được sử dụng
                    continue;
                }

                break; // Thoát khỏi vòng lặp nếu rs chưa được sử dụng
            }
            return rs; // Trả về giá trị rs nhỏ nhất chưa được sử dụng
        }
        public Quest GetFirstInvalidQuestion()
        {
            // Trả về câu hỏi đầu tiên không có ít nhất một câu trả lời đúng
            return Quests?.FirstOrDefault(q => q.Results == null || !q.Results.Any(r => r.IsCorrect));
        }

    }
}
