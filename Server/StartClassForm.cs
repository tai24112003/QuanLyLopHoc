// /Forms/StartClassForm.cs
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class StartClassForm : Form
    {
        private readonly UserBLL _userBLL;
        private readonly SubjectBLL _subjectBLL;
        private readonly ClassSessionController _classSessionController;
        private readonly ComputerSessionController _computerSessionController;

        public StartClassForm(UserBLL userBLL, SubjectBLL subjectBLL, ClassSessionController classSessionController, ComputerSessionController computerSessionController)
        {
            InitializeComponent();

            this.BackColor = Color.Purple;
            this.TransparencyKey = Color.Purple;
            _userBLL = userBLL;
            _subjectBLL = subjectBLL;
            _classSessionController = classSessionController;
            _computerSessionController = computerSessionController;
            this.Load += new EventHandler(MainForm_Load);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await SetupUserAutoComplete();
        }

        private async Task SetupUserAutoComplete()
        {
            try
            {
                string role = "GV";
                List<User> users = await _userBLL.GetListUser(role);

                AutoCompleteStringCollection userCollection = new AutoCompleteStringCollection();
                foreach (var user in users)
                {
                    userCollection.Add(user.name);
                    
                }

                cbbName.DataSource = users;
                cbbName.AutoCompleteCustomSource = userCollection;
                cbbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbName.DisplayMember = "name";
                cbbName.ValueMember = "user_id";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

       

        private void cbbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbSession.SelectedIndex == 0)
            {
                cbbStart.SelectedIndex = 0;
                cbbEnd.SelectedIndex = 0;
            }
            else if (cbbSession.SelectedIndex == 1)
            {
                cbbStart.SelectedIndex = 1;
                cbbEnd.SelectedIndex = 1;
            }
            else if (cbbSession.SelectedIndex == 2)
            {
                cbbStart.SelectedIndex = 2;
                cbbEnd.SelectedIndex = 2;
            }
            else if (cbbSession.SelectedIndex == 3)
            {
                cbbStart.SelectedIndex = 3;
                cbbEnd.SelectedIndex = 3;
            }
            else
            {
                cbbEnd.SelectedIndex = -1;
                cbbStart.SelectedIndex = -1;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            this.Hide();
            string className = txtClass.Text;
            int userID = int.Parse(cbbName.SelectedValue.ToString());
            Console.WriteLine(userID);
            string roomID = "F71";
            string[] start = cbbStart.SelectedItem.ToString().Split('h');
            int hour = int.Parse(start[0]);
            int minute = int.Parse(start[1]);

            // Lấy ngày hiện tại
            DateTime now = DateTime.Now;

            // Tạo đối tượng DateTime với ngày hiện tại và giờ phút từ chuỗi
            DateTime startTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            string[] end = cbbEnd.SelectedItem.ToString().Split('h');
             hour = int.Parse(end[0]);
             minute = int.Parse(end[1]);


            // Tạo đối tượng DateTime với ngày hiện tại và giờ phút từ chuỗi
            DateTime endTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            try
            {
                ClassSession classSession = new ClassSession
                {
                    ClassName = className,
                    Session = cbbSession.SelectedIndex + 1,
                    StartTime = startTime,
                    EndTime = endTime,
                    user_id = userID,
                    RoomID = roomID
                };

                int sessionID = await _classSessionController.StartNewClassSession(classSession);

                var roomBLL = ServiceLocator.ServiceProvider.GetRequiredService<RoomBLL>();

                svForm svForms = new svForm(userID, roomID, roomBLL,sessionID,_computerSessionController);
                svForms.Show();

                Console.WriteLine("Class session started successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to start class session: " + ex);
            }
        }

        private void cbbName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(cbbName.SelectedValue + " - " + (User)cbbName.SelectedItem);
        }

        private void cbbSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(cbbSubject.SelectedValue + " - " + cbbSubject.SelectedItem);

        }

        private async void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                string role = "GV";
                List<User> users = await _userBLL.GetUsersByRoleFromAPI(role);

                AutoCompleteStringCollection userCollection = new AutoCompleteStringCollection();
                foreach (var user in users)
                {
                    userCollection.Add(user.name);

                }

                cbbName.DataSource = users;
                cbbName.AutoCompleteCustomSource = userCollection;
                cbbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbName.DisplayMember = "name";
                cbbName.ValueMember = "user_id";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không có mạng không thể lấy dữ liệu mới nhất");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Select an Excel File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                var excelData = ReadExcelFile(filePath);
                DisplayExcelData(excelData);
            }
        }

        private ExcelData ReadExcelFile(string filePath)
        {
            ExcelData excelData = new ExcelData();
            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                string tmp = worksheet.Cells["D2"].Value?.ToString();
                string[] tmp1=tmp.Split('-');
                excelData.ClassName = tmp1[0];
                excelData.SubjectName = tmp1[1];
                excelData.TeacherName = worksheet.Cells["D3"].Value?.ToString();

                int row = 6; // Start reading student data from row 8
                while (worksheet.Cells[row, 2].Value != null)
                {
                    Student student = new Student
                    {
                        StudentID = worksheet.Cells[row, 2].Value.ToString(),
                        LastName = worksheet.Cells[row, 3].Value.ToString(),
                        FirstName = worksheet.Cells[row, 4].Value.ToString(),
                    };
                    excelData.Students.Add(student);
                    row++;
                }
            }

            return excelData;
        }

        private void DisplayExcelData(ExcelData excelData)
        {
            //lblClassName.Text = $"Tên lớp: {excelData.ClassName}";
            //lblSubjectName.Text = $"Tên môn: {excelData.SubjectName}";
            //lblTeacherName.Text = $"Tên giáo viên: {excelData.TeacherName}";

            //dgvStudents.DataSource = excelData.Students;
        }
    }
}
