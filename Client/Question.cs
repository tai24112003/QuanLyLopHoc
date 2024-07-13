using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace testUdpTcp
{
    public class Question
    {
        public int id { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        public int idxList=0;
        public int idxSub = 0;
        public int idx = 0;

        [JsonProperty("question")]
        public string QuestionText { get; set; }
        public List<string> options { get; set; }
        public List<Question> questions { get; set; }
        public List<string> answer { get; set; }
        public virtual void AddAnswer(string answer)
        {
            return;
        }
        public virtual void RemoveAnswer(string answer) { return; }


    }
    public class singleQuestion : Question
    {
        public List<string> _answer;
       
    }
    public class CommonQuestion : Question
    {
        public List<string> _answer;
    }
}
