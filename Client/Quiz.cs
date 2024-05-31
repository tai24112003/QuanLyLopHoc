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
