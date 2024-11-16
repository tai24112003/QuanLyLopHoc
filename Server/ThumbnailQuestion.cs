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
        private int Index { get; set; }
        private bool IsSelect=false;
        private bool IsView { get; set; }
        public ThumbnailQuestion(Action<ThumbnailQuestion> deleteQuestion, Action<ThumbnailQuestion> doulicateQuestion, Action<ThumbnailQuestion> selectEventNoti, Quest quest, bool state, int index=-1, bool isview=false)
        {
            InitializeComponent();
            DeleteQuestion=deleteQuestion;
            DoulicateQuestion=doulicateQuestion;
            SelectEventNoti=selectEventNoti;
            Quest=quest;
            Index=index==-1?quest.Index:(index);
            IsSelect=state;
            IsView=isview;

            UpdateUI();
            UpdateState();

            ToolTip toolTip=new ToolTip();
            toolTip.SetToolTip(btn_del_ques, "Xóa câu hỏi");
            toolTip.SetToolTip(btn_dou_ques, "Nhân đôi câu hỏi");

            btn_del_ques.Click += (sender, e) => DeleteQuestion?.Invoke(this);
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
            Index = index;
            lbl_question.Text = $"{Index+1}. {Quest.Type.Name}";
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
           if (IsView){
                btn_del_ques.Visible = false;
                btn_dou_ques.Visible = false;
            }
            lbl_question.Text = $"{Index + 1}. {Quest.Type.Name}";

            lbl_num_answers.Text = $"Đáp án: {Quest.Results.Count.ToString()}";
            lbl_ques_time.Text = $"{Quest.CountDownTime.ToString()} s";
            txt_title_question.Text = Quest.Content;
        }
    }
}
