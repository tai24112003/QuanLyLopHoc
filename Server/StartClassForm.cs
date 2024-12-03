using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting.Contracts;
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
        private readonly ClassSessionBLL _classSessionBLL;
        private readonly RoomBLL _roomBLL;
        private readonly SessionBLL _sessionBLL;
        private readonly ExcelController _excelController;
        private readonly IServiceProvider _serviceProvider;
        private readonly LocalDataHandler _localDataHandler;
        List<Class> classes;
        public StartClassForm
            (UserBLL userBLL,
            ClassBLL classBLL,
            ClassSessionBLL classSessionBLL,
            RoomBLL roomBLL,
            ExcelController excelController,
            SessionBLL sessionBLL,
        LocalDataHandler localDataHandler,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();
            DataProvider.ConnectAccess();
            this.BackColor = Color.Purple;
            this.TransparencyKey = Color.Purple;
            _userBLL = userBLL;
            _classBLL = classBLL;
            _classSessionBLL = classSessionBLL;
            _excelController = excelController;
            _roomBLL = roomBLL;
            _serviceProvider = serviceProvider;
            _localDataHandler = localDataHandler;
            _sessionBLL= sessionBLL;
            this.Load += new EventHandler(MainForm_Load);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Khởi tạo form loading
            FormLoading loadingForm = new FormLoading();

            // Hiển thị form loading dưới dạng không modal
            loadingForm.Show();

            try
            {
                // Chạy các tác vụ bất đồng bộ
                await _localDataHandler.MigrateData();
                await SetupUserAutoComplete();
                await SetupSesisionAutoComplete();
            }
            catch (Exception ex)
            {
                loadingForm.Close();

            }
            finally
            {
                // Đóng form loading sau khi hoàn thành các tác vụ
                loadingForm.Close();
            }
        }

        private async Task SetupSesisionAutoComplete()
        {
            var sessions = await _sessionBLL.GetAllSessions();
            var diffsession = new Session();
            diffsession.ID = (int.Parse(sessions.Count.ToString()) + 1).ToString();
            diffsession.StartTime = "";
            diffsession.Display = "Khác";
            diffsession.EndTime = "";
            foreach(var ses in sessions)
            {
                ses.Display="Ca "+ ses.ID;
            }
            sessions.Add(diffsession);

            AutoCompleteStringCollection sessionCollection = new AutoCompleteStringCollection();
            AutoCompleteStringCollection startCollection = new AutoCompleteStringCollection();
            AutoCompleteStringCollection endCollection = new AutoCompleteStringCollection();

            if (sessions != null)
            {
                foreach (var session in sessions)
                {
                    sessionCollection.Add(session.ID);
                    startCollection.Add(session.StartTime);
                    endCollection.Add(session.EndTime);
                }

                // Set up cbbSession with session ID data
                cbbSession.DataSource = sessions;
                cbbSession.AutoCompleteCustomSource = sessionCollection;
                cbbSession.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbSession.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbSession.DisplayMember = "Display";  // Assuming "ID" is the property name
                cbbSession.ValueMember = "ID";

                // Set up cbbStart with start time data
                cbbStart.DataSource = sessions;
                cbbStart.AutoCompleteCustomSource = startCollection;
                cbbStart.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbStart.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbStart.DisplayMember = "StartTime"; // Assuming "StartTime" is the property name
                cbbStart.ValueMember = "ID";

                // Set up cbbEnd with end time data
                cbbEnd.DataSource = sessions;
                cbbEnd.AutoCompleteCustomSource = endCollection;
                cbbEnd.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbbEnd.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cbbEnd.DisplayMember = "EndTime"; // Assuming "EndTime" is the property name
                cbbEnd.ValueMember = "ID";
            }

        }


        private async Task SetupUserAutoComplete()
        {
            try
            {
                List<User> users = await _userBLL.GetUserLocal();
                AutoCompleteStringCollection userCollection = new AutoCompleteStringCollection();
                if (users != null)
                {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

      
        private void cbbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = cbbSession.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < cbbSession.Items.Count)
            {
                // Lấy session đã chọn
                var selectedSession = (Session)cbbSession.SelectedItem;

                if (selectedSession.Display == "Khác")
                {
                    // Luôn đặt lại giá trị thời gian khi chọn "Khác"
                    selectedSession.StartTime = null;
                    selectedSession.EndTime = null;

                    var times = PromptUserForTime("Nhập thời gian bắt đầu và kết thúc:");
                    if (times == null)
                    {
                        MessageBox.Show("Bạn đã hủy nhập thời gian.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string startTime = times.Value.startTime;
                    string endTime = times.Value.endTime;

                    if (!IsValidTimeFormat(startTime) || !IsValidTimeFormat(endTime))
                    {
                        MessageBox.Show("Thời gian không đúng định dạng HH:mm. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Không đặt SelectedIndex nếu dữ liệu không hợp lệ
                    }

                    // Chuyển đổi startTime và endTime sang kiểu DateTime để so sánh
                    if (DateTime.TryParseExact(startTime, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime startDateTime) &&
                        DateTime.TryParseExact(endTime, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime endDateTime))
                    {
                        if (endDateTime <= startDateTime)
                        {
                            MessageBox.Show("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Không đặt SelectedIndex nếu dữ liệu không hợp lệ
                        }

                        // Cập nhật thời gian cho mục "Khác"
                        selectedSession.StartTime = startTime;
                        selectedSession.EndTime = endTime;
                    }
                    else
                    {
                        MessageBox.Show("Đã xảy ra lỗi khi phân tích thời gian. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Đồng bộ cbbStart và cbbEnd
                if (cbbStart.Items.Count > selectedIndex)
                    cbbStart.SelectedIndex = selectedIndex;

                if (cbbEnd.Items.Count > selectedIndex)
                    cbbEnd.SelectedIndex = selectedIndex;
            }
            else
            {
                // Đặt lại chỉ mục nếu không hợp lệ
                if (cbbStart.Items.Count > 0)
                    cbbStart.SelectedIndex = -1;

                if (cbbEnd.Items.Count > 0)
                    cbbEnd.SelectedIndex = -1;
            }
        }




        // Hàm kiểm tra định dạng thời gian
        private bool IsValidTimeFormat(string time)
        {
            return TimeSpan.TryParseExact(time, "hh\\:mm", null, out _);
        }

        private (string startTime, string endTime)? PromptUserForTime(string message)
        {
            using (var inputForm = new Form())
            {
                inputForm.Text = "Nhập thời gian";
                inputForm.Width = 350;
                inputForm.Height = 200;

                Label lblMessage = new Label() { Text = message, Left = 10, Top = 10, Width = 320 };
                Label lblStartTime = new Label() { Text = "Thời gian bắt đầu (HH:mm):", Left = 10, Top = 40, Width = 150 };
                TextBox txtStartTime = new TextBox() { Left = 170, Top = 40, Width = 150 };

                Label lblEndTime = new Label() { Text = "Thời gian kết thúc (HH:mm):", Left = 10, Top = 80, Width = 150 };
                TextBox txtEndTime = new TextBox() { Left = 170, Top = 80, Width = 150 };

                Button btnOk = new Button() { Text = "OK", Left = 10, Top = 120, Width = 80 };
                Button btnCancel = new Button() { Text = "Hủy", Left = 100, Top = 120, Width = 80 };

                btnOk.Click += (s, e) => inputForm.DialogResult = DialogResult.OK;
                btnCancel.Click += (s, e) => inputForm.DialogResult = DialogResult.Cancel;

                inputForm.Controls.Add(lblMessage);
                inputForm.Controls.Add(lblStartTime);
                inputForm.Controls.Add(txtStartTime);
                inputForm.Controls.Add(lblEndTime);
                inputForm.Controls.Add(txtEndTime);
                inputForm.Controls.Add(btnOk);
                inputForm.Controls.Add(btnCancel);

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    return (txtStartTime.Text, txtEndTime.Text);
                }

                return null;
            }
        }




        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem tất cả các trường cần thiết đã được chọn chưa
            if (cbbClass.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbName.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn người dùng.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbbSession.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn buổi học.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Lấy đối tượng session đã chọn từ ComboBox
            var selectedSession = (Session)cbbSession.SelectedItem;
            var selectedStart = (Session)cbbStart.SelectedItem;
            var selectedEnd = (Session)cbbEnd.SelectedItem;
            var classes = (Class)cbbClass.SelectedItem;
            if (selectedStart == null )
            {
                MessageBox.Show("Vui lòng chọn thời gian bắt đầu.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedEnd == null )
            {
                MessageBox.Show("Vui lòng chọn thời gian kết thúc.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy ngày giờ hiện tại cho thời gian bắt đầu và kết thúc
            DateTime now = DateTime.Now;

         

            // Phân tích thời gian bắt đầu và kết thúc từ các đối tượng Session
            string[] startParts = selectedStart.StartTime.Split(':');
            string[] endParts = selectedEnd.EndTime.Split(':');

            int startHour = int.Parse(startParts[0]);
            int startMinute = int.Parse(startParts[1]);
            int endHour = int.Parse(endParts[0]);
            int endMinute = int.Parse(endParts[1]);

            // Thiết lập Thời gian bắt đầu với ngày hiện tại và giờ, phút đã chọn
            DateTime startTime = new DateTime(now.Year, now.Month, now.Day, startHour, startMinute, 0);

            // Tính toán khoảng cách thời gian giữa thời gian bắt đầu và kết thúc
            TimeSpan timeDifference = new TimeSpan(endHour - startHour, endMinute - startMinute, 0);

            // Thiết lập Thời gian kết thúc bằng cách cộng khoảng cách thời gian vào thời gian bắt đầu
            DateTime endTime = startTime.Add(timeDifference);

            if (startTime >= endTime)
            {
                MessageBox.Show("Thời gian bắt đầu phải trước thời gian kết thúc.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Định dạng lại Thời gian bắt đầu và kết thúc theo định dạng: dd/MM/yyyy HH:mm:ss
            string formattedStartTime = startTime.ToString("dd/MM/yyyy HH:mm:ss");
            string formattedEndTime = endTime.ToString("dd/MM/yyyy HH:mm:ss");


            int classID = int.Parse(cbbClass.SelectedValue.ToString());
            int userID = int.Parse(cbbName.SelectedValue.ToString());
            Console.WriteLine(userID);
            //string roomID = (Environment.MachineName.Split('-'))[0];
             string roomID = "F710";

            // Lấy thông tin phòng
            var room = await _roomBLL.GetRoomsByName(roomID);
            if (room == null)
            {
                MessageBox.Show( roomID + " chưa tồn tại vui lòng liên hệ khoa để biết thêm thông tin","Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            this.Hide();

            try
            {
                // Tạo và khởi tạo đối tượng ClassSession
                ClassSession classSession = new ClassSession
                {
                    ClassID = classID,
                    Session = int.Parse(selectedSession.ID), // Lấy ID của session đã chọn
                    StartTime = formattedStartTime,
                    EndTime = formattedEndTime,
                    user_id = userID,
                    RoomID = room.RoomID.ToString(),
                    ClassName = classes.ClassName,
                };

                // Chèn thông tin buổi học và lấy SessionID mới
                int sessionID = (await _classSessionBLL.InsertClassSession(classSession)).SessionID;
                classSession.SessionID = sessionID;

                // Mở form buổi học
                var svFormFactory = _serviceProvider.GetRequiredService<svFormFactory>();
                var svForms = svFormFactory.Create(userID, roomID, classSession, classID);
                svForms.Show();

                svForms.FormClosed += (s, args) =>
                {
                    this.Show(); // Hiển thị lại form hiện tại
                };

                Console.WriteLine("Buổi học đã được bắt đầu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể bắt đầu buổi học. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Lỗi khi bắt đầu buổi học: " + ex);
            }
        }





        private async void cbbName_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            FormLoading loadingForm = new FormLoading();

            loadingForm.Show();
            try
            {
                int userID = int.Parse(cbbName.SelectedValue.ToString());
                List<Class> classes1 = await _classBLL.GetClassByUserID(userID);


                AutoCompleteStringCollection classCollection = new AutoCompleteStringCollection();
                if (classes1 != null)
                {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                loadingForm.Close();
            }
        }

        private void cbbSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(cbbSubject.SelectedValue + " - " + cbbSubject.SelectedItem);

        }

        private async void pictureBox3_Click(object sender, EventArgs e)
        {
            FormLoading loadingForm = new FormLoading();

            loadingForm.Show();
            try
            {
                // Populate cbbName with user data
                var users = await _userBLL.GetUserAPI();
                AutoCompleteStringCollection userCollection = new AutoCompleteStringCollection();
                if (users != null)
                {
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

                // Populate session-related combo boxes with session data
                var sessions = await _sessionBLL.GetAllSessionsAPI();

                AutoCompleteStringCollection sessionCollection = new AutoCompleteStringCollection();
                AutoCompleteStringCollection startCollection = new AutoCompleteStringCollection();
                AutoCompleteStringCollection endCollection = new AutoCompleteStringCollection();

                if (sessions != null)
                {
                    foreach (var session in sessions)
                    {
                        sessionCollection.Add("Ca " + session.ID);
                        startCollection.Add(session.StartTime);
                        endCollection.Add(session.EndTime);
                    }

                    // Set up cbbSession with session ID data
                    cbbSession.DataSource = sessions;
                    cbbSession.AutoCompleteCustomSource = sessionCollection;
                    cbbSession.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cbbSession.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cbbSession.DisplayMember = "ID";  // Assuming "ID" is the property name
                    cbbSession.ValueMember = "ID";

                    // Set up cbbStart with start time data
                    cbbStart.DataSource = sessions;
                    cbbStart.AutoCompleteCustomSource = startCollection;
                    cbbStart.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cbbStart.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cbbStart.DisplayMember = "StartTime"; // Assuming "StartTime" is the property name
                    cbbStart.ValueMember = "ID";

                    // Set up cbbEnd with end time data
                    cbbEnd.DataSource = sessions;
                    cbbEnd.AutoCompleteCustomSource = endCollection;
                    cbbEnd.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cbbEnd.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cbbEnd.DisplayMember = "EndTime"; // Assuming "EndTime" is the property name
                    cbbEnd.ValueMember = "ID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không có mạng không thể lấy dữ liệu mới nhất", "False", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Console.WriteLine("Không có mạng không thể lấy dữ liệu mới nhất: " + ex);
            }
            finally
            {
                loadingForm.Close();
            }
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Tệp Excel|*.xlsx;*.xls;*.xlsm",
                Title = "Chọn một tệp Excel"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FormLoading loadingForm = new FormLoading();

                loadingForm.Show();
                try
                {
                    string filePath = openFileDialog.FileName;
                    var excelData = ReadExcelFile(filePath);

                    var success = await _excelController.AddDataFromExcel(excelData);

                    if (success != null)
                    {
                        MessageBox.Show("Dữ liệu đã được thêm thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi nhập dữ liệu máy tính và sinh viên: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Lỗi bên trong: {ex.InnerException.Message}");
                        Console.WriteLine($"Stack Trace bên trong: {ex.InnerException.StackTrace}");
                    }
                }
                finally
                {
                    loadingForm.Close();
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
                string date = DateTime.Now.ToString();
                while (worksheet.Cells[row, 2].Value != null)
                {
                    Student student = new Student
                    {
                        StudentID = worksheet.Cells[row, 2].Value.ToString(),
                        LastName = worksheet.Cells[row, 4].Value.ToString(),
                        FirstName = worksheet.Cells[row, 3].Value.ToString(),
                        LastTime = date,
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
            FormLoading loadingForm = new FormLoading();

            loadingForm.Show();
            try
            {
                Class classSession = new Class();
                classSession.ClassName = cbbClass.Text;
                classSession.UserID = int.Parse(cbbName.SelectedValue.ToString());
                DateTime currentDate = DateTime.Now;
                string formattedDateTime = currentDate.ToString("dd/MM/yyyy HH:mm:ss");

                classSession.LastTime = formattedDateTime;
                await _classBLL.InsertClass(classSession);
            }
            finally
            {
                loadingForm.Close();
            }
        }

        
    }
}
