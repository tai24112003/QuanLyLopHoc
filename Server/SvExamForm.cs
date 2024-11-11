using DAL.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Server
{
    public partial class SvExamForm : Form
    {
        private int IndexQuestionSelected = 0;
        private int IndexTestSelected = 0;
        private int IndexTestSended = -1;
        private List<Test> Tests;
        private Action<int> SendTest;
        private Action DoTest;


        public SvExamForm(List<Test> tests, Action<int> sendTest, Action doTest)
        {
            InitializeComponent();
            Tests = tests;
            SendTest = sendTest;
            DoTest = doTest;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.pnl_slide_question.AutoScroll = true; // Cho phép cuộn nếu nội dung vượt quá kích thước panel
            this.pnl_slide_question.FlowDirection = FlowDirection.TopDown; // Đặt hướng của FlowLayoutPanel
            this.pnl_slide_question.WrapContents = false; // Không gói nội dung
            this.pnl_slide_question.Height = 0;

            this.pnl_test_list.AutoScroll = true;
            this.pnl_test_list.FlowDirection = FlowDirection.LeftToRight;
            this.pnl_test_list.WrapContents = false; // Không gói nội dung

            if (Tests.Count < 1)
            {
                AddTemplateNewQuestion();
            }
            else ChangeTest();

            Init();
        }
        private void Init()
        {
            txt_test_title.Text = Tests.First().Title;
            txt_max_point.Text = Tests.First().MaxPoint.ToString();

            cbb_quest_type.DataSource = new List<QuestType> {QuestType.SingleSeclect, QuestType.MultipleSelect, QuestType.TrueFalse };
            cbb_quest_type.DisplayMember ="Name";

            cbb_quest_time.DataSource = new List<int> { 20, 30, 40, 50, 60, 90, 120,150, 180 };
            cbb_quest_time.Format += (s, e) => e.Value = $"{e.Value} giây";

            ChangeQuestInTest();
        }

        private void ImportExcel(string filePath)
        {
            try {
                FileInfo fileInfo = new FileInfo(filePath);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                Test newTest = new Test(Tests.Count);
                using (var package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    newTest.Title = worksheet.Cells["B1"].Value?.ToString()??"";

                    if (double.TryParse(worksheet.Cells["B2"].Value?.ToString() ?? "0", out double result))
                    {
                        newTest.MaxPoint = result;
                    }
                    else
                    {
                        newTest.MaxPoint = 0; 
                    }

                    int row = 6;
                    int coutingQuestion = 0;
                    newTest.Quests.Clear();

                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        Quest newQuest = new Quest(coutingQuestion);
                        newQuest.Content=worksheet.Cells[row,1].Value?.ToString();

                        int type=Int32.Parse(worksheet.Cells[row, 2].Value?.ToString()??"0");
                        newQuest.Type = QuestType.GetQuestType(type);

                        newQuest.CountDownTime= Int32.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "30");

                        newQuest.Results.Clear();

                        int col = 4;
                        int countingResult = 0;

                        List<int> resultsCorrect = (worksheet.Cells[row,10].Value?.ToString()??"").Split(',')
                            .Select(item => {
                                return int.TryParse(item, out int number) ? (int?)number : null;
                            })
                            .Where(num => num.HasValue) // Lọc các giá trị hợp lệ
                            .Select(num => num.Value)
                            .ToList();

                        while (worksheet.Cells[row, col].Value != null && col <= 9) {
                            Result newResult = new Result(countingResult)
                            {
                                Content = worksheet.Cells[row, col].Value?.ToString() ?? "",
                                IsCorrect = resultsCorrect.Contains(countingResult)
                            };

                            newQuest.Results.Add(newResult);
                            countingResult++;
                            col++;
                        }
                        newTest.Quests.Add(newQuest);
                        coutingQuestion++;
                        row++;
                    }
                }
                Tests.Add(newTest);
                ThumbnailTest newThumbnailTest = new ThumbnailTest(newTest, true, SelectTest, DeleteTest);
                pnl_test_list.Controls.Add(newThumbnailTest);

                (pnl_test_list.Controls[IndexTestSelected] as ThumbnailTest).ChangeSelectState();
                IndexTestSelected = pnl_test_list.Controls.IndexOf(newThumbnailTest);

                pnl_test_list.HorizontalScroll.Value = pnl_test_list.HorizontalScroll.Maximum;
                pnl_test_list.PerformLayout(); // Cập nhật bố cục sau khi cuộn

                ChangeTest();

                MessageBox.Show("Import toàn bộ file thành công!");
            }
            catch(Exception ex) {
                MessageBox.Show($"Lỗi khi import: {ex.Message}");
            }
        }
        private void AddTemplateNewQuestion()
        {
            Tests.Add(new Test());

            foreach (Test test in Tests)
            {
                ThumbnailTest newTest = new ThumbnailTest(test, true, SelectTest, DeleteTest);
                pnl_test_list.Controls.Add(newTest);
            }

            foreach (Quest quest in Tests[IndexTestSelected].Quests) {
                ThumbnailQuestion newQuestion = new ThumbnailQuestion(DeleteQues, DuplicateQues, SelectQuest, quest, true);
                this.pnl_slide_question.Controls.Add(newQuestion);
            }

             // Thêm điều khiển vào FlowLayoutPanel
            lbl_num_of_quest.Text = this.pnl_slide_question.Controls.Count.ToString();
            UpdatePaneQuestListlHeight();
        }

        private void ChangeQuestInTest()
        {
            Quest temp = Tests[IndexTestSelected].Quests[IndexQuestionSelected];
            pnl_body.Controls.Clear();
            pnl_body.Controls.Add(new QuestionInfo(temp));
            cbb_quest_type.SelectedItem = temp.Type;
            cbb_quest_time.SelectedItem = temp.CountDownTime;
        }

        private void ChangeTest()
        {
            if (pnl_test_list.Controls.Count < 1)
            {
                for (int i = 0; i < Tests.Count; i++)
                {
                    bool isFirstTest = (i == 0);
                    pnl_test_list.Controls.Add(new ThumbnailTest(Tests[i], isFirstTest, SelectTest, DeleteTest));
                }
            }
            pnl_slide_question.Controls.Clear();

            Test temp=Tests[IndexTestSelected];
            IndexQuestionSelected = 0;
            for (int i = 0; i < temp.Quests.Count; i++)
            {
                bool isFirstQuest = (i == 0);
                pnl_slide_question.Controls.Add(new ThumbnailQuestion(DeleteQues, DuplicateQues, SelectQuest, temp.Quests[i], isFirstQuest));
            }
            UpdatePaneQuestListlHeight();
            ChangeQuestInTest();
        }
     
        //action delegate
        //
        private void DeleteQues(ThumbnailQuestion item)
        {
            DialogResult confirmResult = MessageBox.Show(
                            "Bạn có chắc chắn muốn xóa câu hỏi này không?",
                            "Xác nhận xóa",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            int countQuest = Tests[IndexTestSelected].Quests.Count;

            if (countQuest <= 1)
            {
                MessageBox.Show("Không thể xóa câu hỏi cuối cùng.");
                return;
            }

            int index = this.pnl_slide_question.Controls.IndexOf(item);
            pnl_slide_question.Controls.Remove(item);

            if (index == IndexQuestionSelected)
            {
                IndexQuestionSelected = countQuest - 2;

                (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
                ChangeQuestInTest();
            }
            else if (index < IndexQuestionSelected)
            {
                IndexQuestionSelected--;
            }

            Tests[IndexTestSelected].Quests.Remove(item.Quest);

            // Cập nhật nhãn số lượng câu hỏi
            lbl_num_of_quest.Text = this.pnl_slide_question.Controls.Count.ToString();
            UpdatePaneQuestListlHeight();
            SetNumQues(index);

            item.Dispose();
        }

        private void DuplicateQues(ThumbnailQuestion item)
        {
          int index=  pnl_slide_question.Controls.IndexOf(item);

            if (index != -1) // Kiểm tra xem item có trong panel hay không
            {
                // Tạo một câu hỏi mới dựa trên câu hỏi hiện tại
                ThumbnailQuestion duplicatedQuestion = new ThumbnailQuestion(DeleteQues, DuplicateQues, SelectQuest,new Quest(item.Quest), true);
                Tests[IndexTestSelected].Quests.Add(duplicatedQuestion.Quest);

                (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
                IndexQuestionSelected= duplicatedQuestion.Quest.Index;
                ChangeQuestInTest();


                // Thêm câu hỏi mới vào ngay sau vị trí của item
                pnl_slide_question.Controls.Add(duplicatedQuestion);

                // Di chuyển câu hỏi mới vào vị trí tiếp theo sau item
                pnl_slide_question.Controls.SetChildIndex(duplicatedQuestion, index + 1);

                lbl_num_of_quest.Text = pnl_slide_question.Controls.Count.ToString();

                // Cập nhật chiều cao của panel
                UpdatePaneQuestListlHeight();
                SetNumQues(index+1);
            }

        }

        private void SelectQuest(ThumbnailQuestion item)
        {
            int index= pnl_slide_question.Controls.IndexOf(item);
            if (index != -1) // Kiểm tra xem item có trong panel hay không
            {
                if (index == IndexQuestionSelected) return;
                (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
                IndexQuestionSelected=index;
                ChangeQuestInTest();
            }
        }

        private void SelectTest(ThumbnailTest item)
        {
            int index = pnl_test_list.Controls.IndexOf(item);
            if (index != -1)
            {
                if(index==IndexTestSelected) return;
                (pnl_test_list.Controls[IndexTestSelected] as ThumbnailTest).ChangeSelectState();
                IndexTestSelected=index;
                ChangeTest();
            }

        }

        private void DeleteTest(ThumbnailTest item)
        {
            DialogResult confirmResult = MessageBox.Show(
                            "Bạn có chắc chắn muốn xóa đề này không?",
                            "Xác nhận xóa",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            int countTest = Tests.Count;

            if (countTest <= 1)
            {
                MessageBox.Show("Không thể xóa đề cuối cùng.");
                return;
            }

            int index = this.pnl_test_list.Controls.IndexOf(item);
            pnl_test_list.Controls.Remove(item);

            if (index == IndexTestSelected)
            {
                IndexTestSelected = countTest-2;
                (pnl_test_list.Controls[IndexTestSelected] as ThumbnailTest).ChangeSelectState();
                ChangeTest();
            }else if (index<IndexTestSelected)
            {
                IndexTestSelected--;
            }

            Tests.Remove(item.Test);
            item.Dispose();
        }

        //update ui
        //
        private void UpdatePaneQuestListlHeight()
        {
            // Cập nhật chiều cao của panel nếu cần thiết
            int newHeight = pnl_slide_question.Controls.Count * 105 + 10; // 105 là chiều cao trung bình của ThumbnailQuestion
            newHeight = Math.Min(newHeight, 580); // Giới hạn chiều cao tối đa là 580
            this.pnl_slide_question.Height = newHeight;
            if (newHeight >= 580)
            {
                pnl_slide_question.VerticalScroll.Value = pnl_slide_question.VerticalScroll.Maximum;
                pnl_slide_question.PerformLayout(); // Cập nhật bố cục sau khi cuộn
            }
            UpdateBtnAddQuestPositions();
        }
        private void UpdateBtnAddQuestPositions()
        {
            // Cập nhật vị trí của nút thêm câu hỏi ở dưới cùng
            btn_add_question.Location = new Point(btn_add_question.Location.X, pnl_slide_question.Bottom + 20);
        }
        private void SetNumQues(int startIndex)
        {
            int index = 0;
            foreach (ThumbnailQuestion item in this.pnl_slide_question.Controls)
            {   
                if(index < startIndex)
                {
                    index++;
                    continue;
                }
                item.SetNumOfTitle(index);
                index++;
            }
        }
        //action on form
        private void add_question_btn_Click(object sender, EventArgs e)
        {
            int numBefore = Tests[IndexTestSelected].Quests.Count;
            Tests[IndexTestSelected].Quests.Add(new Quest(numBefore));
            ThumbnailQuestion thumbnailQuestion = new ThumbnailQuestion(DeleteQues, DuplicateQues, SelectQuest, Tests[IndexTestSelected].Quests.Last(), true);

            (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
            IndexQuestionSelected = thumbnailQuestion.Quest.Index;
            ChangeQuestInTest();


            this.pnl_slide_question.Controls.Add(thumbnailQuestion); // Thêm điều khiển vào FlowLayoutPanel
            lbl_num_of_quest.Text = this.pnl_slide_question.Controls.Count.ToString();
            UpdatePaneQuestListlHeight();
        }
        private void btn_add_test_Click(object sender, EventArgs e)
        {
            int numBefore = Tests.Count;
            Test newTest = new Test(numBefore);
            Tests.Add(newTest);
            ThumbnailTest newThumbnailTest = new ThumbnailTest(newTest, true, SelectTest, DeleteTest);
            pnl_test_list.Controls.Add(newThumbnailTest);
            (pnl_test_list.Controls[IndexTestSelected] as ThumbnailTest).ChangeSelectState();
            IndexTestSelected=pnl_test_list.Controls.IndexOf(newThumbnailTest);
            
            pnl_test_list.HorizontalScroll.Value = pnl_test_list.HorizontalScroll.Maximum;
            pnl_test_list.PerformLayout(); // Cập nhật bố cục sau khi cuộn
            
            ChangeTest();
        }

        private void btn_del_ques_Click(object sender, EventArgs e)
        {
            ThumbnailQuestion temp=this.pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion;
            DeleteQues(temp);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            Quest temp = Tests[IndexTestSelected].Quests[IndexQuestionSelected];
            temp.CountDownTime = Convert.ToInt32(cbb_quest_time.SelectedValue);
            temp.Type=cbb_quest_type.SelectedValue as QuestType;

            (pnl_body.Controls[0] as QuestionInfo).GetNewQuestContent();

            (this.pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).UpdateUI();
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls;*.xlsm",
                Title = "Select an Excel File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ImportExcel(filePath);
            }
        }

        private void btn_start_test_Click(object sender, EventArgs e)
        {
            Console.WriteLine("TestString");
            Console.WriteLine(Tests[IndexTestSelected].GetTestString());
            SendTest?.Invoke(IndexTestSelected);
            IndexTestSended = IndexTestSelected;
            Test testReady = Tests[IndexTestSended];

            MessageBox.Show($"Đề {testReady.Title} được phát");
        }

        private void btn_doExam_Click(object sender, EventArgs e)
        {
            if (IndexTestSended == -1)
            {
                MessageBox.Show("Chưa có đề nào được phát");
                return;
            }
            Test testReady = Tests[IndexTestSended];
            DialogResult result=MessageBox.Show($"{testReady.NumStudentsReady} sinh viên đã sẵn sàng. Bắt đầu thi?","Thông tin",MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel) return;

            this.DoTest?.Invoke();
        }
    }
}
