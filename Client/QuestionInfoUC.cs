using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class QuestionInfoUC : UserControl
    {
        private Quest Quest { get; set; }
        private Timer CountdownTimer { get; set; }
        private int Counter { get; set; }
        private StudentAnswer StudentAnswer { get; set; }
        private Action<StudentAnswer, int> SendAnswer { get; set; }
        private bool IsAnswer {  get; set; }
        public QuestionInfoUC(Quest quest, Action<StudentAnswer, int> sendAnswer)
        {
            InitializeComponent();

            Quest = quest;
            SendAnswer = sendAnswer;
            StudentAnswer=new StudentAnswer();
            IsAnswer = false;

            Counter = Quest.CountDownTime;
            CountdownTimer = new Timer
            {
                Interval = 1000
            };
            CountdownTimer.Tick += CountdownTimerQuest_Tick;
            InitUI();

        }

        private void InitUI()
        {
            ToolTip toolTip = new ToolTip();

            int screenW = SystemInformation.VirtualScreen.Width;
            int screenH = SystemInformation.VirtualScreen.Height;

            pnl_quest.Size = new Size((int)(screenW * 0.8), (int)(screenH * 0.18));
            pnl_quest.Location = new Point((int)(screenW * 0.1), (int)(screenH * 0.15));

            lbl_question.MaximumSize = new Size((int)(pnl_quest.Width - 5), 0);
            lbl_question.Location = new Point(0, 0);
            lbl_question.Text = Quest.Content;

            lbl_countdown.Text = $"Thời gian: {Counter} s";
            lbl_countdown.Location = new Point((int)(screenW * 0.03), (int)(screenH * 0.05));

            lbl_questtype_info.Text =Quest.Type.Name;
            lbl_questtype_info.Location = new Point((int)(screenW * 0.8), (int)(screenH * 0.05));
            toolTip.SetToolTip(lbl_questtype_info, Quest.Type.Description);

            pnl_answers.Size = new Size((int)(screenW * 0.8), (int)(screenH * 0.5));
            pnl_answers.Location = new Point((int)(screenW * 0.1), (int)(screenH * 0.4));

            btn_confirm.Location = new Point((int)(screenW*0.92), (int)(screenH*0.7));

            Shuffle(Quest.Results);
            List<string> titleRs = new List<string> {"A","B","C","D","E","F" };
            int index = 0;
            foreach (var item in Quest.Results)
            {
                ResultOptionUC newRs = new ResultOptionUC(item, StudentSelectTheRs, titleRs[index]);
                newRs.ChangeSize(pnl_answers.Width, pnl_answers.Height);
                pnl_answers.Controls.Add(newRs);
                index++;
            }
        }

        private bool StudentSelectTheRs(Result rs)
        {
            int index=Quest.Results.IndexOf(rs);
            if (index != -1)
            {
                if (StudentAnswer.SelectResultsId.Contains(rs.Id))
                {
                    StudentAnswer.SelectResultsId.Remove(rs.Id);
                    return true;
                }

                if (Quest.Type == QuestType.SingleSeclect && StudentAnswer.SelectResultsId.Any())
                {
                    MessageBox.Show("Đây là câu hỏi 1 đáp án");
                    return false;
                }
                int indexRs = Quest.Results[index].Id;
                StudentAnswer.SelectResultsId.Add(indexRs);
                return true;
            }

            return false;
        }
        static void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        private void CountdownTimerQuest_Tick(object sender, EventArgs e)
        {
            if (Counter > 0)
            {
                Counter--;
                lbl_countdown.Text = $"Thời gian: {Counter}s";
            }
            else
            {
                CountdownTimer.Stop();
                lbl_countdown.Text = "Time's up!";
                this.Dispose();
            }
        }
        public void StartDo()
        {
            CountdownTimer.Start();
        }
        private void btn_confirm_Click(object sender, EventArgs e)
        {
            if (!StudentAnswer.SelectResultsId.Any())
            {
                MessageBox.Show("Bạn chưa chọn đáp án nào");
                return;
            }
            if (IsAnswer)
            {
                return;
            }
            int timeDo = Quest.CountDownTime - Counter;
            StudentAnswer.TimeDoQuest = timeDo;

            foreach (Control item in pnl_answers.Controls)
            {
                item.Enabled = false;
            }
            SendAnswer?.Invoke(StudentAnswer, Quest.Index);
            IsAnswer = true;
        }
    }
}
