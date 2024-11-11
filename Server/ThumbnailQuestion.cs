using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class ThumbnailQuestion : UserControl
    {
        private readonly Action<ThumbnailQuestion> DoulicateQuestion;
        private readonly Action<ThumbnailQuestion> DeleteQuestion;
        private readonly Action<ThumbnailQuestion> SelectEventNoti;

        public Quest Quest { get; set; }

        private bool IsSelect=false;
        public ThumbnailQuestion(Action<ThumbnailQuestion> deleteQuestion, Action<ThumbnailQuestion> doulicateQuestion, Action<ThumbnailQuestion> selectEventNoti, Quest quest, bool state)
        {
            InitializeComponent();
            DeleteQuestion=deleteQuestion;
            DoulicateQuestion=doulicateQuestion;
            SelectEventNoti=selectEventNoti;
            Quest=quest;
            IsSelect=state;

            UpdateUI();
            UpdateState();

            ToolTip toolTip=new ToolTip();
            toolTip.SetToolTip(btn_des_ques, "Xóa câu hỏi");
            toolTip.SetToolTip(btn_dou_ques, "Nhân đôi câu hỏi");

            btn_des_ques.Click += (sender, e) => DeleteQuestion?.Invoke(this);
            btn_dou_ques.Click += (sender, e) => DoulicateQuestion?.Invoke(this);
        }

     
        private void ThumbnailQuestion_Click(object sender, EventArgs e)
        {
            IsSelect=true;
            UpdateState();
            SelectEventNoti?.Invoke(this);
        }

        public void SetNumOfTitle(int index)
        {
            Quest.Index = index;
            lbl_question.Text = $"{Quest.Index+1}. {Quest.Content}";
        }

        public void ChangeSelectState() {
            IsSelect=!IsSelect;
            UpdateState();
        }

        private void UpdateState()
        {
            this.BackColor = IsSelect ? Color.Gray : Color.White;
            this.BorderStyle = IsSelect ? BorderStyle.Fixed3D : BorderStyle.FixedSingle;
        }

        public void UpdateUI()
        {
            lbl_question.Text = $"{Quest.Index + 1}. {Quest.Type.Name}";
            lbl_num_answers.Text = $"Đáp án: {Quest.Results.Count.ToString()}";
            lbl_ques_time.Text = $"{Quest.CountDownTime.ToString()} s";
            txt_title_question.Text = Quest.Content;
        }
    }
}
