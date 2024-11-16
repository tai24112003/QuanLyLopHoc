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
        private int IndexSelectQuest { get; set; }
        public TrackExam(Test test)
        {
            InitializeComponent();
            Test = test;

            Init();
        }

        private void Init()
        {
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
            if (index != IndexSelectQuest)
            {
                foreach (ThumbnailQuestion item in pnl_slideQuests.Controls)
                {
                    if (item.Quest.Index != IndexSelectQuest)
                    {
                        continue;
                    }
                    item.ChangeSelectState();
                    break;
                }
                IndexSelectQuest=index;
                foreach (ThumbnailQuestion item in pnl_slideQuests.Controls)
                {
                    if (item.Quest.Index != IndexSelectQuest)
                    {
                        continue;
                    }
                    item.ChangeSelectState();
                    break;
                }
            }
            int numQ = Test.Quests.Count;
            this.lbl_testTitle.Text = Test.Title;
            this.btn_switchView.Text = IsPresent ? "Phần trăm" : "Số lượng";
            this.lbl_state.Text = "Trạng thái: " + (Test.IsExamining ? "Đang thi" : "Đã thi");
            this.lbl_numQuest.Text = $"Số câu hỏi: {numQ}";


            if (Test.IsExamining)
            {
                this.lbl_currentQuest.Visible = true;
                this.lbl_currentQuest.Text = $"Tiến trình: {Test.Progress}/{numQ}";
            }
            RenderAnswer();
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
        private void RenderAnswer()
        {
            pnl_chart.Controls.Clear();
            pnl_chart.Controls.Add(this.btn_switchView);
            int mH = (int)(this.pnl_chart.ClientSize.Height * 0.6);
            int left = this.pnl_chart.ClientSize.Width;
            int top = this.pnl_chart.ClientSize.Height - btn_switchView.Bottom;


            Quest item = Test.Quests[IndexSelectQuest];

            int numAnswer = item.Results.Count;
            // Calculate panel width with a maximum of 80
            int panelWidth = Math.Min(left / numAnswer - 20, 80);
            int totalPanelWidth = panelWidth * numAnswer;

            // Calculate spacing to center the panels
            int spacing = (left - totalPanelWidth) / (numAnswer + 1);
            int tmp = 0;

            List<string> titles = new List<string> {"A","B","C","D",
                    "E",
                    "F" };
            foreach (Result rs in item.Results)
            {
                int sum = item.StudentAnswers.Count == 0 ? 1 : item.StudentAnswers.Count;
                int value = item.GetNumStudentSelectByResult(rs);
                double valueView = !IsPresent ? value : Math.Round((double)(value * 100 / sum ), 2);

                // Create a panel
                TrackAnswerInfo answerPanel = new TrackAnswerInfo
                {
                    Width = panelWidth,
                    Height = mH*value/sum+5, // Adjust height as needed
                    BackColor = rs.IsCorrect ? Color.LimeGreen : Color.LightGray, // Optional: distinguishable background color
                    BorderStyle = BorderStyle.FixedSingle // Optional: border for better visibility
                };
                toolTip1.SetToolTip(answerPanel, rs.Content);

                // Calculate position
                int x = spacing + tmp * (panelWidth + spacing);
                int y = pnl_chart.ClientSize.Height - top + 80; // Center vertically
                answerPanel.Location = new Point(x, y);
                pnl_chart.Controls.Add(answerPanel);

                // Add content to panel (e.g., answer text)
                // Label title
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
                
                // Label value
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
    }
}
