using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace testUdpTcp
{
    public partial class ExamForm : Form
    {
        private Quiz quiz;
        ThongBaoDiemForm baoDiem;
        private List<QuestionUC> listQuestion = new List<QuestionUC>();
        public ExamForm(Quiz quiz)
        {
            InitializeComponent();
            this.quiz = quiz;
            InitalStateUI();
        }

        private void InitalStateUI()
        {

            pnelContainExam.Padding = new Padding(100); // Đặt Padding là 10 cho tất cả các phía (trên, dưới, trái, phải)

            int topPosition = 10; // Khoảng cách dọc giữa các UserControl
            foreach (var question in quiz.Questions)
            {
                QuestionUC questionControl = new QuestionUC();
                questionControl.SetQuestion(question);

                // Đặt vị trí cho UserControl và tăng vị trí top cho UserControl tiếp theo
                questionControl.Location = new Point(CenterPanel(questionControl), topPosition);
                topPosition += questionControl.Height + 10; // Cộng thêm khoảng cách giữa các UserControl
                listQuestion.Add(questionControl);
                pnelContainExam.Controls.Add(questionControl);
            }
            QuestionUC questionC = new QuestionUC();
            Question a = new Question();
            questionC.SetQuestion(a);

            // Đặt vị trí cho UserControl và tăng vị trí top cho UserControl tiếp theo
            questionC.Location = new Point(CenterPanel(questionC), topPosition);
            topPosition += questionC.Height + 10; // Cộng thêm khoảng cách giữa các UserControl
            questionC.PnContain.BorderStyle = BorderStyle.None;
            pnelContainExam.Controls.Add(questionC);


        }
        private int CenterPanel(QuestionUC a)
        {
            int w = pnelContainExam.Width;
            int wuc = a.Width;
            w = w / 2;
            return w - (wuc / 2);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            List<Question> questions = ConvertToQuestions(listQuestion);

            // Chuyển đổi danh sách Question thành chuỗi JSON
            string jsonString = JsonConvert.SerializeObject(questions, Formatting.Indented);

            // Đường dẫn tới file JSON
            string filePath = @"D:\DATAQUANLYLOPHOC\123456_answer.json";
            // Ghi chuỗi JSON ra file
            File.WriteAllText(filePath, jsonString);
            baoDiem = new ThongBaoDiemForm(CompareAnswers(quiz.Questions, questions));
            baoDiem.ShowDialog();
            this.Close();
        }

        private List<Question> ConvertToQuestions(List<QuestionUC> questionUCList)
        {
            List<Question> questions = new List<Question>();

            foreach (var questionUC in questionUCList)
            {
                Question question = new Question();

                question.Type = questionUC.getQuestion.Type;
                // Set the QuestionText
                question.QuestionText = questionUC.getQuestion.QuestionText;

                // Set the Options
                question.Options = questionUC.getQuestion.Options;

                // Set the Answer
                if(questionUC.getQuestion.Type == "multiple_choice" || questionUC.getQuestion.Type == "ordering")
                    question.Answer = questionUC.SelectedOptions;
                else if(questionUC.getQuestion.Type == "single_choice")
                    question.Answer = questionUC.SelectedOption;

                questions.Add(question);
            }

            return questions;
        }

        //private List<Question> ReadQuestionsFromFile(string filePath)
        //{
        //    var jsonString = File.ReadAllText(filePath);
        //    return JsonConvert.DeserializeObject<List<Question>>(jsonString);
        //}

        public int CompareAnswers(List<Question> questions, List<Question> answers)
        {
            int score = 0;

            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var answer = answers[i];

                if (question.Type == "single_choice")
                {
                    if (question.Answer.ToString() == answer.Answer.ToString())
                    {
                        score++;
                    }
                }
                else if (question.Type == "multiple_choice")
                {
                    var questionAnswerList = ConvertAnswerToList(question.Answer).OrderBy(x => x).ToList();
                    var answerAnswerList = ConvertAnswerToList(answer.Answer).OrderBy(x => x).ToList();
                    if (questionAnswerList.SequenceEqual(answerAnswerList))
                    {
                        score++;
                    }
                }
                else if (question.Type == "ordering")
                {
                    var questionAnswerList = ConvertAnswerToList(question.Answer);
                    var answerAnswerList = ConvertAnswerToList(answer.Answer);
                    if (questionAnswerList.SequenceEqual(answerAnswerList))
                    {
                        score++;
                    }
                }
            }

            return score;
        }
        private List<string> ConvertAnswerToList(object answer)
        {
            if (answer == null)
            {
                return new List<string>();
            }

            if (answer is System.Collections.Generic.List<string> objectList)
            {
                
                List<string> lst = objectList.ConvertAll(x => x.ToString());
                
                return lst;
            }

            if (answer is List<string> stringList)
            {
                return stringList;
            }

            if(answer is Newtonsoft.Json.Linq.JArray jArray)
            {
                return jArray.ToObject<List<string>>();
            }

            if (answer is string singleAnswer)
            {
                return new List<string> { singleAnswer };
            }

            // Add additional checks or conversions if needed

            throw new InvalidOperationException("Unsupported answer format.");
        }

    }
}
