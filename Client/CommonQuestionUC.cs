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
    public partial class CommonQuestionUC : UserControl
    {
        private Question question;

        public CommonQuestionUC()
        {
            InitializeComponent();
            wbQuestion.DocumentCompleted += WebBrowser_DocumentCompleted;
        }

        public void SetQuestion(Question question)
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
            Console.WriteLine(htmlContent);
            wbQuestion.DocumentText = htmlContent;
            LoadQuestion();
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            wbQuestion.Width = this.Width;
            wbQuestion.Location = new Point(0, 0);
            wbQuestion.Height = wbQuestion.Document.Body.ScrollRectangle.Height + 30 ;
            this.Height = wbQuestion.Height;
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            flpnContain.Controls.Clear();
            if (question != null && question.questions != null && question.questions.Count > 0)
            {
                int top = wbQuestion.Bottom + 10; // Đặt vị trí bắt đầu cho các câu hỏi con
                foreach (var questionChild in question.questions)
                {
                    QuestionUC q = new QuestionUC();
                    q.SetQuestion(questionChild);
                    //q.Location = new Point(0, top);
                    flpnContain.Controls.Add(q);

                    // Cập nhật top position và chiều cao của CommonQuestionUC
                    top += q.Height + 10; // Cộng thêm khoảng cách giữa các câu hỏi con
                }

                // Cập nhật chiều cao của flpnContain và CommonQuestionUC
                flpnContain.Height = top;
                //this.Height = wbQuestion.Height + flpnContain.Height + 20; // Cộng thêm khoảng cách giữa lblCommonQuestion và flpnContain
            }
        }

        private void UpdateHeight()
        {
            int totalHeight = wbQuestion.Height; // Chiều cao ban đầu là chiều cao của lblCommonQuestion + khoảng đệm

            foreach (Control control in flpnContain.Controls)
            {
                totalHeight += control.Height + 10; // Cộng thêm chiều cao của mỗi control và khoảng cách giữa chúng
            }

            flpnContain.Height = totalHeight - wbQuestion.Height ;
            this.Height = totalHeight + 60 ;
        }

        private void flpnContain_ControlAdded(object sender, ControlEventArgs e)
        {
            //UpdateHeight();
        }

        private void flpnContain_Layout(object sender, LayoutEventArgs e)
        {
            UpdateHeight();

        }
    }
}
