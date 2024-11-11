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
    public partial class ResultInfo : UserControl
    {

        public Result Result { get; set; }
        private readonly Func<bool> CheckQuestType;
        public ResultInfo(Result result, Func<bool> checkQuestType)
        {
            InitializeComponent();
            this.Result = result;
            this.CheckQuestType = checkQuestType;

            Init();
        }

       
        private void Init()
        {
            lbl_answer_id.Text=$"{Result.Id+1}";
            txt_answer_content.Text=Result.Content;
            pnl_is_correct.BackColor = Result.IsCorrect ? Color.MediumSeaGreen : Color.RoyalBlue;
        }
        public void GetNewResultInfo()
        {
            Result.Content = txt_answer_content.Text;
        }

        private void setCorrect_Click(object sender, EventArgs e)
        {
            if (!Result.IsCorrect&& CheckQuestType())
            {
                    MessageBox.Show("Đã đủ đáp án đúng cho loại câu hỏi này rồi", "Thông báo", MessageBoxButtons.OK);
                    return;
            }
            Result.IsCorrect= !Result.IsCorrect;
            pnl_is_correct.BackColor= Result.IsCorrect ? Color.MediumSeaGreen: Color.RoyalBlue;
        }
    }
}
