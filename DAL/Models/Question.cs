using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Question
    {
        public int id { get; set; }
        public int type { get; set; }
        public int idxList = 0;
        public int idxSub = 0;
        public int idx = 0;
        public string question { get; set; }
        public List<string> options { get; set; }
        public List<Question> questions { get; set; }
        public List<string> answer { get; set; }
    }
}
