using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class QuestType
    {
        
        public static readonly QuestType MultipleSelect = new QuestType(1, "Trắc nghiệm nhiều đáp án", "Chọn 1 hoặc nhiều đáp án chính xác trong các lựa chọn");
        public static readonly QuestType SingleSeclect = new QuestType(0,"Trắc nghiệm 1 đáp án", "Chọn 1 đáp án chính xác trong các lựa chọn");

        public int Id { get; set; }
        public string Name { get; set;}
        public string Description { get; set; }
        private QuestType(int id,string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public QuestType(string name)
        {
            Id = -1;
            Name = name;
            Description = name;
        }

        public static QuestType GetQuestType(int type) {
            List<QuestType> types =new List<QuestType> {QuestType.SingleSeclect, QuestType.MultipleSelect };
            return types[type];
        }
    }
}
