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
        private int checkboxIndex = 1;
        private List<string> selectedOptions = new List<string>();
        private string selectedOption=string.Empty;
        List<CheckBox> checkBoxes = new List<CheckBox>();
        public List<string> SelectedOptions
        {
            get { return selectedOptions; }
        }
        public string SelectedOption
        {
            get { return selectedOption; }
        }
        public Question getQuestion
        {
            get { return question; }
        }
        public Panel PnContain => pnContain;
        public QuestionUC()
        {
            InitializeComponent();
        }
        public void SetQuestion(Question question)
        {
            this.question = question;
            lblQuestion.Text = question.QuestionText;
            this.Height = lblQuestion.Height+25;
            pnOption.Height = 0;
            Console.WriteLine(question.QuestionText);
            LoadChoices();
        }
        private void LoadChoices()
        {
            pnOption.Controls.Clear();

            if (question != null && question.Options != null && question.Options.Count > 0)
            {
                int top = 0; // Vị trí top ban đầu
                foreach (var optionText in question.Options)
                {
                    if (question.Type == "single_choice")
                    {
                        RadioButton radioButton = new RadioButton();
                        radioButton.Text = optionText;
                        radioButton.AutoSize = true;
                        radioButton.Location = new Point(50, top);
                        this.Height = this.Height + radioButton.Height;
                        pnOption.Height = this.Height - lblQuestion.Height - 25;// Đặt vị trí của RadioButton trên Panel
                        radioButton.CheckedChanged += (s, e) =>
                        {
                            if (radioButton.Checked) selectedOption=optionText;
                        };
                        pnOption.Controls.Add(radioButton);
                    }
                    else if (question.Type == "multiple_choice")
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
                                selectedOptions.Add(optionText);
                                
                            }
                            else
                            {
                                selectedOptions.Remove(optionText);
                               
                            }
                        };
                        this.Height = this.Height + checkBox.Height;
                        pnOption.Height = this.Height - lblQuestion.Height - 25;
                        // Đặt vị trí của RadioButton trên Panel
                        // Đặt vị trí của CheckBox trên Panel
                        pnOption.Controls.Add(checkBox);
                    }
                    else if (question.Type == "ordering")
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
                                selectedOptions.Add(optionText);
                                cb.Text = optionText + " - " + checkboxIndex;
                                checkboxIndex++;

                            }
                            else
                            {
                                selectedOptions.Remove(optionText);
                                checkboxIndex--;
                                // Nếu checkbox bị bỏ chọn, trả lại về văn bản ban đầu
                                cb.Text = optionText;
                            }
                        };
                        this.Height = this.Height + checkBox.Height;
                        pnOption.Height = this.Height - lblQuestion.Height - 25;
                        checkBoxes.Add(checkBox);
                        // Đặt vị trí của RadioButton trên Panel
                        // Đặt vị trí của CheckBox trên Panel
                        pnOption.Controls.Add(checkBox);
                    }

                    top += 25; // Tăng vị trí top để các option không chồng lên nhau
                }

                // Đặt chiều cao của UserControl dựa trên số lượng lựa chọn và khoảng cách giữa chúng
            }

        }



    }
}
