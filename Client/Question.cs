using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace testUdpTcp
{
    public class Question
    {
        public string Type { get; set; }

        [JsonProperty("question")]
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public object Answer { get; set; }
        public List<Pair> Pairs { get; set; }
    }
}
