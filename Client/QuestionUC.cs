using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class QuestionUC : UserControl
    {
        private Question question;
        static int idx = 0;
        private int checkboxIndex = 1;
        List<CheckBox> checkBoxes = new List<CheckBox>();
        public Panel PnContain => pnContain;
        public QuestionUC()
        {
            InitializeComponent();
            idx++;
        }
        private static string WrapText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || maxLength < 1)
            {
                return text;
            }

            StringBuilder sb = new StringBuilder();
            int currentIndex = 0;

            while (currentIndex < text.Length)
            {
                // Tìm vị trí kết thúc của đoạn hiện tại
                int endIndex = Math.Min(currentIndex + maxLength, text.Length);

                // Nếu đoạn hiện tại kết thúc trong giữa từ, tìm khoảng trắng gần nhất
                if (endIndex < text.Length && !char.IsWhiteSpace(text[endIndex]))
                {
                    int lastSpace = text.LastIndexOf(' ', endIndex);
                    if (lastSpace > currentIndex)
                    {
                        endIndex = lastSpace;
                    }
                }

                sb.Append(text.Substring(currentIndex, endIndex - currentIndex).Trim());
                currentIndex = endIndex;

                if (currentIndex < text.Length)
                {
                    sb.Append("\n");
                    while (currentIndex < text.Length && char.IsWhiteSpace(text[currentIndex]))
                    {
                        currentIndex++;
                    }
                }
            }

            return sb.ToString();
        }
        public void SetQuestion(Question question, Boolean isLast = false)
        {
            this.question = question;
            if (!isLast)
                lblQuestion.Text = WrapText(idx + "." + question.QuestionText, 70);
            else lblQuestion.Text = string.Empty;
            this.Height = lblQuestion.Height + 25;
            pnOption.Height = 0;
            LoadChoices();
        }
        private void LoadChoices()
        {
            pnOption.Controls.Clear();
            if (question != null && question.Options != null && question.Options.Count > 0)
            {
                int top = 0;
                foreach (var optionText in question.Options)
                {
                    if (question.Type == QuestionType.singleType)
                    {  
                        RadioButton radioButton = new RadioButton();
                        radioButton.Text = optionText;
                        radioButton.AutoSize = true;
                        radioButton.Location = new Point(50, top);
                        this.Height = this.Height + radioButton.Height;
                        pnOption.Height = this.Height - lblQuestion.Height - 25;
                        radioButton.CheckedChanged += (s, e) =>
                        {
                            if (radioButton.Checked)
                                question.AddAnswer(optionText);
                        };
                        pnOption.Controls.Add(radioButton);
                    }
                    else if (question.Type == QuestionType.multipleType)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = optionText;
                        checkBox.AutoSize = true;
                        checkBox.Location = new Point(50, top);
                        checkBox.CheckedChanged += (s, e) =>
                        {
                            CheckBox cb = s as CheckBox;
                            if (cb.Checked)
                            {
                                question.AddAnswer(optionText);
                            }
                            else
                            {
                                question.RemoveAnswer(optionText);
                            }
                        };
                        this.Height = this.Height + checkBox.Height;
                        pnOption.Height = this.Height - lblQuestion.Height - 25;
                        pnOption.Controls.Add(checkBox);
                    }
                    else if (question.Type == QuestionType.orderingType)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = optionText;
                        checkBox.AutoSize = true;
                        checkBox.Location = new Point(50, top);
                        checkBox.CheckedChanged += (s, e) =>
                        {
                            CheckBox cb = s as CheckBox;
                            if (cb.Checked)
                            {
                                question.AddAnswer(optionText);
                                cb.Text = optionText + " - " + checkboxIndex;
                                checkboxIndex++;

                            }
                            else
                            {
                                question.RemoveAnswer(optionText);
                                checkboxIndex--;
                                cb.Text = optionText;
                            }
                        };
                        this.Height = this.Height + checkBox.Height;
                        pnOption.Height = this.Height - lblQuestion.Height - 25;
                        checkBoxes.Add(checkBox);
                        pnOption.Controls.Add(checkBox);
                    }
                    top += 25;
                }
            }
        }
    }
}
