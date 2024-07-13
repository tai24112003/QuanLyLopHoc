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
            wbQuestion.DocumentCompleted += WebBrowser_DocumentCompleted;
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
            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Sample HTML</title>
                </head>
                <body>
                    {question.QuestionText}
                </body>
                </html>";
            if (!isLast)
            {
                wbQuestion.DocumentText = htmlContent;
                this.Height = wbQuestion.Height;
            }
            else
            {
                lblQuestion.Text = string.Empty;
                wbQuestion.Hide();
            }
            pnOption.Height = 0;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Tự động điều chỉnh kích thước của UserControl theo chiều cao của nội dung HTML
            wbQuestion.Height = wbQuestion.Document.Body.ScrollRectangle.Height + 30;
            this.Height = wbQuestion.Height + 30;
            LoadChoices();
        }
        private void LoadChoices()
        {
            pnOption.Controls.Clear();
            if (question != null && question.options != null && question.options.Count > 0)
            {
                int top = wbQuestion.Bottom - 30;
                foreach (var optionText in question.options)
                {
                    if (question.answer.Count() == 1)
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
                    else
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
                    top += 25;
                }
            }
        }
    }
}
