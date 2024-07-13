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

namespace testUdpTcp
{
    public partial class ExamForm : Form
    {
        private Quiz quiz;
        private bool canClosing = false;
        private bool flag = false;
        private int currentIdxList = 0;
        private int currentIdxSubQuestion = 0;
        private int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        private int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        private DataTable table;
        ThongBaoDiemForm baoDiem;
        public ExamForm(Quiz quiz)
        {
            InitializeComponent();
            int screenWidth = SystemInformation.VirtualScreen.Width;
            pnExam.Width = screenWidth;
            this.quiz = quiz;
            InitalStateUI();
        }

        private void InitalStateUI()
        {
            label8.Text = this.quiz.duration;
            label4.Text = this.quiz.name;
            label5.Text = this.quiz.subject.name;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            pnExam.Location = new Point(0, headerPanel.Height);
            pnExam.Width = (int)(screenWidth * 0.75);
            pnExam.Height = (int)(screenHeight - headerPanel.Height);
            pnExam.AutoScroll = true;
            pnlAnsSheet.Width = (int)(screenWidth * 0.25);
            pnlAnsSheet.Location = new Point(0+ pnExam.Width, pnExam.Location.Y);
            pnlAnsSheet.Height = (int)(screenHeight - headerPanel.Height);
            dgvListQuestion.Width = pnlAnsSheet.Width;
            dgvListQuestion.Height = pnlAnsSheet.Height;
            
        }

        private int CenterPanel(Control a)
        {   
            int w = SystemInformation.VirtualScreen.Width; ;
            int wuc = a.Width;
            w = w / 2;
            return w - (wuc / 2);
        }

        private int CenterPanelCommon(CommonQuestionUC a)
        {
            int w = SystemInformation.VirtualScreen.Width; 
            int wuc = a.Width;
            w = w / 2;
            return w - (wuc / 2);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {   
            baoDiem = new ThongBaoDiemForm(0);
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

        private void ExamForm_Load(object sender, EventArgs e)
        {
            if (quiz.Questions.Count > 0)
            {
                Question question = quiz.Questions[currentIdxList];
                if (question.Type == QuestionType.singleQuestion)
                {
                    flag = true;
                    lblIndex.Text = (question.idx+1).ToString();
                    renderSingleQuestion(question);
                }
                else
                {
                    if (question.questions.Count - 1 != currentIdxSubQuestion)
                    {
                        flag = false;
                        lblIndex.Text = (question.questions[currentIdxSubQuestion].idx+1).ToString();
                        renderCommonQuestion(question);
                    }
                    else
                    {
                        lblIndex.Text = (question.questions[currentIdxSubQuestion].idx+1).ToString();
                        renderCommonQuestion(question);
                        flag = true;
                    }

                }

            }
            LoadData();

        }

        private void LoadData()
        {
            // Create a list of Person objects and fill it with data
            List<Question> formatData = new List<Question>{};


            table = new DataTable();
            table.Columns.Add("STT", typeof(int));
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Text", typeof(string));
            table.Columns.Add("Answer", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("idxList", typeof(int));
            table.Columns.Add("idxSub", typeof(int));

            foreach (Question question in quiz.Questions)
            {
                if (question.Type == QuestionType.singleQuestion)
                {
                    string ans = "";
                    foreach(string a in question.answer)
                    {
                        ans = ans+"-"+a[a.Length-1];
                    }
                    table.Rows.Add(question.idx+1,question.id,question.QuestionText,ans,question.Type.ToString(),question.idxList,question.idxSub);
                }
                else
                {
                    foreach (Question subQuestion in question.questions)
                    {
                        string ans = "";
                        foreach (string a in subQuestion.answer)
                        {
                            ans = ans + "-" + a[a.Length - 1];
                        }
                        table.Rows.Add(subQuestion.idx+1,subQuestion.id,subQuestion.QuestionText,ans,subQuestion.Type.ToString(),subQuestion.idxList,subQuestion.idxSub);
                    }
                }
            }

            // Set the DataSource of the DataGridView
            dgvListQuestion.DataSource = table;

            dgvListQuestion.Columns["ID"].Visible = false;
            dgvListQuestion.Columns["idxList"].Visible = false;
            dgvListQuestion.Columns["idxSub"].Visible = false;
            dgvListQuestion.Columns["Text"].Visible = false;
            dgvListQuestion.Columns["Type"].Visible = false;
        }

        private void renderCommonQuestion(Question question)
        {
            int idxAns = 0;
            char charAns = 'A';
            int vPadding = 20;

            WebBrowser webQuestion = new WebBrowser();
            webQuestion.DocumentText = question.QuestionText;
            pnExam.Controls.Add(webQuestion);
            webQuestion.Width = pnExam.Width - 2;
            webQuestion.Height = 200;
            webQuestion.Location = new Point(0, 0);

            Question childQuestion = question.questions[currentIdxSubQuestion];

            WebBrowser webChildQuestion = new WebBrowser();
            webChildQuestion.DocumentText = childQuestion.QuestionText;
            pnExam.Controls.Add(webChildQuestion);
            webChildQuestion.Width = pnExam.Width - 2;
            webChildQuestion.Height = 200;
            webChildQuestion.Location = new Point(0, webQuestion.Height + vPadding);
            int chkSize = 30;
            int lblW = 30;
            int brsAnW = (pnExam.Width / 2 - chkSize - 20);
            int brsAnH = (screenHeight - headerPanel.Height - webQuestion.Height * 2 - 4 * vPadding) / 2;
            int row = webChildQuestion.Bottom + vPadding;
            foreach (string choice in childQuestion.options)
            {
                Label checkBox = new Label();
                checkBox.Name = "ckb-" + childQuestion.idxList.ToString() + '-' + childQuestion.idxSub.ToString()+"-"+ charAns.ToString();
                if (childQuestion.answer.Contains(checkBox.Name))
                {
                    checkBox.Text = "X";
                }
                else
                    checkBox.Text = "";
                checkBox.Width = chkSize;
                checkBox.Height = chkSize;
                checkBox.Font = new Font("Arial", 16, FontStyle.Bold);
                checkBox.BorderStyle = BorderStyle.Fixed3D;
                checkBox.BackColor = Color.LightBlue;
                checkBox.Click += (s, e) =>
                {
                    if (checkBox.Text == "")
                    {
                        checkBox.Text = "X";
                        childQuestion.answer.Add(checkBox.Name);
                        updateGridView(childQuestion);
                    }
                    else
                    {
                        checkBox.Text = "";
                        childQuestion.answer.Remove(checkBox.Name);
                        updateGridView(childQuestion);

                    }
                };
                Label label = new Label();
                label.Text = charAns.ToString();
                label.Width = lblW;
                label.Font = new Font("Arial", 14, FontStyle.Bold);
                WebBrowser webAns = new WebBrowser();
                webAns.Width = brsAnW;
                webAns.DocumentText = choice;
                webAns.Height = brsAnH;
                if (idxAns % 2 == 0)
                {
                    checkBox.Location = new Point(0, row);
                    label.Location = new Point(0, checkBox.Bottom);
                    webAns.Location = new Point(checkBox.Width + 10, row);
                }
                else
                {
                    checkBox.Location = new Point((int)(pnExam.Width / 2), row);
                    label.Location = new Point((int)(pnExam.Width / 2), checkBox.Bottom);
                    webAns.Location = new Point((int)(pnExam.Width / 2) + checkBox.Width + 10, row);
                }
                pnExam.Controls.Add((checkBox));
                pnExam.Controls.Add((label));
                pnExam.Controls.Add((webAns));
                idxAns++;
                charAns++;
                if (idxAns % 2 == 0 && idxAns != 0) row += (brsAnH + vPadding);
            }
        }

        private void renderSingleQuestion(Question question)
        {
            //MessageBox.Show("run");
            WebBrowser webQuestion = new WebBrowser();
            webQuestion.DocumentText = question.QuestionText;
            pnExam.Controls.Add(webQuestion);
            webQuestion.Width = pnExam.Width - 2;
            webQuestion.Height = 350;
            webQuestion.Location = new Point(0, 0);
            int idxAns = 0;
            char charAns = 'A';
            int vPadding = 20;

            int chkSize = 30;
            int lblW = 30;
            int brsAnW = (pnExam.Width / 2 - chkSize - 20);
            int brsAnH = (screenHeight - headerPanel.Height - webQuestion.Height - 4 * vPadding) / 2;
            int row = webQuestion.Bottom + vPadding;
            foreach (string choice in question.options)
            {
                //MessageBox.Show(choice);
                Label checkBox = new Label();
                checkBox.Name = "ckb-"+question.idxList.ToString()+'-'+question.idxSub.ToString()+"-"+charAns.ToString();
                if (question.answer.Contains(checkBox.Name))
                {
                    checkBox.Text = "X";
                    question.answer.Add(checkBox.Name);
                    
                }
                else
                {
                    checkBox.Text = "";
                    question.answer.Remove(checkBox.Name);
                    
                }
                checkBox.Width = chkSize;
                checkBox.Height = chkSize;
                checkBox.Font = new Font("Arial", 16, FontStyle.Bold);
                checkBox.BorderStyle = BorderStyle.Fixed3D;
                checkBox.BackColor = Color.LightBlue;
                checkBox.Click += (s, e) =>
                {
                    if (checkBox.Text == "")
                    {
                        checkBox.Text = "X";
                        question.answer.Add(checkBox.Name);
                        updateGridView(question);
                    }
                    else
                    {
                        checkBox.Text = "";
                        question.answer.Remove(checkBox.Name);
                        updateGridView(question);
                    }
                };
                Label label = new Label();
                label.Text = charAns.ToString();
                label.Width = lblW;
                label.Font = new Font("Arial", 14, FontStyle.Bold);
                WebBrowser webAns = new WebBrowser();
                webAns.Width = brsAnW;
                webAns.DocumentText = choice;
                webAns.Height = brsAnH;
                if (idxAns % 2 == 0)
                {
                    checkBox.Location = new Point(0, row);
                    label.Location = new Point(0, checkBox.Bottom);
                    webAns.Location = new Point(checkBox.Width + 10, row);
                }
                else
                {
                    checkBox.Location = new Point((int)(pnExam.Width / 2), row);
                    label.Location = new Point((int)(pnExam.Width / 2), checkBox.Bottom);
                    webAns.Location = new Point((int)(pnExam.Width / 2) + checkBox.Width + 10, row);
                }
                pnExam.Controls.Add((checkBox));
                pnExam.Controls.Add((label));
                pnExam.Controls.Add((webAns));
                idxAns++;
                charAns++;
                if (idxAns % 2 == 0 && idxAns != 0) row += (brsAnH + vPadding);
            }
        }

        private void updateGridView(Question newQ)
        {
            string ans = "";
            foreach (string a in newQ.answer)
            {
                ans = ans + "" + a[a.Length - 1];
            }
            dgvListQuestion.Rows[newQ.idx].Cells["Answer"].Value = ans;

            dgvListQuestion.ClearSelection(); // Xóa lựa chọn hiện tại
            dgvListQuestion.Rows[newQ.idx].Selected = true;

            // Focus vào dòng được chọn
            dgvListQuestion.CurrentCell = dgvListQuestion.Rows[newQ.idx].Cells[0]; // Chọn ô đầu tiên của dòng

            // Cuộn đến dòng được chọn
            dgvListQuestion.FirstDisplayedScrollingRowIndex = newQ.idx;
            DataTable dataTable = (DataTable)dgvListQuestion.DataSource;

           
            string filePath = @"D:\file.json"; 

            
            ExportDataGridViewToJson(dataTable, filePath);
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int limit = quiz.Questions.Count;
            
            if(currentIdxList == limit - 1)
            {
                return;
            }

            if (flag)
            {
                currentIdxList += 1;
                currentIdxSubQuestion = 0;
            }
            else currentIdxSubQuestion += 1;

            pnExam.Controls.Clear();
            Question question = quiz.Questions[currentIdxList];

            
            if (question.Type == QuestionType.singleQuestion)
            {
                lblIndex.Text = (question.idx+1).ToString();
                renderSingleQuestion(question);
                flag = true;
            }
            else
            {
                if(question.questions.Count - 1 > currentIdxSubQuestion)
                {
                    //MessageBox.Show(currentIdxSubQuestion.ToString());
                    lblIndex.Text = (question.questions[currentIdxSubQuestion].idx+1).ToString();
                    renderCommonQuestion(question);
                    flag = false;
                }
                else if(question.questions.Count - 1 == currentIdxSubQuestion)
                {
                    lblIndex.Text = (question.questions[currentIdxSubQuestion].idx + 1).ToString();
                    renderCommonQuestion(question);
                    flag = true;
                }
                else
                {
                    currentIdxSubQuestion = question.questions.Count - 1;
                    lblIndex.Text = (question.questions[currentIdxSubQuestion].idx + 1).ToString();
                    renderCommonQuestion(question);
                    flag = true;
                }
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int limit = quiz.Questions.Count;

            if (currentIdxSubQuestion == 0)
                currentIdxList -= 1;

            if (currentIdxList < 0)
            {
                currentIdxList = 0;
                return;
            }
            
            pnExam.Controls.Clear();
            Question question = quiz.Questions[currentIdxList];


            if (question.Type == QuestionType.singleQuestion)
            {
                lblIndex.Text = (question.idx+1).ToString();
                renderSingleQuestion(question);
                flag=true;
            }
            else
            {
                if (currentIdxSubQuestion != 0)
                {
                    currentIdxSubQuestion -= 1;
                    lblIndex.Text = (question.questions[currentIdxSubQuestion].idx + 1).ToString();
                    renderCommonQuestion(question);
                    if (currentIdxSubQuestion == question.questions.Count - 1) flag = true;
                    else flag = false;
                }
                else
                {
                    currentIdxSubQuestion = question.questions.Count - 1;
                    lblIndex.Text = (question.questions[currentIdxSubQuestion].idx + 1).ToString();
                    renderCommonQuestion(question);
                    flag = false;
                }
            }
        }

        private static void ExportDataGridViewToJson(DataTable dataTable, string filePath)
        {
            // Convert DataTable to JSON
            string json = JsonConvert.SerializeObject(dataTable, fm.Indented);

            // Write JSON to file
            File.WriteAllText(filePath, json);
        }
    }
}
