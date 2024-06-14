using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace testUdpTcp
{
    public class Question
    {
        public string Type { get; set; }

        [JsonProperty("question")]
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public object Answer { get; set; }
        public virtual void AddAnswer(string answer)
        {
            return;
        }
        public virtual void RemoveAnswer(string answer) { return; }
        public virtual double getScore() { return 0d; }


    }
    public class SingleChoiceQuestion : Question
    {
        public string _answer;
        public override void AddAnswer(string answer)
        {
            Console.WriteLine("Run");
            this._answer = answer;
        }
        public override double getScore()
        {
            
            return (_answer == this.Answer.ToString()) ? 1 : 0;
        }
    }
    public class MultipleChoiceQuestion : Question
    {
        public List<string> _answer;
        public override void AddAnswer(string answer)
        {

            if (_answer == null)
            {
                _answer = new List<string>();
            }
            this._answer.Add(answer);
        }
        public override void RemoveAnswer(string answer)
        {
            this._answer.Remove(answer);
        }
        public override double getScore()
        {
            if (_answer == null) return 0;
            var jsonArray = (JArray)this.Answer;
            var stringList = jsonArray.ToObject<List<string>>();

            if (_answer.OrderBy(x => x).SequenceEqual(stringList.OrderBy(x => x)))
            {
                return 1;
            }
            return 0;
        }
    }

    public class OrderingQuestion : Question
    {
        public List<string> _answer;
        public override void AddAnswer(string answer)
        {
            if(_answer == null)
            {
                _answer = new List<string>();
            }
            this._answer.Add(answer);
        }
        public override void RemoveAnswer(string answer)
        {
            this._answer.Remove(answer);
        }
        public override double getScore()
        {
            if (_answer == null) return 0;

            var jsonArray = (JArray)this.Answer;
            var stringList = jsonArray.Select(x => (string)x).ToList();

            if (_answer.SequenceEqual(stringList))
            {
                return 1;
            }
            return 0;
        }
    }
}
