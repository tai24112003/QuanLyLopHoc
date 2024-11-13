using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Result
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }

        private void Initialize(int id) {
            Id = id;
            Content = $"Đáp án {Id+1}";
            IsCorrect = false;
        }
        public Result() {
            Initialize(0);
        }

        public Result(int id) {
            Initialize(id);
        }
        public Result(string rsString) {
            string input = rsString;
            string[] result = input.Split(new string[] {"rs-"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in result)
            {
                string[] keyValue = item.Split(new string[] { "rs:" }, StringSplitOptions.RemoveEmptyEntries);
                string key= keyValue[0];
                string value = keyValue[1];

                switch (key)
                {
                    case "resultId":
                        Id=int.TryParse(value, out int typeId)?typeId:0;
                        break;
                    case "content":
                        Content=value;
                        break;
                    case "is":
                        IsCorrect= bool.TryParse(value, out bool typeBool) && typeBool;
                        break;
                }
            }
        }
        public string GetResultString() {
            string rs = "";
            rs += $"result@rs-resultIdrs:{Id}rs-contentrs:{Content}rs-isrs:{IsCorrect}";

            return rs;
        }
    }
}
