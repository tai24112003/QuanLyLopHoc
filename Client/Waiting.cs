using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;


namespace testUdpTcp
{
    public partial class Waiting : Form
    {
        private ExamForm exFrm;

        public Waiting()
        {
            InitializeComponent();
            CenterTextBoxAndButton(txtMSSV, btnLogin);
            btnDoExam.Hide();
        }

        public void offExamForm()
        {
            exFrm.Close();
        }

        private void CenterTextBoxAndButton(TextBox textBox, Button button)
        {
            // Căn giữa TextBox theo chiều ngang
            textBox.Left = (this.ClientSize.Width - textBox.Width) / 2;
            textBox.Top = (this.ClientSize.Height - textBox.Height - button.Height - 10) / 2; // Điều chỉnh 10 để thêm khoảng cách giữa TextBox và Button

            // Căn giữa Button theo chiều ngang, đặt nó ngay dưới TextBox
            button.Left = (this.ClientSize.Width - button.Width) / 2;
            button.Top = textBox.Bottom + 10; // Khoảng cách giữa TextBox và Button
        }

        private void CenterLabel(Label label)
        {
            // Căn giữa chiều ngang
            label.Left = (this.ClientSize.Width - label.Width) / 2;

            // Căn giữa chiều dọc
            label.Top = (this.ClientSize.Height - label.Height) / 2;
            label.Padding = new Padding(10, 10, 10, 10);

        }

        private void CenterButton(Button btn)
        {
            // Căn giữa chiều ngang
            btn.Left = (this.ClientSize.Width - btn.Width) / 2;
            // Căn giữa chiều dọc
            btn.Top = label1.Top + 400;

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            txtMSSV.Hide();
            btnLogin.Hide();
            CenterLabel(label1);
            CenterButton(btnDoExam);
            btnDoExam.Show();
        }

        private void btnDoExam_Click(object sender, EventArgs e)
        {
            string jsonText = File.ReadAllText(PathExam.Path+ "\\"+ PathExam.fileExam);

            Quiz quiz = JsonConvert.DeserializeObject<Quiz>(jsonText);
            quiz.Questions = convertType(quiz);
            Shuffle(quiz.Questions);
            int idx = 0;
            int idxList = 0;
            foreach (Question question in quiz.Questions)
            {
                question.idxList = idxList;
                if(question.Type == QuestionType.singleQuestion)
                {
                    question.idx = idx;
                    idx++;
                }
                else
                {
                    int idxInQuestions = 0;
                    foreach (Question subquestion in question.questions)
                    {
                        subquestion.idx = idx;
                        subquestion.idxList = idxList;
                        subquestion.idxSub = idxInQuestions;
                        idx++;
                        idxInQuestions++;
                    }
                }
                idxList++;
            }
            exFrm = new ExamForm(quiz);
            exFrm.ShowDialog();
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
        private List<Question> convertType(Quiz quiz)
        {
            List<Question> questions = new List<Question>();
            foreach (var question in quiz.Questions)
            {
                if (question.Type == QuestionType.singleQuestion)
                {
                    singleQuestion multiQuestion = new singleQuestion();
                    multiQuestion.id = question.id;
                    multiQuestion.Type = question.Type;
                    multiQuestion.QuestionText = question.QuestionText;
                    multiQuestion.answer = question.answer;
                    multiQuestion.options = question.options;
                    questions.Add(multiQuestion);
                }
                else if (question.Type == QuestionType.commonQuestion)
                {
                    CommonQuestion multiQuestion = new CommonQuestion();
                    multiQuestion.Type = question.Type;
                    multiQuestion.QuestionText = question.QuestionText;
                    multiQuestion.questions = question.questions;
                    questions.Add(multiQuestion);
                }
            }
            return questions;
        }

    }
}
