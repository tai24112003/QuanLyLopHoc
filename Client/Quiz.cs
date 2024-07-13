using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace testUdpTcp
{
    public class Quiz
    {

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }

        public string duration { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public Subject subject { get; set; }
     
    }
    public class Subject
    {
        public string name { get; set; }
    }

}
