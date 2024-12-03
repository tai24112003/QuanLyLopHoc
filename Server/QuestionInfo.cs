using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class QuestionInfo : UserControl
    {   
        public Quest Quest { set; get; }
        public QuestionInfo(Quest quest)
        {
            InitializeComponent();
            Quest = quest;

            Init();
        }

        private void Init()
        {
            txt_ques_typing.Text=Quest.Content;
            foreach (Result item in Quest.Results)
            {
                pnl_answers.Height = (Quest.Results.Count+1) / 2 * 96;
                pnl_answers.Controls.Add(new ResultInfo(item, CheckQuestType));
            }
        }

        public void GetNewQuestContent()
        {
            Quest.Content = txt_ques_typing.Text.Trim();
            GetNewResultContent();
        }

        private void GetNewResultContent()
        {
          //  Quest.NumOfResults = pnl_answers.Controls.Count;
            Quest.Results.Clear();

            foreach (ResultInfo item in pnl_answers.Controls) { 
                item.GetNewResultInfo();
                int newId = Quest.CreateIndexResultInQuest();
                item.Result.Id = newId;
                Quest.Results.Add(item.Result);
            }
            
        }

        private bool CheckQuestType()
        {
            if (Quest.Type == QuestType.SingleSeclect)
            {
                foreach (ResultInfo item in pnl_answers.Controls)
                {
                    if (item.GetState())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void btn_add_result_Click(object sender, EventArgs e)
        {
            int numResultContro = pnl_answers.Controls.Count;
            if (numResultContro == 6) { 
                return;
            }
            pnl_answers.Controls.Add(new ResultInfo(new Result(numResultContro), CheckQuestType));

            pnl_answers.Height = (numResultContro+2) / 2 * 96;

        }
        private void btn_del_result_Click(object sender, EventArgs e)
        {
            int numResultContro = pnl_answers.Controls.Count;
            if (numResultContro == 4)
            {
                return;
            }
            pnl_answers.Controls.RemoveAt(--numResultContro);

            pnl_answers.Height = (numResultContro+1) / 2 * 96;

        }
        private void txt_ques_typing_TextChanged(object sender, EventArgs e)
        {
            int fixedWidth = txt_ques_typing.Width;

            // Đo kích thước của văn bản với chiều rộng cố định và phông chữ hiện tại
            Size textSize = TextRenderer.MeasureText(txt_ques_typing.Text, txt_ques_typing.Font,
                                                      new Size(fixedWidth, int.MaxValue),
                                                      TextFormatFlags.WordBreak);

            // Đặt lại chiều cao của TextBox để vừa với nội dung
            txt_ques_typing.Height = Math.Min(textSize.Height + 20, 150);
        }

        
    }
}
