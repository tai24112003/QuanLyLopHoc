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
    public partial class TrackExam : Form
    {
        private Test Test { get; set; }
        private bool IsPresent { get; set; }
        private bool IsScore { get; set; }
        private int IndexSelectQuest { get; set; }
        private BindingList<StudentScore> RankingList { get; set; }
        public TrackExam(Test test)
        {
            InitializeComponent();
            Test = test;
            Init();
        }

        private void Init()
        {
            RankingList = new BindingList<StudentScore>();
            dgv_ranking.DataSource = RankingList;
            dgv_ranking.ReadOnly = true;
            dgv_ranking.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_ranking.DefaultCellStyle.SelectionBackColor = Color.Blue;
            dgv_ranking.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv_ranking.Columns["Top"].HeaderText = "Thứ hạng";
            dgv_ranking.Columns["StudentId"].HeaderText = "MSSV";
            dgv_ranking.Columns["Score"].HeaderText = "Điểm";
            dgv_ranking.Columns["NumCorrect"].HeaderText = "Câu đúng";
            dgv_ranking.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_ranking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
          
            this.lbl_currentQuest.Visible = false;
            IndexSelectQuest = 0;
            RenderSlideQuests();
            UpdateUI(IndexSelectQuest);
        }
        private void RenderSlideQuests()
        {
            foreach (Quest quest in Test.Quests)
            {
                pnl_slideQuests.Controls.Add(new ThumbnailQuestion(null,null, SelectQuest, quest,quest.Index==IndexSelectQuest, quest.Index,true ));
            }
        }
        public void UpdateUI(int index)
        {
            int foundCount = 0;
            foreach (ThumbnailQuestion item in pnl_slideQuests.Controls)
            {
                if (item.Quest.Index == IndexSelectQuest)
                {
                    item.ChangeSelectState();
                    foundCount++;
                }

                if (item.Quest.Index == index)
                {
                    item.ChangeSelectState();
                    foundCount++;
                }

                if (foundCount == 2)
                    break;
            }
            IndexSelectQuest = index;

            int numQ = Test.Quests.Count;
            this.lbl_testTitle.Text = Test.Title;
            this.btn_switchView.Text = IsPresent ? "Phần trăm" : "Số lượng";
            this.lbl_state.Text = "Trạng thái: " + (Test.IsExamining ? "Đang thi" : "Đã thi");
            this.lbl_numQuest.Text = $"Số câu hỏi: {numQ}";
            this.lbl_numStudent.Text = $"Số sinh viên làm: {Test.GetNumStudentDo()}";
            this.lbl_maxP.Text = $"Điểm tối đa: {Test.MaxPoint}";

            if (Test.IsExamining)
            {
                this.lbl_currentQuest.Visible = true;
                this.lbl_currentQuest.Text = $"Tiến trình: {Test.Progress}/{numQ}";
            }
            RenderAnswer();
            UpdateRankings();
        }

        private void SelectQuest(ThumbnailQuestion item)
        {
            int index = pnl_slideQuests.Controls.IndexOf(item);
            if (index != -1) // Kiểm tra xem item có trong panel hay không
            {
                if (index == IndexSelectQuest) return;
                (pnl_slideQuests.Controls[IndexSelectQuest] as ThumbnailQuestion).ChangeSelectState();
                IndexSelectQuest = index;
                RenderAnswer();
            }
        }
        private void UpdateRankings()
        {
            List<StudentScore> studentsScore = Test.ScoringForClass();

            // Cập nhật danh sách
            RankingList.Clear();
            foreach (var student in studentsScore)
            {
                RankingList.Add(student);
            }
        }

        private void RenderAnswer()
        {
            pnl_chart.Controls.Clear();
            pnl_chart.Controls.Add(this.btn_switchView);
            int mH = (int)(this.pnl_chart.ClientSize.Height * 0.6);
            int w = this.pnl_chart.ClientSize.Width;
            int top = this.pnl_chart.ClientSize.Height - btn_switchView.Bottom;

            Quest item = Test.Quests[IndexSelectQuest];

            lbl_fastest.Text = "";
            StudentAnswer fastest = item.GetFastestStudent();
            if (fastest != null)
            {
                lbl_fastest.Text = $"Sinh viên nhanh nhất: {fastest.StudentID}; Thời gian: {fastest.TimeDoQuest}s";
            }

            lbl_studentDo.Text = $"Số sinh viên làm: {item.StudentAnswers.Count}/{Test.GetNumStudentDo()}";
            pnl_chart.Controls.Add(lbl_studentDo);
            pnl_chart.Controls.Add(lbl_fastest);
            int numAnswer = item.Results.Count;
            // Calculate panel width with a maximum of 60
            int panelWidth = Math.Min(w / numAnswer - 20, 60);
            int totalPanelWidth = panelWidth * numAnswer;

            // Calculate spacing to center the panels
            int spacing = (w - totalPanelWidth) / (numAnswer + 1);
            int tmp = 0;

            List<string> titles = new List<string> {"A","B","C","D","E","F" };
            foreach (Result rs in item.Results)
            {
                int sum = Test.GetNumStudentDo() == 0 ? 1 : Test.GetNumStudentDo();
                int value = item.GetNumStudentSelectByResult(rs);
                double valueView = !IsPresent ? value : Math.Round((double)(value * 100 / sum ), 2);

                TrackAnswerInfo answerPanel = new TrackAnswerInfo
                {
                    Width = panelWidth,
                    Height = mH*value/sum+5,  
                    BackColor = rs.IsCorrect ? Color.LimeGreen : Color.LightGray, // Optional: distinguishable background color
                    BorderStyle = BorderStyle.FixedSingle 
                };
                toolTip1.SetToolTip(answerPanel, rs.Content);

                int x = spacing + tmp * (panelWidth + spacing);
                int y = pnl_chart.ClientSize.Height - top + 80; // Center vertically
                answerPanel.Location = new Point(x, y);
                pnl_chart.Controls.Add(answerPanel);

                Label answerTitle = new Label
                {
                    AutoSize = false, // Disable AutoSize to control width
                    Width = panelWidth, // Set the maximum width
                    Text = titles[tmp], // Replace with actual answer text
                    Font = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter, // Center align text
                    Location = new Point(x, answerPanel.Top - 25) // Center horizontally above panel
                };
                pnl_chart.Controls.Add(answerTitle);
                
                Label answerValue = new Label
                {
                    Name = "lbl_value" + tmp,
                    AutoSize = false,
                    Width = panelWidth, // Set the maximum width
                    Text = $"{valueView} " + (IsPresent ? "%" : ""), // Replace with actual answer text
                    TextAlign = ContentAlignment.MiddleCenter, // Center align text
                    Location = new Point(x, answerPanel.Bottom + 10) // Center horizontally below panel
                };
                pnl_chart.Controls.Add(answerValue);
                toolTip1.SetToolTip(answerPanel, rs.Content);
                toolTip1.SetToolTip(answerTitle, rs.Content);
                toolTip1.SetToolTip(answerValue, rs.Content);

                tmp++;
            }
        }
        private void btn_switchView_Click(object sender, EventArgs e)
        {
           IsPresent=!IsPresent;
           UpdateUI(IndexSelectQuest);
        }
        private void TrackExam_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private void btn_switchScoreView_Click(object sender, EventArgs e)
        {
            IsScore = !IsScore;

        }
    }
}
