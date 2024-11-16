using DAL.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class SvExamForm : Form
    {
        private int IndexQuestionSelected { get; set; }
        private int IndexTestSelected { get; set; }
        private int IndexTestSended { get; set; }
        private List<Test> Tests { get; set; }
        private Action<string, string, int> SendTest { get; set; }
        private List<int> DidExams { get; set; }
        private List<string> Students { get; set; }
        private TrackExam TrackExamForm {  get; set; }
        private bool IsOpen = true;
        private bool IsEdit = false;

        public SvExamForm(List<Test> tests, Action<string, string, int> sendTest, List<string> students, List<int> didExam)
        {
            InitializeComponent();
            Tests = tests;
            SendTest = sendTest;
            IndexQuestionSelected = 0;
            IndexTestSelected = 0;
            IndexTestSended = -1;
            DidExams = didExam;
            Students = students;

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
                Tests.Add(new Test());
            }

            Init();
            ChangeTest();

            IsOpen = false;
        }
        private void Init()
        {
            txt_testTitle.Text = Tests.First().Title;
            txt_maxPoint.Text = Tests.First().MaxPoint.ToString();

            List<QuestType> questTypeDefault = new List<QuestType> { QuestType.SingleSeclect, QuestType.MultipleSelect, QuestType.TrueFalse };
            List<int> doTimeDefault = new List<int> { 20, 30, 40, 50, 60, 90, 120, 150, 180 };

            cbb_questType.DataSource = new List<QuestType>(questTypeDefault);
            cbb_questType.DisplayMember = "Name";

            cbb_questTypeFilter.DataSource = new List<QuestType> { new QuestType("Tất cả") }.Concat(questTypeDefault).ToList();
            cbb_questTypeFilter.DisplayMember = "Name";

            cbb_questType_exam.DataSource = new List<QuestType>(questTypeDefault);
            cbb_questType_exam.DisplayMember = "Name";
            cbb_questType_exam.SelectedIndex = -1;

            cbb_doQuestTime.DataSource = new List<int>(doTimeDefault);
            cbb_doQuestTime.Format += (s, e) => e.Value = $"{e.Value} giây";

            cbb_doQuestTime_exam.DataSource = new List<int>(doTimeDefault);
            cbb_doQuestTime_exam.SelectedIndex = -1;

            cbb_restEachTime_exam.DataSource = new List<int> { 3, 4, 5, 6 };
            cbb_restEachTime_exam.Format += (s, e) => e.Value = $"{e.Value} giây";
            cbb_restEachTime_exam.SelectedIndex = -1;
        }
        public void NotiHaveNewAnswer(int index){
                this.TrackExamForm?.UpdateUI(index);
        }
        private void ChangeStateExam()
        {
            Test temp = Tests[IndexTestSelected];
            txt_testTitle.Text = temp.Title;
            txt_maxPoint.Text = temp.MaxPoint.ToString();
            lbl_time_remaining.Text = $"Thời gian: {temp.GetTimeOfTest()}s";

            
            lbl_state_exam.Text = Tests[IndexTestSelected].IsExamining ? $"Trạng thái: Đang thi" : "";

            btn_trackTheExam.Visible = temp.IsExamining;
            if (DidExams.Contains(temp.Index))
            {
                btn_trackTheExam.Visible = true;
                btn_trackTheExam.Text = "Xem thông kê";
            }
            else
            {
                btn_trackTheExam.Text = "Theo dõi bài kiểm tra";
            }

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
        private void ExportExcel(string filePath)
        {
            try {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                FileInfo newFile = new FileInfo(filePath);
                if (newFile.Exists)
                {
                    newFile.Delete();  // Xóa tệp đã tồn tại    
                    newFile = new FileInfo(filePath);
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

                    Test test = Tests[IndexTestSelected];
                    // Đặt tiêu đề cho các cột
                    worksheet.Cells[1,1].Value = test.Title;
                    worksheet.Cells[2, 1].Value = "Thống kê";
                    //số sinh viên làm bài KT
                    //3.1 Sinh viên làm kiểm tra 3.2 num
                    worksheet.Cells[3, 1].Value = "Số sinh viên thực hiện";
                    worksheet.Cells[3, 2].Value = $"{test.GetNumStudentDo()}";

                    //tổng quan các câu hỏi: tổng sv làm câu hỏi, sl đúng/sai, sv nhanh nhất
                    worksheet.Cells[4, 1].Value = "Chi tiết từng câu hỏi";

                    int countIndex = 1;
                    int row = 5;

                    worksheet.Cells[row, 1].Value = "STT";
                    worksheet.Cells[row, 2].Value = "Nội dung câu hỏi";
                    worksheet.Cells[row, 3].Value = "Loại câu hỏi";
                    worksheet.Cells[row, 4].Value = "Đáp án đúng";
                    worksheet.Cells[row, 5].Value = "Số sinh viên trả lời";
                    worksheet.Cells[row, 6].Value = "Trả lời đúng";
                    worksheet.Cells[row, 7].Value = "Trả lời sai";
                    worksheet.Cells[row, 8].Value = "MSSV nhanh nhất";
                    worksheet.Cells[row, 9].Value = "Thời gian làm";

                    foreach (Quest quest in test.Quests)
                    {
                        row++;

                        worksheet.Cells[row, 1].Value = $"Câu {countIndex}";
                        worksheet.Cells[row, 2].Value = $"{quest.Content}";
                        worksheet.Cells[row, 3].Value = $"{quest.Type.Name}";

                        string v = string.Join(", ", quest.GetResultsCorrect()
                                                    .Select(item => item.Content).ToList());
                        worksheet.Cells[row, 4].Value = $"{v}";

                        int sum = quest.StudentAnswers.Count;
                        worksheet.Cells[row, 5].Value = $"{sum}";

                        int correctly = quest.GetStudentsAnsweredCorrectly().Count;
                        worksheet.Cells[row, 6].Value = $"{correctly}";

                        worksheet.Cells[row, 7].Value = $"{sum-correctly}";

                        StudentAnswer fastestStudent = quest.GetFastestStudent();
                        if (fastestStudent != null)
                        {
                            worksheet.Cells[row, 8].Value = $"{fastestStudent.StudentID}";
                            worksheet.Cells[row, 9].Value = $"{fastestStudent.TimeDoQuest}";
                        }

                        countIndex++;
                    }

                    package.Save();
                }
                MessageBox.Show($"Xuất file excel thành công");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất file excel: {ex}");
            }
        }
        private void ChangeQuestInTest()
        {
            pnl_body.Controls.Clear();
            if (IndexQuestionSelected < 0)
            {
                MessageBox.Show("Chưa có câu hỏi cho loại này");
                return;
            }
            Quest temp = (this.pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).Quest;
            pnl_body.Controls.Add(new QuestionInfo(temp));
            cbb_questType.SelectedItem = temp.Type;
            cbb_doQuestTime.SelectedItem = temp.CountDownTime;
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

            
            ChangeStateExam();

            
            FilterQuestByType(cbb_questTypeFilter.SelectedItem as QuestType);
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
                IndexQuestionSelected = pnl_slide_question.Controls.Count - 1;
                if (IndexQuestionSelected > 0)
                {
                    (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
                }
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
                int newId = Tests[IndexTestSelected].CreateIndexQuestInTest();
                // Tạo một câu hỏi mới dựa trên câu hỏi hiện tại
                ThumbnailQuestion duplicatedQuestion = new ThumbnailQuestion(DeleteQues, DuplicateQues, SelectQuest,new Quest(item.Quest,newId), true);
                Tests[IndexTestSelected].Quests.Add(duplicatedQuestion.Quest);

                (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
                IndexQuestionSelected=index+1;


                // Thêm câu hỏi mới vào ngay sau vị trí của item
                pnl_slide_question.Controls.Add(duplicatedQuestion);

                // Di chuyển câu hỏi mới vào vị trí tiếp theo sau item
                pnl_slide_question.Controls.SetChildIndex(duplicatedQuestion, index + 1);
                ChangeQuestInTest();

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

        private async void SendQuest()
        {
            await Task.Delay(5200);

            Test doingTest =Tests[IndexTestSended];
            foreach (Quest item in doingTest.Quests)
            {
                SendTest(item.GetQuestString(),"QuestCome",-1);
                doingTest.Progress++;
                await Task.Delay(item.CountDownTime * 1000+1000);

                List<StudentScore> top3 = doingTest.ScoringForClass(Students,3);
                string mess = "";
                int cout=top3.Count;
                switch (cout)
                {
                    case 0:
                        top3 = new List<StudentScore> {new StudentScore(), new StudentScore(), new StudentScore() };
                        break;
                    case 1:
                        top3 = new List<StudentScore>() { top3[0], new StudentScore(),  new StudentScore() };
                        break;
                    case 2:
                        top3 = new List<StudentScore>() { top3[0], top3[1], new StudentScore() };
                        break;
                }
                for (int i = 0; i < 3; i++)
                {
                    mess +=$"sts@{top3[i].GetTopString(i+1)}";
                }

                SendTest(mess, "TopStudent", -1);
                await Task.Delay(doingTest.RestTimeBetweenQuests * 1000+1000); //thêm 0.5s
            }
            SendTest("", "TestDone", -1);
            doingTest.IsExamining = false;
        }

        //update ui
        //
        private void UpdatePaneQuestListlHeight()
        {
            // Cập nhật chiều cao của panel nếu cần thiết
            int newHeight = pnl_slide_question.Controls.Count * 105 + 10; // 105 là chiều cao trung bình của ThumbnailQuestion
            newHeight = Math.Min(newHeight, 490); // Giới hạn chiều cao tối đa là 580
            this.pnl_slide_question.Height = newHeight;
            if (newHeight >= 490)
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
                if (!item.Visible)
                {
                    continue;
                }

                if (index < startIndex)
                {
                    index++;
                    continue;
                }
                item.SetNumOfTitle(index);
                index++;
            }
        }
        private void FilterQuestByType(QuestType type)
        {
            pnl_slide_question.Controls.Clear();
            bool isEmpty = true;
            foreach (Quest quest in Tests[IndexTestSelected].Quests )
            {
                if (quest.Type == type||type.Name=="Tất cả")
                {
                    pnl_slide_question.Controls.Add(new ThumbnailQuestion(DeleteQues, DuplicateQues, SelectQuest,quest,false));
                    isEmpty = false;
                }
            }
            IndexQuestionSelected =isEmpty?-1: 0;

            if (pnl_slide_question.Controls.Count > 0){
                (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
            }
            lbl_num_of_quest.Text = this.pnl_slide_question.Controls.Count.ToString();
            SetNumQues(0);
            UpdatePaneQuestListlHeight();
            ChangeQuestInTest();
        }

        //action on form
        private void add_question_btn_Click(object sender, EventArgs e)
        {
            int newId = Tests[IndexTestSelected].CreateIndexQuestInTest();
            int indexShowCurrent = pnl_slide_question.Controls.Count;
            if((cbb_questTypeFilter.SelectedItem as QuestType).Name == "Tất cả")
            {
                Tests[IndexTestSelected].Quests.Add(new Quest(newId));
            }
            else
            {
                Quest newQ = new Quest(numBefore)
                {
                    Type = (cbb_questTypeFilter.SelectedItem as QuestType)
                };
                Tests[IndexTestSelected].Quests.Add(newQ);
            }
            ThumbnailQuestion thumbnailQuestion = new ThumbnailQuestion(DeleteQues, DuplicateQues, SelectQuest, Tests[IndexTestSelected].Quests.Last(), true, indexShowCurrent);

            if (pnl_slide_question.Controls.Count > 0)
            {
                (pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).ChangeSelectState();
            }
            this.pnl_slide_question.Controls.Add(thumbnailQuestion); // Thêm điều khiển vào FlowLayoutPanel
            IndexQuestionSelected =pnl_slide_question.Controls.IndexOf( thumbnailQuestion);
            ChangeQuestInTest();


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
            Test tempTest = Tests[IndexTestSelected];
            tempTest.Title=txt_testTitle.Text.Trim();
            tempTest.MaxPoint =int.TryParse(txt_maxPoint.Text.Trim(), out int value)?value:0;

            txt_maxPoint.Text = tempTest.MaxPoint.ToString();

            Quest temp;
            if (IndexQuestionSelected > -1)
            {
                temp = Tests[IndexTestSelected].Quests[IndexQuestionSelected];
                temp.CountDownTime = Convert.ToInt32(cbb_doQuestTime.SelectedValue);
                temp.Type = cbb_questType.SelectedValue as QuestType;
                (pnl_body.Controls[0] as QuestionInfo).GetNewQuestContent();
                (this.pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion).UpdateUI();
                if (temp.GetResultsCorrect().Count < 1)
                {
                    MessageBox.Show("Câu hỏi chưa xác định đáp án đúng!", "Thông báo");
                }
            }

            MessageBox.Show("Lưu thành công", "Thông báo");
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
        private void btn_export_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Chọn thư mục lưu trữ";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    Console.WriteLine($"{folderPath}");
                    // Hiển thị đường dẫn thư mục đã chọn, ví dụ trên một label
                    ExportExcel(folderPath + @"\F711-Info.xlsx");
                }
            }
        }
        private void btn_sendExam_Click(object sender, EventArgs e)
        {
            SendTest(Tests[IndexTestSelected].GetTestStringOutOfQuest(),"Key-Exam",IndexTestSelected);
            IndexTestSended = IndexTestSelected;
        }
        private void btn_doExam_Click(object sender, EventArgs e)
        {
            if (IndexTestSended == -1)
            {
                MessageBox.Show("Chưa có đề nào được phát");
                return;
            }
            Test testReady = Tests[IndexTestSended];
            
            DialogResult result=MessageBox.Show($"Bắt đầu thi đề: {testReady.Title}?","Xác nhận",MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel) return;
            
            testReady.IsExamining = true;
            ChangeStateExam();
            SendTest("","DoExam",-1);

            SendQuest();
        }
        private void cbb_quest_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsOpen||!IsEdit||IndexQuestionSelected<0)
                return;
            IsEdit = false;
            ThumbnailQuestion item= pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion;
            QuestType newType = cbb_questType.SelectedItem as QuestType;
            if (item.Quest.Type == newType)
            {
                return;
            }
            item.Quest.Type = newType;
            if (item.Quest.Type== QuestType.SingleSeclect)
            {
                item.Quest.DropCorrectResult();
                ChangeQuestInTest();
            }
            item.UpdateUI();
            MessageBox.Show("Đổi loại câu hỏi thành công");
        }
        private void cbb_quest_type_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterQuestByType(cbb_questTypeFilter.SelectedItem as QuestType);
        }
        private void cbb_quest_type_DropDown(object sender, EventArgs e)
        {
            IsEdit = true;
        }
        private void cbb_quest_time_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsOpen || !IsEdit|| IndexQuestionSelected < 0)
                return;
            IsEdit = false;

            ThumbnailQuestion item = pnl_slide_question.Controls[IndexQuestionSelected] as ThumbnailQuestion;
            int newValue = int.TryParse(cbb_doQuestTime.SelectedItem.ToString(), out int value) ? value : 0;

            if (item.Quest.CountDownTime == newValue||value==0)
            {
                return;
            }
            item.Quest.CountDownTime = newValue;
            item.UpdateUI();
            lbl_time_remaining.Text = $"Thời gian: {Tests[IndexTestSelected].GetTimeOfTest()}s";
            MessageBox.Show("Đổi thời gian làm cho câu hỏi thành công");
        }
        private void btn_refesh_filter_Click(object sender, EventArgs e)
        {
            FilterQuestByType(cbb_questTypeFilter.SelectedItem as QuestType);
        }
        private void pnl_body_ControlChanged(object sender, ControlEventArgs e)
        {
            bool isShow = pnl_body.Controls.Count > 0;
            btn_del_ques.Enabled = isShow;
        }
        private void btn_confirmSetExam_Click(object sender, EventArgs e)
        {
            if (cbb_doQuestTime_exam.SelectedIndex == -1 && cbb_questType_exam.SelectedIndex == -1 && cbb_restEachTime_exam.SelectedIndex == -1)
            {
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc áp dụng cài đặt trên cho toàn bộ câu hỏi?","Cảnh báo",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Cancel) {
                return;
            }

            QuestType newType = new QuestType("default");

            int newDoTime = 0;
            int newRestTime = 0;

            if (cbb_questType_exam.SelectedItem != null)
            {
                newType=cbb_questType_exam.SelectedItem as QuestType;
            }

            if (cbb_doQuestTime_exam.SelectedItem != null)
            {
                newDoTime = int.Parse(cbb_doQuestTime_exam.SelectedItem.ToString());
            }

            if (cbb_restEachTime_exam.SelectedItem != null)
            {
                newRestTime = int.Parse(cbb_restEachTime_exam.SelectedItem.ToString());
            }

            Tests[IndexTestSelected].RestTimeBetweenQuests=newRestTime!=0 ? newRestTime : Tests[IndexTestSelected].RestTimeBetweenQuests;
            foreach (Quest item in Tests[IndexTestSelected].Quests)
            {
                item.CountDownTime = newDoTime!=0?newDoTime:item.CountDownTime;
                item.Type = newType.Name!="default"?newType:item.Type;
            }
            cbb_questType_exam.SelectedIndex = -1;
            cbb_restEachTime_exam.SelectedIndex= -1;
            cbb_doQuestTime_exam.SelectedIndex = -1;
            MessageBox.Show("Thay đổi thành công");
            ChangeTest();
        }
        private void btn_trackTheExam_Click(object sender, EventArgs e)
        {
          TrackExamForm=  new TrackExam(Tests[IndexTestSelected]);
          TrackExamForm.ShowDialog();
        }
    }
}
