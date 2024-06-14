using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Xml;

namespace testUdpTcp
{
    public partial class ExamForm : Form
    {
        private Quiz quiz;
        private bool canClosing = false;
        ThongBaoDiemForm baoDiem;
        public ExamForm(Quiz quiz)
        {
            InitializeComponent();
            int screenWidth = SystemInformation.VirtualScreen.Width;
            pnelContainExam.Width = screenWidth;
            this.quiz = quiz;
            InitalStateUI();
        }

        private void InitalStateUI()
        {   
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            pnelContainExam.Padding = new Padding(100); 

            int topPosition = 10;
            foreach (var question in quiz.Questions)
            {
                QuestionUC questionControl = new QuestionUC();
                questionControl.SetQuestion(question);

                questionControl.Location = new Point(CenterPanel(questionControl), topPosition);
                topPosition += questionControl.Height + 10;
                pnelContainExam.Controls.Add(questionControl);
            }
            QuestionUC questionC = new QuestionUC();
            Question a = new SingleChoiceQuestion();
            questionC.SetQuestion(a,true);

            questionC.Location = new Point(CenterPanel(questionC), topPosition);
            topPosition += questionC.Height + 10;
            questionC.PnContain.BorderStyle = BorderStyle.None;
            pnelContainExam.Controls.Add(questionC);
        }
        private int CenterPanel(QuestionUC a)
        {   
            int w = SystemInformation.VirtualScreen.Width; ;
            int wuc = a.Width;
            w = w / 2;
            return w - (wuc / 2);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {   
            baoDiem = new ThongBaoDiemForm(quiz.getScore());
            baoDiem.ShowDialog();
            string jsonstring = JsonConvert.SerializeObject(quiz.Questions, Newtonsoft.Json.Formatting.Indented);
            string filepath = @"d:\\demo1\\dataquanlylophoc\\123456_answer.json";
            canClosing = true;
            this.Close();
        }

        private void ExamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !canClosing;
        }
    }
}
