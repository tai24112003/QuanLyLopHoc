using Autofac;
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
        private readonly ClassBLL _classBLL;
        private readonly RoomBLL _roomBLL;
        private readonly SubjectBLL _subjectBLL;
        private readonly ClassSessionController _classSessionController;
        private readonly ExcelController _excelController;
        private readonly IServiceProvider _serviceProvider;
        List<Class> classes;
        public StartClassForm
            (UserBLL userBLL, 
            SubjectBLL subjectBLL, 
            ClassBLL classBLL,
            RoomBLL roomBLL,
            ClassSessionController classSessionController, 
            ExcelController excelController,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();
            DataProvider.ConnectAccess();
            this.BackColor = Color.Purple;
            this.TransparencyKey = Color.Purple;
            _userBLL = userBLL;
            _classBLL = classBLL;
            _subjectBLL = subjectBLL;
            _classSessionController = classSessionController;
            _excelController = excelController;
            _roomBLL= roomBLL;
            _serviceProvider = serviceProvider;
            this.Load += new EventHandler(MainForm_Load);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await SetupUserAutoComplete();
            await SetupClassAutoComplete();
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
                cbbName.ValueMember = "id";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async Task SetupClassAutoComplete()
        {
            try
            {
                classes = await _classBLL.GetAllClass();
                cbbName.SelectedValue = cbbClass.SelectedValue;

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
            int classID = int.Parse(cbbClass.SelectedValue.ToString());
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
              var room = await _roomBLL.GetRoomsByID(roomID);
            try
            {
                ClassSession classSession = new ClassSession
                {
                    ClassID = classID,
                    Session = cbbSession.SelectedIndex + 1,
                    StartTime = startTime,
                    EndTime = endTime,
                    user_id = userID,
                    RoomID = room.RoomID.ToString()
                };

                int sessionID = await _classSessionController.StartNewClassSession(classSession,_excelController);

                classSession.SessionID = sessionID;


                // Set các thuộc tính cần thiết của svForm
                var svFormFactory = _serviceProvider.GetRequiredService<svFormFactory>();
                var svForms = svFormFactory.Create(userID, roomID, sessionID,classID);
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
            try
             {
                int userID = int.Parse(cbbName.SelectedValue.ToString());
                List<Class> classes1 = classes.FindAll(c => c.UserID== userID);


                AutoCompleteStringCollection classCollection = new AutoCompleteStringCollection();
                foreach (var _class in classes1)
                {
                    classCollection.Add(_class.ClassName);
                }

                cbbClass.DataSource = classes1;
                cbbClass.AutoCompleteCustomSource = classCollection;
                cbbClass.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbClass.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbClass.DisplayMember = "ClassName";
                cbbClass.ValueMember = "ClassID";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void cbbSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(cbbSubject.SelectedValue + " - " + cbbSubject.SelectedItem);

        }

        private async void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                await SetupUserAutoComplete();
                await SetupClassAutoComplete();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không có mạng không thể lấy dữ liệu mới nhất: "+ex);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls;*.xlsm",
                Title = "Select an Excel File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                var excelData = ReadExcelFile(filePath);

                

                var success = await _excelController.AddDataFromExcel(excelData);

                if (success!=null)
                {
                    MessageBox.Show("Data added successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to add data. Data has been saved locally.");
                }
            }
        }


        private ExcelData ReadExcelFile(string filePath)
        {
            ExcelData excelData = new ExcelData();
            FileInfo fileInfo = new FileInfo(filePath);


            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                excelData.ClassName = worksheet.Cells["D2"].Value?.ToString();
                excelData.TeacherName = worksheet.Cells["D3"].Value?.ToString();

                int row = 6; 
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

       

        //private async void cbbClass_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        Console.WriteLine("vua chon"+cbbClass.SelectedValue);
        //        int classID = int.Parse(cbbClass.SelectedValue.ToString());
        //        List<ClassSubject> classes = await _classSubjectBLL.GetClassSubjectsByClassID(classID);

        //        AutoCompleteStringCollection classSubjectCollection = new AutoCompleteStringCollection();
        //        foreach (var _class in classes)
        //        {
        //            classSubjectCollection.Add(_class.SubjectName);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error o day ne: " + ex.Message);
        //    }
        //}

       

        private void StartClassForm_Load(object sender, EventArgs e)
        {

        }

        private void cbbClass_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void btnAddClass_Click(object sender, EventArgs e)
        {
            Class classSession = new Class();
            classSession.ClassName = cbbClass.Text;
            classSession.UserID=int.Parse(cbbClass.SelectedValue.ToString());
           await _classBLL.InsertClass(classSession);
        }
    }
}
