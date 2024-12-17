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
using Newtonsoft.Json.Linq;

namespace testUdpTcp
{
    public partial class ExamForm : Form
    {
        private bool canClose = false;
        private int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        private int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        ThongBaoDiemForm baoDiem;

        private Test Test { get; set; }

        private Timer CountdownTimer { get; set; }
        private int CounterA { get; set; }
        private  Action<string> SendData { get; set; }
        private  Action<string,string> ChangeInfoInClientForm { get; set; }
        private StudentAnswer Student { get; set; }
        private bool IsLate {  get; set; }
        public ExamForm(StudentAnswer student,Test test, Action<string> sendAnswer,Action<string, string> changeInfoStudent, bool state=false )
        {
            InitializeComponent();

            Test = test;
            SendData = sendAnswer;
            ChangeInfoInClientForm = changeInfoStudent;
            Student = student;
            IsLate = state;

            InitForm();
            InitalStateUI();
        }
        private void InitForm()
        {
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
            int sW = SystemInformation.VirtualScreen.Width;

            panel4.Size =new Size(sW/2 +20, headerPanel.Height);
            int pnl4W = panel4.Width;

            label4.Text = this.Test.Title;
            int l4 = pnl4W / 2 - label4.Width / 2;
            label4.Location = new Point(l4>0?l4:0,5);

            lbl_maxP.Text = $"Điểm tối đa: {Test.MaxPoint}";
            lbl_maxP.Location = new Point(pnl4W / 2 - lbl_maxP.Width / 2, label4.Bottom+5);

            lbl_mssv.Text ="MSSV: "+ this.Student.StudentID;
            lbl_mssv.Location = new Point(pnl4W / 2 - lbl_mssv.Width / 2, lbl_maxP.Bottom + 5);
            
            lbl_name.Text="Tên: "+this.Student.StudentName;
            lbl_name.Location = new Point(pnl4W / 2 - lbl_name.Width / 2, lbl_mssv.Bottom + 5);

            btn_changeMssv.Location = new Point(Math.Max(lbl_mssv.Right,lbl_name.Right) +10 , lbl_mssv.Bottom);

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

            int headerH=this.headerPanel.Height;
            foreach (Quest item in Test.Quests)
            {
                if (item.Index != indexQ)
                {
                    continue;
                }

                QuestionInfoUC newQ = new QuestionInfoUC(Student,item, SendAnswer, Test.IsShowResult, headerH)
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
            Console.WriteLine(Student.StudentName);

            string mess = $"answer@indexQuest:{indexQuest}{answer.GetAnswerString()}";
            Console.WriteLine(mess);
            SendData(mess);
        }
        public void ShowTop(List<string> top)
        {
            pnExam.Controls.Clear();
            pnExam.Controls.Add(lbl_top1);
            pnExam.Controls.Add(lbl_top2);
            pnExam.Controls.Add(lbl_top3);

            lbl_top1.Text = top[0];
            CenterLabelInPanel(lbl_top1, 1);

            lbl_top2.Text = top[1];
            CenterLabelInPanel(lbl_top2, 2);

            lbl_top3.Text = top[2];
            CenterLabelInPanel(lbl_top3, 3);


            lbl_top1.Visible = true;
            lbl_top2.Visible = true;
            lbl_top3.Visible = true;
        }
        public bool CheckStarted()
        {
            return CounterA > 0;
        }

        public void StartDoExam(bool islate=false) {
            btn_changeMssv.Visible = false;
            if (islate) {
                CounterA = 0;
            }else
            CountdownTimer.Start();
        }
        public async void NotiQuestCome(int indexQ)
        {
            while (CounterA > 0)
            {
                await Task.Delay(1500);
            }
            CreateQuestUI(indexQ);
        }
        public async void QuestDone(string mess)
        {
            pnExam.Controls.Clear();
            lbl_time_to_start.Text = mess;
            lbl_time_to_start.Visible = true;
            pnExam.Controls.Add(lbl_time_to_start);
            CenterLabelInPanel(lbl_time_to_start, 2);
            await Task.Delay(4000);
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
        private void ChangeMssvInForm(string newMSSV, string name)
        {
            string mess = $"ReadyAgain-{Student}-{newMSSV}";
            this.Student.StudentID = newMSSV;
            this.Student.StudentName = name;
            lbl_mssv.Text = newMSSV;
            lbl_name.Text = name;
            ChangeInfoInClientForm(newMSSV,name);
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
