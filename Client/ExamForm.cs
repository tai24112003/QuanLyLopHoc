using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using fm = Newtonsoft.Json.Formatting;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using DAL.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace testUdpTcp
{
    public partial class ExamForm : Form
    {
        private bool canClosing = false;
        private bool flag = false;
        private int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        private int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        ThongBaoDiemForm baoDiem;


        private Test Test { get; set; }

        private Timer CountdownTimer { get; set; }
        private int Counter { get; set; }
        private readonly Action<StudentAnswer, int> SendAnswer;

        public ExamForm(Test test, Action<StudentAnswer, int> sendAnswer)
        {
            InitializeComponent();

            Test = test;
            SendAnswer = sendAnswer;

            InitForm();
            InitalStateUI();
        }
        private void InitForm()
        {
            //Test.Quests.Add(new Quest());
            //Test.Quests[1].Content = "Câu hỏi 2";

            int screenWidth = SystemInformation.VirtualScreen.Width;
            pnExam.Width = screenWidth;

            CountdownTimer = new Timer();
            CountdownTimer.Interval = 1000;
            CountdownTimer.Tick += CountdownTimer_Tick;

            Counter = 5;

            CountdownTimer.Start();
        }

        private void InitalStateUI()
        {
            //label8.Text = this.quiz.duration;
            //label4.Text = this.quiz.name;
            //label5.Text = this.quiz.subject.name;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            pnExam.Location = new Point(0, headerPanel.Height);
            pnExam.Width = (int)(screenWidth );
            pnExam.Height = (int)(screenHeight - headerPanel.Height);
            pnExam.AutoScroll = true;

            CenterLabelInPanel();
           // pnExam.Controls.Add(new QuestionInfoUC(new Quest()) { Dock=DockStyle.Fill });
        }

        private void CreateQuestUI()
        {
            foreach (Quest item in Test.Quests)
            {
                QuestionInfoUC newQ = new QuestionInfoUC(item, ShowNextQuest, SendAnswer)
                {
                    Dock = DockStyle.Fill,
                    Visible = false
                };
                pnExam.Controls.Add(newQ);
            }

            pnExam.Controls[0].Visible = true;
        }

        private void ShowNextQuest()
        {

            Control controlToRemove = pnExam.Controls[0];
            pnExam.Controls.RemoveAt(0);
            controlToRemove.Dispose();

            if (pnExam.Controls.Count > 1)
            {
                pnExam.Controls[0].Visible = true;
                return;
            }    
            
            // xử lý khi hết câu hỏi0

        }
        private void ExamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !canClosing;
        }

        private void ExamForm_Load(object sender, EventArgs e)
        {

        }

       

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (Counter > 0)
            {
                Counter--;
                lbl_time_to_start.Text = $"Bắt đầu làm bài sau {Counter}s";
            }
            else
            {
                CountdownTimer.Stop();
                pnExam.Controls.Clear();
                CreateQuestUI();
            }
        }
        private void CenterLabelInPanel()
        {
            lbl_time_to_start.Text = $"Bắt đầu làm bài sau {Counter}s";
            lbl_time_to_start.Location = new Point(
                (pnExam.ClientSize.Width - lbl_time_to_start.Width) / 2,
                (pnExam.ClientSize.Height - lbl_time_to_start.Height) / 2
            );
            lbl_time_to_start.Anchor = AnchorStyles.None;
        }
    }
}
