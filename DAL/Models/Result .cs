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
            string[] result = input.Split('-').Skip(1).ToArray();
            Id= int.Parse(result[0].Split(':')[1]);
            Content = result[1].Split(':')[1];
            IsCorrect = bool.Parse(result[2].Split(':')[1]);
        }
        public string GetResultString() {
            string rs = "";
            rs += $"result@-resultId:{Id}-content:{Content}-is:{IsCorrect}";

            return rs;
        }
    }
}
