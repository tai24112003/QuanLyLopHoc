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
using System.Threading.Tasks;

namespace testUdpTcp
{
    public partial class ExamForm : Form
    {
        private bool flag = false;
        private bool canClose = false;
        private int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        private int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        ThongBaoDiemForm baoDiem;


        private Test Test { get; set; }

        private Timer CountdownTimer { get; set; }
        private int CounterA { get; set; }
        private  Action<string> SendData { get; set; }
        private readonly Action<string> ChangeMssvInClientForm;
        private string Mssv { get; set; }
        private bool IsLate {  get; set; }
        public ExamForm(string mssv,Test test, Action<string> sendAnswer,Action<string> changeMssv, bool state=false )
        {
            InitializeComponent();

            Test = test;
            SendData = sendAnswer;
            ChangeMssvInClientForm = changeMssv;
            Mssv = mssv;
            IsLate = state;

            InitForm();
            InitalStateUI();
        }
        private void InitForm()
        {
            //Test.Quests.Add(new Quest());
            //Test.Quests[1].Content = "Câu hỏi 2";

            int screenWidth = SystemInformation.VirtualScreen.Width;
            pnExam.Width = screenWidth;

            CountdownTimer = new Timer
            {
                Interval = 1000
            };
            CountdownTimer.Tick += CountdownTimer_Tick;

            CounterA = IsLate?0:5;

        }

        private void InitalStateUI()
        {
            label4.Text = this.Test.Title;
            lbl_mssv.Text = this.Mssv;
            label8.Text = $"{this.Test.GetTimeOfTest()} giây"; 
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            pnExam.Location = new Point(0, headerPanel.Height);
            pnExam.Width = (int)(screenWidth );
            pnExam.Height = (int)(screenHeight - headerPanel.Height);
            pnExam.AutoScroll = true;

            lbl_top1.Visible = false;
            lbl_top2.Visible = false;
            lbl_top3.Visible = false;

            lbl_time_to_start.Text = $"Chờ tín hiệu làm bài";
            CenterLabelInPanel(lbl_time_to_start, 2);
        }

        private void CreateQuestUI(int indexQ)
        {
            pnExam.Controls.Clear();
            lbl_top1.Visible=false;
            lbl_top2.Visible = false;
            lbl_top3.Visible = false;

            foreach (Quest item in Test.Quests)
            {
                if (item.Index != indexQ)
                {
                    continue;
                }
                QuestionInfoUC newQ = new QuestionInfoUC(item, SendAnswer)
                {
                    Dock = DockStyle.Fill,
                };
                pnExam.Controls.Add(newQ);
                newQ.StartDo();
                break;
            }
        }
        private void SendAnswer(StudentAnswer answer, int indexQuest)
        {
            answer.StudentID = Mssv;
            string mess = $"answer@indexQuest:{indexQuest}{answer.GetAnswerString()}";
            Console.WriteLine(mess);
            SendData(mess);
        }
        public void ShowTop(List<string> top)
        {
            lbl_top1.Text = top[0];
            CenterLabelInPanel(lbl_top1, 1);

            lbl_top2.Text = top[1];
            CenterLabelInPanel(lbl_top2, 2);

            lbl_top3.Text = top[2];
            CenterLabelInPanel(lbl_top3, 3);

            pnExam.Controls.Clear();
            pnExam.Controls.Add(lbl_top1);
            pnExam.Controls.Add(lbl_top2);
            pnExam.Controls.Add(lbl_top3);

            lbl_top1.Visible = true;
            lbl_top2.Visible = true;
            lbl_top3.Visible = true;
        }
        public void StartDoExam() {
            btn_changeMssv.Visible = false;
            CountdownTimer.Start();
        }
        public void NotiQuestCome(int indexQ)
        {
            while (CounterA > 0)
            {

            }
            CreateQuestUI(indexQ);
        }
        public void QuestDone()
        {
            pnExam.Controls.Clear();
            MessageBox.Show("Bạn đã thi xong", "Thông tin");
            canClose = true;
            this.Dispose();
        }
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (CounterA > 0)
            {
                CounterA--;
                lbl_time_to_start.Text = $"Bắt đầu làm bài sau {CounterA}s";
            }
            else
            {
                CountdownTimer.Stop();
                lbl_time_to_start.Visible = false;
            }
        }
        private void CenterLabelInPanel(Label lbl, int top)
        {
            lbl.Location = new Point(
                (pnExam.ClientSize.Width - lbl.Width) / 2,
                (pnExam.ClientSize.Height - lbl.Height) *top/ 4
            );
            lbl.Anchor = AnchorStyles.None;
        }
        private void ChangeMssvInForm(string newMSSV)
        {
            string mess = $"ReadyAgain-{Mssv}-{newMSSV}";
            this.Mssv = newMSSV;
            lbl_mssv.Text = Mssv;
            ChangeMssvInClientForm(newMSSV);
            SendData(mess);
        }
        private void btn_changeMssv_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn muốn nhập lại MSSV?", "Thông tin", MessageBoxButtons.OKCancel);
            if (rs == DialogResult.Cancel) {
                return;            
            }
            (new Waiting(ChangeMssvInForm)).ShowDialog();
        }

        private void ExamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !canClose;
        }
    }
}
