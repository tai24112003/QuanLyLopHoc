using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testUdpTcp
{
    public class Quiz
    {
        public List<Question> Questions { get; set; }
        public double getScore()
        {
            double rs = 0;
            foreach (var question in this.Questions)
            {
                rs += question.getScore();
            }
            double score = rs* 10 / this.Questions.Count;
            return score;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Quiz Questions:");
            foreach (var question in Questions)
            {
                sb.AppendLine(question.ToString());
            }
            return sb.ToString();
        }
    }

    
}
