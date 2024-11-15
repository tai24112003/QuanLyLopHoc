using System;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsTimer = System.Windows.Forms.Timer;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using OfficeOpenXml;
using System.IO.Compression;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using DAL.Models;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Shapes;
using System.Security.Cryptography.X509Certificates;

namespace Server
{
    public partial class svForm : Form
    {
        private bool isFullInfoMode = false;
        private WinFormsTimer timer;
        private string Ip= "192.168.20.52";
        private TcpListener tcpListener;
        private Thread listenThread;
        private Thread screenshotThread;
        private NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        private List<List<string>> fullInfoList = new List<List<string>>();
        private List<List<string>> summaryInfoList = new List<List<string>>();
        private bool isRunning = true;
        private List<Dictionary<string, string>> privateStandardInfoList = new List<Dictionary<string, string>>();
        private RoomBLL _roomBLL;
        private SessionComputerBLL _sessionComputerBLL;
        private ClassSessionBLL _classSessionBLL;
        private ClassStudentBLL _classStudentBLL;
        private AttendanceBLL _attendanceBLL;
        private StudentBLL _studentBLL;
        private ComputerBLL _computerBLL;
        private string roomID;
        private int classID;
        private int userID;
        private string selectedStudent = "";
        private int numbercomputer;
        private Room room = new Room();
        private int sessionID;
        private List<SessionComputer> sessionComputers = new List<SessionComputer>();
        private List<ClassSession> classSessions = new List<ClassSession>();
        private List<ClassStudent> classStudents = new List<ClassStudent>();
        private List<Attendance> attendances = new List<Attendance>();
        private List<ClassStudent> students;
        private IServiceProvider _serviceProvider;
        ContextMenuStrip contextMenuStrip;
        ContextMenuStrip contextMenuStrip1;
        SlideShowForm form1;
        ImageList imageList1 = new ImageList();

        SvExamForm ExamForm {  get; set; }
        private List<Test> Tests {get; set; }
        private int IndexTestReady {  get; set; }
        private List<string> StudentsAreReady {get;set;}
        private List<int> DidExamId {get; set;}

        private Dictionary<string, List<List<string>>> groups = new Dictionary<string, List<List<string>>>();
        public svForm()
        {
            InitializeComponent();
            Tests = new List<Test>();
            IndexTestReady = -1;
            StudentsAreReady = new List<string>();
            DidExamId = new List<int>();
        }
        public void Initialize(int userID, string roomID, int classID, RoomBLL roomBLL, int sessionID, SessionComputerBLL sessionComputer, ClassSessionBLL classSession, ClassStudentBLL classStudentBLL, AttendanceBLL attendanceBLL, ComputerBLL computerBLL, StudentBLL studentBLL, IServiceProvider serviceProvider)
        {
            this.userID = userID;
            this.roomID = roomID;
            this.classID = classID;
            _classStudentBLL = classStudentBLL;
            this._roomBLL = roomBLL;
            this._sessionComputerBLL = sessionComputer;
            this._classSessionBLL = classSession;
            this._attendanceBLL = attendanceBLL;
            this._computerBLL = computerBLL;
            this.sessionID = sessionID;
            this._studentBLL = studentBLL;
            _serviceProvider = serviceProvider;

            //Ip = ;
            Ip = getIPServer();
            InitializeContextMenu();

            // Thực hiện các logic khởi tạo khác nếu cần thiết
        }

        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip1 = new ContextMenuStrip();

            // Tạo các mục menu
            var menuItem3 = new ToolStripMenuItem("Trình chiếu đến máy chủ");
            var menuItem4 = new ToolStripMenuItem("Thêm sinh viên");
            var menuItem5 = new ToolStripMenuItem("Chọn sinh viên kiểm tra máy");

            // Thêm các mục menu vào ContextMenuStrip
            contextMenuStrip.Items.Add(menuItem3);
            contextMenuStrip1.Items.Add(menuItem4);
            contextMenuStrip1.Items.Add(menuItem5);

            // Gắn sự kiện click cho các mục menu
            menuItem3.Click += contextMenu_SlideShowToClient_Click;
            menuItem4.Click += async (sender, e) => await AddSelectedStudentsAsync();
            menuItem5.Click += contextMenu_SelectStudentCheckMachine_Click; 
        }

        private async Task AddSelectedStudentsAsync()
        {
            try
            {
                // Kiểm tra nếu không có dòng nào được chọn
                if (dgv_attendance.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một sinh viên để thêm.");
                    return;
                }

                List<Student> students = new List<Student>();
                List<ClassStudent> class_students = new List<ClassStudent>();

                // Duyệt qua các dòng được chọn
                foreach (DataGridViewRow selectedRow in dgv_attendance.SelectedRows)
                {
                    // Lấy thông tin từ dòng
                    string studentID = selectedRow.Cells["StudentID"]?.Value?.ToString();
                    string firstName = selectedRow.Cells["FirstName"]?.Value?.ToString();
                    string lastName = selectedRow.Cells["LastName"]?.Value?.ToString();

                    // Kiểm tra dữ liệu hợp lệ
                    if (string.IsNullOrWhiteSpace(studentID) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                    {
                        MessageBox.Show("Thông tin sinh viên không hợp lệ, vui lòng kiểm tra lại.");
                        continue;
                    }

                    // Xử lý thêm sinh viên (ví dụ thêm vào danh sách, lưu vào cơ sở dữ liệu, v.v.)
                    var newStudent = new Student
                    {
                        StudentID = studentID,
                        FirstName = firstName,
                        LastName = lastName,
                        LastTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                    };
                    students.Add(newStudent);

                    var clst = new ClassStudent { StudentID = studentID, ClassID = classID };
                    class_students.Add(clst);
                }

                await _studentBLL.InsertStudent(students);
                await _classStudentBLL.InsertClassStudent(class_students);

                // Thông báo sau khi thêm xong
                MessageBox.Show($"Đã thêm {dgv_attendance.SelectedRows.Count} sinh viên vào danh sách.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sinh viên: {ex.Message}");
            }
        }

        // Event handler for "Chọn sinh viên kiểm tra máy"
        private void contextMenu_SelectStudentCheckMachine_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu không có dòng nào được chọn
                if (dgv_attendance.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một sinh viên.");
                    return;
                }

                selectedStudent = dgv_attendance.SelectedRows[0].Cells["StudentID"]?.Value?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn sinh viên kiểm tra máy: {ex.Message}");
            }
        }

        // Dummy method to simulate machine check for selected students



        private async Task SetupAttendance(int classID)
        {
            try
            {
                // Thiết lập các cột cố định (StudentID, FirstName, LastName) nếu chưa có
                if (dgv_attendance.Columns.Count == 0)
                {
                    dgv_attendance.Columns.Add("StudentID", "Student ID");
                    dgv_attendance.Columns.Add("FirstName", "First Name");
                    dgv_attendance.Columns.Add("LastName", "Last Name");
                }

                var attendances = await _attendanceBLL.GetAttendanceByClassID(classID);

                if (attendances == null || !attendances.Any())
                {
                    students = await _classStudentBLL.GetClassStudentsByID(classID);


                    // Thêm cột ngày hiện tại cho buổi học
                    string sessionID = DateTime.Now.ToString("ddMMyyyy");
                    if (!dgv_attendance.Columns.Contains(sessionID))
                    {
                        dgv_attendance.Columns.Add(sessionID, DateTime.Now.ToString("dd/MM/yyyy"));
                    }
                    if (students == null || students.Count == 0)
                    {
                        MessageBox.Show("Không có thông tin học sinh.");
                    }
                    else
                        foreach (var student in students)
                        {
                            string cleanStudentID = student.StudentID.Replace("-", string.Empty);
                            dgv_attendance.Rows.Add(cleanStudentID, student.Student.FirstName, student.Student.LastName, 'v');
                        }
                }
                else
                {
                    // Sắp xếp dữ liệu theo StartTime
                    var sortedSessions = attendances.OrderBy(a => a.StartTime).ToList();

                    // Thêm các cột vào DataGridView
                    foreach (var session in sortedSessions)
                    {
                        if (!dgv_attendance.Columns.Contains(session.SessionID.ToString()))
                        {
                            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
                            {
                                HeaderText = session.StartTime.ToString(),
                                Name = session.SessionID.ToString()
                            };
                            dgv_attendance.Columns.Add(column);
                        }
                    }

                    // Nhóm dữ liệu theo StudentID
                    var groupedByStudent = sortedSessions.GroupBy(a => a.StudentID);

                    // Thêm các hàng vào DataGridView
                    foreach (var group in groupedByStudent)
                    {
                        var studentSessions = group.ToList();
                        var firstSession = studentSessions.First();

                        // Tạo một hàng mới với thông tin sinh viên và trạng thái điểm danh cho mỗi buổi học
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dgv_attendance);

                        // Loại bỏ dấu gạch nối trong StudentID
                        string cleanStudentID = firstSession.StudentID.Replace("-", string.Empty);
                        row.Cells[0].Value = cleanStudentID;
                        row.Cells[1].Value = firstSession.LastName;
                        row.Cells[2].Value = firstSession.FirstName;

                        // Điền trạng thái điểm danh vào các ô tương ứng
                        foreach (var session in studentSessions)
                        {
                            var columnIndex = dgv_attendance.Columns[session.SessionID.ToString()].Index;
                            row.Cells[columnIndex].Value = session.Present;
                        }

                        dgv_attendance.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up attendance: {ex.Message}");
            }
        }


        private async Task SetupRoom()
        {
            room = await _roomBLL.GetRoomsByName(roomID);
            this.Text = "Server - " + roomID + " - Số lượng máy: " + room.NumberOfComputers;
            btnAll.Text = "All:" + room.NumberOfComputers;
            btnAll.Click += (s, e) => LoadFullInfoListIntoDataGridView(fullInfoList);
            roomID = room.RoomID.ToString();
            var computers = await _computerBLL.GetComputersByID(room.RoomID.ToString());
            foreach (var computer in computers)
                InitializeStandard(computer.ID, computer.ComputerName, computer.CPU, computer.RAM, computer.HDD);
            InitializeFullInfoList(room.NumberOfComputers, room.RoomName);
            LoadFullInfoListIntoDataGridView(fullInfoList);
        }
        private void CreateGroup(string groupName, List<List<string>> selectedComputers)
        {
            if (!groups.ContainsKey(groupName))
            {
                groups[groupName] = selectedComputers;

                // Thêm nút mới vào FlowLayoutPanel
                var btnGroup = new Button
                {
                    Text = groupName,
                    Width = 100
                };

                // Tạo ContextMenuStrip cho nút
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem deleteItem = new ToolStripMenuItem("Xóa nhóm");
                contextMenu.Items.Add(deleteItem);

                // Sự kiện khi nhấn vào "Xóa nhóm"
                deleteItem.Click += (sender, e) =>
                {
                    // Xóa nhóm khỏi FlowLayoutPanel
                    flowLayoutPanel1.Controls.Remove(btnGroup);

                    // Giải phóng bộ nhớ của list liên quan đến nhóm
                    if (groups.ContainsKey(groupName))
                    {
                        // Giải phóng bộ nhớ của List<List<string>> trong groups
                        var groupList = groups[groupName];

                        // Duyệt qua từng List<string> trong nhóm và giải phóng
                        foreach (var list in groupList)
                        {
                            list.Clear(); // Giải phóng tài nguyên trong từng List<string>
                        }

                        groupList.Clear(); // Giải phóng List chính
                        groups.Remove(groupName); // Xóa nhóm khỏi dictionary
                    }

                    // Thông báo xóa thành công
                    MessageBox.Show("Nhóm đã được xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                // Gán ContextMenuStrip cho nút
                btnGroup.ContextMenuStrip = contextMenu;

                // Thêm sự kiện click vào nút
                btnGroup.Click += (s, e) => DisplayGroup(groupName);

                // Thêm nút vào FlowLayoutPanel
                flowLayoutPanel1.Controls.Add(btnGroup);
            }
            else
            {
                MessageBox.Show("Tên nhóm này đã tồn tại. Vui lòng chọn tên khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void DisplayGroup(string groupName)
        {
            if (groups.TryGetValue(groupName, out var computers))
            {
                LoadFullInfoListIntoDataGridView(computers);
            }
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            FormLoading formLoading = new FormLoading();
            formLoading.Show();
            try
            {
                this.Hide();
                imageList1.ImageSize = new Size(64, 64);
                dgv_attendance.Hide();
                lst_client.Hide();
                lst_client.LargeImageList = imageList1;
                //students = await _classStudentBLL.GetClassStudentsByID(classID);
                await SetupRoom();
                await SetupAttendance(classID);
                //await updateAttanceToDB();
                //await updateSessionComputer();
                sendAllIPInLan();
                timer = new WinFormsTimer();
                timer.Interval = 5000;
                // Gán sự kiện xảy ra khi Timer đã chạy đủ thời gian
                timer.Tick += OnTimerTick;
                this.ContextMenuStrip = contextMenuStrip;
                // Bắt đầu Timer
                timer.Start();
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                //IPAddress ip = IPAddress.Parse(Ip);
                int tcpPort = 8765;

                // Tạo đối tượng TcpListener để lắng nghe kết nối từ client
                tcpListener = new TcpListener(ip, tcpPort);
                listenThread = new Thread(new ThreadStart(ListenForClients));
                listenThread.Start();
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;

                double width80Percent = screenWidth * 0.8;


                dgv_attendance.Width = int.Parse(width80Percent.ToString());
                dgv_client.Width = int.Parse(width80Percent.ToString());
                lst_client.Width = int.Parse(width80Percent.ToString());

            }
            finally
            {
                this.Show();
                formLoading.Close();
            }
        }
        private void serverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpListener.Stop();
        }


        private void LoadFullInfoListIntoDataGridView(List<List<string>> fullInfoList)
        {
            dgv_client.Rows.Clear(); // Clear old rows before adding new ones
            int i = 0;

            foreach (List<string> infoList in fullInfoList)
            {
                if (infoList.Count == 10)
                {
                    infoList.Insert(10, "");
                }
                // Check if the information contains "không kết nối"
                bool isDisconnected = infoList.Any(value => value.Contains("không kết nối"));

                // Process values by removing ":0", ":1" and replacing "|" with newlines
                var processedInfo = infoList
                    .Select(value => value.Replace(":0", "")       // Remove ":0"
                                          .Replace(":1", "")       // Remove ":1"
                                          .Replace("|", "\n"))     // Replace "|" with "\n"
                    .ToArray();

                // Add each row to the DataGridView
                dgv_client.Rows.Add(processedInfo);

                // Check if "không kết nối" exists, then set all row text to red
                if (isDisconnected)
                {
                    for (int j = 0; j < dgv_client.Columns.Count; j++)
                    {
                        dgv_client.Rows[i].Cells[j].Style.ForeColor = Color.Red; // Red for all cells in the row
                    }
                }
                else
                {
                    // Otherwise, check for ":0" in the original data and color accordingly
                    for (int j = 0; j < dgv_client.Columns.Count; j++)
                    {
                        var originalValue = infoList[j]; // Use the original value for comparison

                        if (originalValue.Contains(":0"))
                        {
                            dgv_client.Rows[i].Cells[j].Style.ForeColor = Color.Red; // Red for cells containing ":0"
                        }
                        else
                        {
                            dgv_client.Rows[i].Cells[j].Style.ForeColor = Color.Black; // Black for other cells
                        }
                    }
                }

                i++;
            }
        }








        //Function 
        private string getIPServer()
        {
            string ip = string.Empty;
            // Lấy tất cả card mạng của máy tính
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // Kiểm tra chỉ lấy card mạng có cấu hình IP cục bộ (Local Area Network)
                if ((networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
            networkInterface.Name.Contains("Ethernet") &&
                    networkInterface.OperationalStatus == OperationalStatus.Up) || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    // Lấy danh sách địa chỉ IP của card mạng này
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipInfo in ipProperties.UnicastAddresses)
                    {
                        // Chỉ lấy địa chỉ IPv4 của mạng LAN cục bộ
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ipInfo.Address) &&
                            !ipInfo.Address.ToString().StartsWith("169.254")) // Loại bỏ các địa chỉ APIPA
                        {
                            ip = ipInfo.Address.ToString();
                            // Trả về địa chỉ IP đầu tiên tìm thấy
                            return ip;
                        }
                    }
                }
            }
            return ip;
        }



        private void sendAllIPInLan()
        {


            // Gửi tin nhắn UDP đến từng địa chỉ IP trong mạng


            IPAddress broadcastAddress = GetBroadcastAddress() ?? null;
            Console.WriteLine(broadcastAddress);

            SendUDPMessage(broadcastAddress, 11312, Ip);
            //SendUDPMessage(IPAddress.Parse("192.168.129.65"), 11312, Ip);

        }
        private void SendUDPMessage(IPAddress ipAddress, int port, String mes)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);

                string messageToSend = mes;
                byte[] data = Encoding.UTF8.GetBytes(messageToSend);

                udpClient.Send(data, data.Length, remoteEndPoint);

                //Console.WriteLine($"Sent message to {ipAddress}");
                udpClient.Close();
            }

        }
        private void OnTimerTick(object sender, EventArgs e)
        {

            // Gửi tin nhắn UDP đến từng địa chỉ IP trong mạng
            sendAllIPInLan();
            // Dừng Timer sau khi đã thực hiện công việc
            //timer.Stop();
        }
        private void ListenForClients()
        {
            tcpListener.Start();

            Console.WriteLine("Server is listening for clients...");

            while (true)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

                    // Bạn có thể xử lý kết nối client ở đây
                    HandleClient(client);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("TCP: " + ex);
                    break;
                }
            }
        }
        private void HandleClient(TcpClient tcpClient)
        {
            NetworkStream clientStream = tcpClient.GetStream();
            byte[] messageBuffer = new byte[1024];
            int bytesRead;
            string receivedMessage = "";
            while ((bytesRead = clientStream.Read(messageBuffer, 0, messageBuffer.Length)) > 0)
            {
                // Xử lý dữ liệu nhận được từ client
                receivedMessage += Encoding.UTF8.GetString(messageBuffer, 0, bytesRead);
                if (receivedMessage.Contains("Picture5s"))
                    break;
            }

            string[] tmp = receivedMessage.Split(new char[] { '-' }, 2);
            Console.WriteLine(receivedMessage);

            if (receivedMessage.StartsWith("Ready-"))
            {
                string[] parts= receivedMessage.Split('-');
                if (!StudentsAreReady.Contains(parts[1]))
                    StudentsAreReady.Add(parts[1]);
            }
            else if (receivedMessage.StartsWith("ReadyAgain"))
            {
                string[] parts = receivedMessage.Split('-');
                StudentsAreReady.Remove(parts[1]);
                if (!StudentsAreReady.Contains(parts[2]))
                    StudentsAreReady.Add(parts[2]);
            }
            else if (receivedMessage.StartsWith("answer@"))
            {
                string[] parts = receivedMessage.Split(new[] { "answer@" }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    StudentAnswer newAnswer = new StudentAnswer();
                    int indexQuest =-1;

                    string[] answer = parts[0].Split('-');
                    foreach (string item in answer)
                    {
                        string[] keyValue = item.Split(':');
                        string key = keyValue[0];
                        string value = keyValue[1];

                        switch (key)
                        {
                            case "indexQuest":
                                indexQuest = int.TryParse(value, out int indexQuestO)?indexQuestO:0;
                                break;

                            case "studentId":
                                newAnswer.StudentID = value;
                                break;

                            case "timeDoQuest":
                                newAnswer.TimeDoQuest= int.TryParse(value, out int timeDoQuestO) ? timeDoQuestO : 0;
                                break;

                            case "selectResultsId":
                                newAnswer.SelectResultsId=value.Split(',').Select(x=> int.TryParse(x, out int rsIdO) ? rsIdO : 0).ToList();
                                break;
                        }
                    }
                    if (indexQuest == -1)
                    {
                        MessageBox.Show("Định dạng thiếu vị trí câu hỏi");
                        return;
                    }
                    Tests[IndexTestReady].Quests[indexQuest].StudentAnswers.Add(newAnswer);
                    //MessageBox.Show($"Nhận câu trả lời thành công");
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            ExamForm?.NotiHaveNewAnswer(indexQuest);

                        }));
                    }
                    else
                    {
                    ExamForm?.NotiHaveNewAnswer(indexQuest);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"xử lý câu trả lời thất bại: {ex}");
                }
            }
            else if (receivedMessage.StartsWith("Picture5s"))
            {
                string[] parts = receivedMessage.Split(new string[] { "Anh-" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    string machineName = parts[0].Replace("Picture5s-", "");

                    // Tiếp tục đọc dữ liệu ảnh từ clientStream
                    using (MemoryStream ms = new MemoryStream())
                    {
                        clientStream.CopyTo(ms); // Đọc toàn bộ dữ liệu còn lại vào MemoryStream
                        byte[] imageBytes = ms.ToArray();

                        // Chuyển đổi byte[] thành Image
                        using (MemoryStream imageStream = new MemoryStream(imageBytes))
                        {
                            Image image = Image.FromStream(imageStream);

                            // Cập nhật ảnh vào ListView với tên máy
                            if (lst_client.InvokeRequired)
                            {
                                lst_client.Invoke(new Action(() =>
                                {
                                    UpdateListView(machineName, image);


                                }));
                            }
                            else
                            {
                                UpdateListView(machineName, image);


                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Nhận dữ liệu ảnh không hợp lệ");
                }
            }
            else if (tmp[0] == "InfoClient")
            {
                ReciveInfo(tmp[1]);
            }
            else if (tmp[0] == "LOAD_DONE")
            {
                isRunning = true;

                screenshotThread = new Thread(new ThreadStart(CaptureAndSendScreenshotsContinuously));
                screenshotThread.Start();
            }
            else if (tmp[0] == "ReadyToCapture")
            {
                OpenNewForm(tcpClient);
            }




            // Đóng kết nối khi client đóng kết nối
            //tcpClient.Close();
        }


        private void UpdateListView(string machineName, Image image)
        {
            // Đặt kích thước cố định cho ảnh (ví dụ: 100x100 pixel)
            int targetWidth = 256;
            int targetHeight = 125;
            Image resizedImage = ResizeImage(image, targetWidth, targetHeight);

            if (imageList1.ImageSize.Width != targetWidth || imageList1.ImageSize.Height != targetHeight)
            {
                imageList1.ImageSize = new Size(targetWidth, targetHeight);
            }
            // Tìm hoặc tạo mới một ListViewItem với tên máy
            var listViewItem = lst_client.Items.Cast<ListViewItem>()
                .FirstOrDefault(item => item.Text == machineName) ?? lst_client.Items.Add(machineName);

            // Cập nhật ảnh cho ListViewItem
            if (listViewItem.ImageIndex == -1)
            {
                imageList1.Images.Add(machineName, resizedImage);
                listViewItem.ImageIndex = imageList1.Images.IndexOfKey(machineName);
            }
            else
            {
                imageList1.Images[listViewItem.ImageIndex] = resizedImage;
            }
        }

        // Hàm ResizeImage để thay đổi kích thước ảnh
        private Image ResizeImage(Image imgToResize, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, width, height);
            }
            return b;
        }

        public void InitializeStandard(int id, string name, string cpu, string ram, string hdd)
        {
            string keysString = "ID?Tên máy?Ổ cứng?CPU?RAM?MSSV?IPC?Chuột?Bàn phím?Màn hình";
            // Chuỗi chứa các value cho standardInfoList
            string privateStandardValuesString = id + "?" + name + "?" + hdd + "?" + cpu + "? " + ram + "? ? ?không kết nối?không kết nối?không kết nối";
            // Chuỗi chứa các value cho privateStandardInfoList

            // Tách các key và value từ chuỗi
            var keys = keysString.Split('?');
            var privateStandardValues = privateStandardValuesString.Split('?');


            // Khởi tạo privateStandardInfoList
            var privateStandardInfo = new Dictionary<string, string>();
            for (int i = 0; i < keys.Length; i++)
            {
                privateStandardInfo[keys[i]] = privateStandardValues[i];
            }
            privateStandardInfoList.Add(privateStandardInfo);
        }

        public void InitializeFullInfoList(int machineCount, string roomName)
        {
            for (int i = 1; i <= machineCount; i++)
            {
                string machineName = $"{roomName}-{i:D2}";
                var newEntry = new List<string>();

                //Kiểm tra nếu máy tồn tại trong privateStandardInfoList
                var privateInfo = privateStandardInfoList.FirstOrDefault(info => info.ContainsKey("Tên máy") && info["Tên máy"] == machineName);
                newEntry.Add(privateInfo["Tên máy"]);
                newEntry.Add(privateInfo["Ổ cứng"]);
                newEntry.Add(privateInfo["CPU"]);
                newEntry.Add(privateInfo["RAM"]);
                newEntry.Add(privateInfo["MSSV"]);
                newEntry.Add(privateInfo["IPC"]);
                newEntry.Add(privateInfo["Chuột"]);
                newEntry.Add(privateInfo["Bàn phím"]);
                newEntry.Add(privateInfo["Màn hình"]);
                newEntry.Add(privateInfo["ID"]);
                newEntry.Add("");



                fullInfoList.Add(newEntry);
            }
        }


        public Dictionary<string, string> CompareInfo(Dictionary<string, string> newInfo)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, string> matchedInfo = null;
            Dictionary<string, string> mismatchInfo = new Dictionary<string, string>(); // Mới: lưu trữ tất cả sai lệch

            // Check privateStandardInfoList first
            foreach (var info in privateStandardInfoList)
            {
                if (info.ContainsKey("Tên máy") && newInfo.ContainsKey("Tên máy") && info["Tên máy"] == newInfo["Tên máy"])
                {
                    matchedInfo = info;
                    break;
                }
            }

            // So sánh từng thông tin trong newInfo
            if (matchedInfo != null)
                foreach (var key in newInfo.Keys)
                {
                    if (key == "Chuột" || key == "Bàn phím" || key == "Màn hình")
                    {
                        if (newInfo[key] == "Đã kết nối")
                        {
                            result[key] = newInfo[key] + ":1";

                        }
                        else
                        {
                            result[key] = newInfo[key] + ":0";
                            mismatchInfo[key] = key + ": Mất kết nối";


                        }
                    }
                    else if (key == "Ổ cứng")
                    {
                        // So sánh ổ cứng (Model, Interface, Size) - hỗ trợ nhiều ổ cứng
                        string[] parts = newInfo[key].Split(';');
                        List<string> newOcungModels = new List<string>();
                        List<string> newOcungInterfaces = new List<string>();
                        List<string> newOcungSizes = new List<string>();

                        // Tách thông tin ổ cứng
                        foreach (var part in parts)
                        {
                            var split = part.Split(':');
                            if (split.Length == 2)
                            {
                                if (split[0].Trim().ToLower() == "model")
                                    newOcungModels.Add(split[1].Trim());
                                else if (split[0].Trim().ToLower() == "interface")
                                    newOcungInterfaces.Add(split[1].Trim());
                                else if (split[0].Trim().ToLower() == "size")
                                    newOcungSizes.Add(split[1].Trim());
                            }
                        }

                        // So sánh từng ổ cứng
                        bool allMatching = true;
                        for (int i = 0; i < newOcungModels.Count; i++)
                        {
                            string model = newOcungModels[i];
                            string interfaceType = newOcungInterfaces[i];
                            string sizeStr = newOcungSizes[i];
                            string modelInDB = matchedInfo["OcungModel"].Trim();
                            string interfaceInDB = matchedInfo["OcungInterface"].Trim();
                            string sizeInDB = matchedInfo["OcungSize"].Trim();

                            double sizeInDBValue = Convert.ToDouble(sizeInDB.Replace(" GB", "").Trim());
                            double sizeReceivedValue = Convert.ToDouble(sizeStr.Replace(" GB", "").Trim());

                            double tolerance = 0.1 * sizeInDBValue; // Sai số cho phép là 10%
                            bool sizeMatches = Math.Abs(sizeInDBValue - sizeReceivedValue) <= tolerance;

                            if (!(model == modelInDB && interfaceType == interfaceInDB && sizeMatches))
                            {
                                allMatching = false;
                                break;
                            }
                        }

                        if (allMatching)
                        {
                            result[key] = newInfo[key] + ":1";
                        }
                        else
                        {
                            result[key] = newInfo[key] + ":0";
                            mismatchInfo["Ocung"] = "Sai lệch ổ cứng: Kích thước hoặc Model ổ cứng không khớp.";
                        }
                    }
                    else if (key == "CPU")
                    {
                        // So sánh CPU
                        string cpuReceived = newInfo[key].Trim();
                        string cpuInDB = matchedInfo["CPU"].Trim();

                        if (cpuReceived == cpuInDB)
                        {
                            result[key] = newInfo[key] + ":1";
                        }
                        else
                        {
                            result[key] = newInfo[key] + ":0";
                            mismatchInfo["CPU"] = $"Sai lệch CPU: {cpuReceived} (Expected: {cpuInDB})";
                        }
                    }
                    else if (key == "RAM")
                    {
                        // So sánh RAM - hỗ trợ nhiều RAM
                        string[] ramReceived = newInfo[key].Split('|');
                        List<string> receivedRAMs = new List<string>();

                        // Tách các phần tử RAM
                        foreach (var ram in ramReceived)
                        {
                            if (ram.Trim() != "")
                                receivedRAMs.Add(ram.Trim());
                        }

                        string[] ramInDB = matchedInfo["RAM"].Split('|');
                        List<string> dbRAMs = new List<string>(ramInDB);

                        bool allRAMMatching = true;
                        for (int i = 0; i < receivedRAMs.Count; i++)
                        {
                            if (receivedRAMs[i].Trim() != dbRAMs[i].Trim())
                            {
                                allRAMMatching = false;
                                break;
                            }
                        }

                        if (allRAMMatching)
                        {
                            result[key] = newInfo[key] + ":1";
                        }
                        else
                        {
                            result[key] = newInfo[key] + ":0";
                            mismatchInfo["RAM"] = "Sai lệch RAM: Dung lượng hoặc nhà sản xuất RAM không khớp.";
                        }
                    }
                    else
                    {
                        if (key != "Tên máy" && key != "MSSV" && key != "IPC")
                        {
                            if (matchedInfo != null && matchedInfo.ContainsKey(key) && matchedInfo[key] == newInfo[key])
                            {
                                result[key] = newInfo[key] + ":1";
                            }
                            else
                            {
                                result[key] = newInfo[key] + ":0";
                            }
                        }
                    }
                }

            // Nếu có sai lệch thì thêm vào MismatchInfo
            if (mismatchInfo.Count > 0)
            {
                result["MismatchInfo"] = string.Join("\n ", mismatchInfo.Values);
            }
            else return newInfo;
            return result;
        }


        // Hàm tách và lấy các mã số sinh viên
        private Dictionary<string, string> ExtractStudentIDs(string mssvInfo)
        {
            Dictionary<string, string> studentInfo = new Dictionary<string, string>();

            // Tách các phần theo dấu "+"
            string[] students = mssvInfo.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var student in students)
            {
                // Tìm mã số sinh viên và tên
                string[] parts = student.Split('-');
                if (parts.Length > 1)
                {
                    string studentName = parts[0].Trim();  // Lấy tên sinh viên
                    string studentID = parts[1].Trim();    // Lấy MSSV

                    // Thêm vào dictionary với key là tên và value là MSSV
                    if (!studentInfo.ContainsKey(studentName))  // Kiểm tra nếu tên sinh viên chưa có trong dictionary
                    {
                        studentInfo.Add(studentName, studentID);
                    }
                }
            }

            return studentInfo;
        }


        public void ReciveInfo(string info)
        {
            Console.WriteLine(info);

            // Phân tách chuỗi thông tin nhận được thành các phần tử riêng biệt
            string[] infC = info.Split(new string[] { "Tenmay: ", "MSSV: ", "Ocung: ", "CPU: ", "RAM: ", "IPC: ", "Chuot: ", "Banphim: ", "Manhinh: " }, StringSplitOptions.None);
            Dictionary<string, string> studentIDs = ExtractStudentIDs(infC[9]);

            // Tạo danh sách newEntry từ thông tin phân tách
            List<string> newEntry = new List<string>
    {
        infC[2],  // Tên máy
        infC[6],  // Ổ cứng
        infC[7],  // CPU
        infC[8],  // RAM
        string.Join("\n", studentIDs.Values),  // MSSV
        infC[1],  // IPC
        infC[3],  // Chuột
        infC[4],  // Bàn phím
        infC[5],   // Màn hình
        ""
    };
            // Tạo dictionary mới từ danh sách thông tin nhận được
            var newInfoDict = new Dictionary<string, string>
    {
        { "Tên máy", infC[2].Trim() },
        { "Ổ cứng", infC[6].Trim() },
        { "CPU", infC[7].Trim() },
        { "RAM", infC[8].Trim() },
        { "MSSV", string.Join("\n", studentIDs.Values) }, // Chỉ cập nhật mã số sinh viên
        { "IPC", infC[1].Trim() },
        { "Chuột", infC[3].Trim() },
        { "Bàn phím", infC[4].Trim() },
        { "Màn hình", infC[5].Trim() },
        { "MismatchInfo", "" }
    };

            // So sánh và cập nhật thông tin từ newInfoDict
            var comparedInfo = CompareInfo(newInfoDict);

            // Cập nhật fullInfoList và danh sách tóm tắt
            UpdateInfoList(newEntry, comparedInfo);
            //UpdateInfoList(newEntry, new Dictionary<string, string>());

            //// Tạo bản sao của newEntry để rút gọn thông tin
            //List<string> newEntryCopy = new List<string>(newEntry);

            //// Cập nhật thông tin vào danh sách tóm tắt
            //UpdateSummaryInfoList(newEntryCopy);

            //// Hiển thị dữ liệu trên DataGridView và so sánh
            //AddOrUpdateRowToDataGridView(newEntry);
            if (dgv_client.InvokeRequired)
            {
                dgv_client.Invoke(new Action(() =>
                {
                    LoadFullInfoListIntoDataGridView(fullInfoList);

                }));
            }
            else
            {
                LoadFullInfoListIntoDataGridView(fullInfoList);

            }


            // Cập nhật thông tin MSSV vào dgv_attendance nếu tồn tại
            if (studentIDs.Count != 0)
                UpdateAttendanceDataGridView(studentIDs, sessionID, "cm");

            Console.WriteLine("length: " + fullInfoList.Count);
        }

        private void UpdateAttendanceDataGridView(Dictionary<string, string> studentID, int sessionID, string value)
        {
            if (dgv_attendance.InvokeRequired)
            {
                // Gọi hàm trong UI thread mà không dùng async trong Invoke
                dgv_attendance.Invoke(new Action(() =>
                {
                    UpdateOrAddRowAttendance(studentID, sessionID, value).ConfigureAwait(false);  // Không dùng await ở đây
                }));
            }
            else
            {
                // Thực hiện update trực tiếp nếu đã ở UI thread
                UpdateOrAddRowAttendance(studentID, sessionID, value).ConfigureAwait(false);  // Không dùng await ở đây
            }
        }

        private async Task UpdateOrAddRowAttendance(Dictionary<string, string> studentID, int sessionID, string value)
        {
            foreach (var student in studentID)
            {
                bool studentExists = false;

                foreach (DataGridViewRow row in dgv_attendance.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == student.Value)
                    {
                        studentExists = true;

                        // Kiểm tra và thêm cột nếu cột sessionID chưa tồn tại
                        if (!dgv_attendance.Columns.Contains(sessionID.ToString()))
                        {
                            // Thêm cột mới nếu chưa có
                            dgv_attendance.Columns.Add(sessionID.ToString(), DateTime.Now.ToString("dd/MM/yyyy"));
                        }

                        // Lấy chỉ số cột dựa trên sessionID
                        int columnIndex = dgv_attendance.Columns[sessionID.ToString()].Index;
                        row.Cells[columnIndex].Value = value;
                        break;
                    }
                }

                // Nếu sinh viên chưa có trong DataGridView
                if (!studentExists)
                {
                    string[] nameParts = student.Key.Trim().Split(' ');
                    string firstName = string.Join(" ", nameParts, 0, nameParts.Length - 1);
                    string lastName = nameParts[nameParts.Length - 1];

                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(dgv_attendance);
                    newRow.Cells[0].Value = student.Value;
                    newRow.Cells[1].Value = firstName;
                    newRow.Cells[2].Value = lastName;

                    // Kiểm tra và thêm cột sessionID nếu chưa tồn tại
                    if (!dgv_attendance.Columns.Contains(sessionID.ToString()))
                    {
                        // Thêm cột mới nếu chưa có
                        dgv_attendance.Columns.Add(sessionID.ToString(), DateTime.Now.ToString("dd/MM/yyyy"));
                    }

                    // Lấy chỉ số cột dựa trên sessionID
                    int columnIndex = dgv_attendance.Columns[sessionID.ToString()].Index;
                    newRow.Cells[columnIndex].Value = value;
                    newRow.DefaultCellStyle.BackColor = Color.Red; // Đánh dấu hàng mới
                    dgv_attendance.Rows.Add(newRow);

                 
                }
            }

            dgv_attendance.Refresh();
        }

        private void dgv_client_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra chỉ số dòng và cột hợp lệ
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Lấy "Tên máy" của hàng đang được thay đổi
                string computerName = dgv_client.Rows[e.RowIndex].Cells[0].Value?.ToString(); // Cột 0 là Tên máy
                string newValue = dgv_client.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                // Cập nhật vào fullInfoList dựa trên Tên máy
                UpdateFullInfoListByComputerName(computerName, e.ColumnIndex, newValue);
            }
        }

        // Cập nhật vào fullInfoList dựa trên Tên máy
        private void UpdateFullInfoListByComputerName(string computerName, int columnIndex, string newValue)
        {
            // Tìm hàng trong fullInfoList có "Tên máy" trùng khớp
            var targetRow = fullInfoList.FirstOrDefault(row => row[0] == computerName);

            if (targetRow != null && columnIndex < targetRow.Count)
            {
                // Cập nhật giá trị ở cột tương ứng
                targetRow[columnIndex] = newValue;
            }
            else
            {
                Console.WriteLine("Computer name not found or column index out of range in fullInfoList.");
            }
        }

        private void UpdateInfoList(List<string> newEntry, Dictionary<string, string> comparedInfo)
        {
            foreach (var key in comparedInfo.Keys)
            {
                newEntry[GetIndexForKey(key)] = comparedInfo[key];
            }
            // Kiểm tra xem tên máy đã tồn tại trong danh sách hay chưa
            bool entryExists = false;
            for (int i = 0; i < fullInfoList.Count; i++)
            {

                if (fullInfoList[i][0] == newEntry[0]) // So sánh tên máy
                {
                    // Cập nhật thông tin cho danh sách đầy đủ, bỏ qua chỉ mục 9
                    for (int j = 0; j < newEntry.Count; j++)
                    {
                        if (j == 9) // Bỏ qua phần tử ở chỉ mục 9
                        {
                            fullInfoList[i][j + 1] = newEntry[j];
                            break;
                        }
                        else
                        {
                            fullInfoList[i][j] = newEntry[j];

                        }
                    }
                    entryExists = true;

                    // Nếu đang ở chế độ xem đầy đủ, cập nhật hiển thị
                    if (isFullInfoMode)
                    {
                        AddOrUpdateRowToDataGridView(newEntry);
                    }
                    break;
                }
            }

            // Nếu tên máy không tồn tại, thêm danh sách mới vào danh sách đầy đủ
            if (!entryExists)
            {
                fullInfoList.Add(newEntry);
                if (isFullInfoMode)
                {
                    AddOrUpdateRowToDataGridView(newEntry);
                }
            }


        }

        private int GetIndexForKey(string key)
        {
            switch (key)
            {
                case "Tên máy": return 0;
                case "Ổ cứng": return 1;
                case "CPU": return 2;
                case "RAM": return 3;
                case "MSSV": return 4;
                case "IPC": return 5;
                case "Chuột": return 6;
                case "Bàn phím": return 7;
                case "Màn hình": return 8;
                case "MismatchInfo": return 9;
                default: return -1;
            }
        }

        private void UpdateSummaryInfoList(List<string> newEntryCopy)
        {
            bool entryExists = false;
            for (int i = 0; i < summaryInfoList.Count; i++)
            {
                if (summaryInfoList[i][0] == newEntryCopy[0]) // So sánh tên máy
                {
                    // Cập nhật thông tin cho danh sách tóm tắt
                    summaryInfoList[i] = newEntryCopy;
                    entryExists = true;
                    break;
                }
            }

            // Nếu tên máy không tồn tại trong danh sách tóm tắt, thêm danh sách mới
            if (!entryExists)
            {
                summaryInfoList.Add(newEntryCopy);
            }
        }

        private void AddOrUpdateRowToDataGridView(List<string> entry)
        {
            if (dgv_client.InvokeRequired)
            {
                dgv_client.Invoke(new Action(() =>
                {
                    AddOrUpdateRow(entry);
                }));
            }
            else
            {
                AddOrUpdateRow(entry);
            }
        }

        public static List<(string FullName, string ID)> ExtractNamesAndIDs(string input)
        {
            // Danh sách để lưu kết quả
            var result = new List<(string FullName, string ID)>();

            // Mẫu Regex để tách họ tên và dãy số (MSSV)
            string pattern = @"([a-z\s]+)-\s*(\d+)";

            // Tìm các cặp phù hợp với mẫu
            MatchCollection matches = Regex.Matches(input.ToLower(), pattern);

            // Duyệt qua các kết quả và thêm vào danh sách
            foreach (Match match in matches)
            {
                string fullName = match.Groups[1].Value.Trim();
                string id = match.Groups[2].Value.Trim();
                result.Add((fullName, id));
            }

            return result;
        }

        private void AddOrUpdateRow(List<string> entry)
        {
            DataGridViewRow row = null;
            int rowIndex = -1;

            // Tìm hàng cần cập nhật hoặc thêm mới
            foreach (DataGridViewRow dgvRow in dgv_client.Rows)
            {
                if (entry[0].Contains(dgvRow.Cells[0].Value.ToString()))
                {
                    row = dgvRow;
                    rowIndex = dgvRow.Index;
                    break;
                }
            }

            if (row != null)
            {
                // Cập nhật dữ liệu nếu dòng đã tồn tại
                for (int i = 0; i < entry.Count; i++)
                {
                    int indexOf = entry[i].LastIndexOf(":");
                    indexOf = indexOf == -1 ? entry[i].Length : indexOf;
                    string newValue = entry[i].Substring(0, indexOf);
                    string tmp = entry[i].Substring(indexOf);

                    // Đối với thông tin RAM, sử dụng Environment.NewLine để xuống dòng
                    if (i == 3) // Ở đây giả sử cột RAM là cột thứ 3 (index là 2)
                    {
                        row.Cells[i].Value = newValue.Replace("\n", Environment.NewLine);
                    }
                    else if (i == 4)
                    {
                        var nameAndIDs = ExtractNamesAndIDs(newValue);
                        string lstID = "";
                        foreach (var id in nameAndIDs)
                        {
                            lstID += id + Environment.NewLine;
                        }
                        row.Cells[i].Value = lstID;

                    }
                    else
                    {
                        row.Cells[i].Value = newValue;
                    }

                    // Cập nhật màu sắc và lưu trạng thái màu
                    if (tmp.Contains("0"))
                    {
                        row.Cells[i].Style.ForeColor = Color.Red;
                    }
                    else if (tmp.Contains("1"))
                    {
                        row.Cells[i].Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        row.Cells[i].Style.ForeColor = Color.Red;

                    }
                }
            }
            else
            {
                // Thêm dòng mới nếu không tồn tại
                rowIndex = dgv_client.Rows.Add(entry.ToArray());
                DataGridViewRow newRow = dgv_client.Rows[rowIndex];

                // Kiểm tra và tô màu các ô của dòng mới
                for (int i = 0; i < entry.Count; i++)
                {
                    int indexOf = entry[i].LastIndexOf(":");
                    indexOf = indexOf == -1 ? entry[i].Length : indexOf;
                    string newValue = entry[i].Substring(0, indexOf);
                    string tmp = entry[i].Substring(indexOf);

                    // Đối với thông tin RAM, sử dụng Environment.NewLine để xuống dòng
                    if (i == 3) // Ở đây giả sử cột RAM là cột thứ 3 (index là 2)
                    {
                        newRow.Cells[i].Value = newValue.Replace("\n", Environment.NewLine);
                    }
                    else if (i == 4)
                    {
                        var nameAndIDs = ExtractNamesAndIDs(newValue);
                        string lstID = "";
                        foreach (var id in nameAndIDs)
                        {
                            lstID += id.ID + Environment.NewLine;
                        }
                        newRow.Cells[i].Value = lstID;

                    }
                    else
                    {
                        newRow.Cells[i].Value = newValue;
                    }

                    // Cập nhật màu sắc và lưu trạng thái màu
                    if (tmp.Contains("0"))
                    {
                        newRow.Cells[i].Style.ForeColor = Color.Red;
                    }
                    else
                    {
                        newRow.Cells[i].Style.ForeColor = Color.Black;
                    }
                }
            }

            dgv_client.Refresh();
        }

        private void StopSlideShow_Click(object sender, EventArgs e)
        {
            string message = "CloseSlideShow";
            byte[] data = Encoding.UTF8.GetBytes(message);

            List<Thread> clientThreads = new List<Thread>();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    string clientIP = "";
                    if (row.Cells[5].Value.ToString() != null)
                        clientIP = row.Cells[5].Value.ToString();

                    // Kiểm tra xem địa chỉ IP có hợp lệ không
                    if (IsValidIPAddress(clientIP))
                    {
                        // Tạo một luồng riêng biệt cho mỗi client
                        Thread clientThread = new Thread(() =>
                        {
                            try
                            {
                                TcpClient client = new TcpClient(clientIP, 8888);

                                // Gửi yêu cầu khóa tới máy Client
                                NetworkStream stream = client.GetStream();
                                stream.Write(data, 0, data.Length);
                                // Đóng kết nối
                                client.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                            }
                        });

                        // Bắt đầu luồng cho client hiện tại
                        clientThreads.Add(clientThread);
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }

            isRunning = false;
        }

        private void CaptureAndSendScreenshotsContinuously()
        {
            UdpClient udpClient = new UdpClient(); // Tạo UDP client
            int udpBufferSize = udpClient.Client.SendBufferSize;

            // Lấy địa chỉ broadcast của mạng local
            IPAddress broadcastAddress = GetBroadcastAddress();
            int i = 1;
            int fps = 60;
            while (isRunning)
            {
                try
                {
                    // Lấy kích thước của vùng hiển thị thực sự của màn hình
                    int screenLeft = SystemInformation.VirtualScreen.Left;
                    int screenTop = SystemInformation.VirtualScreen.Top;
                    int screenWidth = SystemInformation.VirtualScreen.Width;
                    int screenHeight = SystemInformation.VirtualScreen.Height;

                    using (Bitmap screenshot = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(screenshot))
                        {
                            graphics.CopyFromScreen(screenLeft, screenTop, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy);

                            // Draw the cursor
                            DrawCursorOnScreenshot(graphics);

                            // Compress and send the screenshot
                            using (MemoryStream stream = new MemoryStream())
                            {
                                CompressAndSendImage(i, screenshot, stream, udpBufferSize, udpClient, broadcastAddress);
                            }
                        }
                    }
                    if (i > fps - 1) i = 1;
                    i++;
                    // Chờ khoảng thời gian trước khi chụp và gửi tiếp
                    Thread.Sleep(1000 / fps); // Chờ 1/fps giây (mili giây) trước khi gửi hình tiếp theo
                }
                catch (ThreadInterruptedException ex)
                {
                    // Xử lý ngoại lệ ở đây
                    Console.WriteLine("Thread interrupted: " + ex.Message);
                }
            }
        }

        private void CompressAndSendImage(int i, Bitmap image, MemoryStream stream, int bufferSize, UdpClient udpClient, IPAddress broadcastAddress)
        {
            long quality = 100L; // Bắt đầu với chất lượng 90%

            byte[] imageData;
            do
            {
                stream.SetLength(0); // Đặt lại stream cho mỗi lần nén
                SaveJpeg(stream, image, quality);
                imageData = stream.ToArray();
                quality -= 10; // Giảm chất lượng đi 10% cho lần tiếp theo nếu cần
            } while (imageData.Length > bufferSize && quality > 10);

            int bytesSent = 0;
            int index = 0;
            int cutCount = (imageData.Length / (bufferSize - 100)) + 1;
            int cutCounter = 1;
            while (bytesSent < imageData.Length)
            {
                string str = $"{i:D4}{cutCount:D4}{cutCounter:D4}+";
                byte[] signal = Encoding.UTF8.GetBytes(str);
                int signalLength = signal.Length;

                int remainingBytes = imageData.Length - bytesSent;
                int bytesToSend = Math.Min(bufferSize - signalLength - 100, remainingBytes); // Đảm bảo có chỗ cho tín hiệu

                // Tạo một mảng byte mới để chứa tín hiệu và dữ liệu cần gửi
                byte[] dataToSend = new byte[signalLength + bytesToSend];
                Console.WriteLine(imageData.Length + " " + cutCount + " " + i + " " + quality);
                // Sao chép tín hiệu và dữ liệu vào mảng mới
                Array.Copy(signal, 0, dataToSend, 0, signalLength);
                Array.Copy(imageData, index, dataToSend, signalLength, bytesToSend);

                // Gửi dữ liệu bao gồm tín hiệu
                udpClient.Send(dataToSend, dataToSend.Length, new IPEndPoint(broadcastAddress, 8889)); // Gửi dữ liệu qua UDP

                // Cập nhật số byte đã gửi và vị trí index
                bytesSent += bytesToSend;
                index += bytesToSend;
                cutCounter++;
            }
        }

        private void SaveJpeg(Stream stream, Bitmap image, long quality)
        {
            var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            var jpegCodec = GetEncoder(ImageFormat.Jpeg);
            if (jpegCodec == null)
                return;

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            image.Save(stream, jpegCodec, encoderParams);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void DrawCursorOnScreenshot(Graphics graphics)
        {
            // Get the cursor position
            Point cursorPosition = Cursor.Position;

            // Adjust for the screen bounds
            cursorPosition.X -= SystemInformation.VirtualScreen.Left;
            cursorPosition.Y -= SystemInformation.VirtualScreen.Top;

            // Draw the cursor on the screenshot
            Cursor cursor = Cursors.Default;
            cursor.Draw(graphics, new System.Drawing.Rectangle(cursorPosition, cursor.Size));
        }

        // Hàm để lấy địa chỉ broadcast của mạng local
        private IPAddress GetBroadcastAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
            ni.Name.Contains("Ethernet") &&
                    ni.OperationalStatus == OperationalStatus.Up) || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            var subnetMask = ip.IPv4Mask;
                            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();
                            byte[] ipAddressBytes = ip.Address.GetAddressBytes();

                            if (subnetMaskBytes.Length == 4 && ipAddressBytes.Length == 4)
                            {
                                byte[] broadcastAddressBytes = new byte[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    broadcastAddressBytes[i] = (byte)(ipAddressBytes[i] | (subnetMaskBytes[i] ^ 255));
                                }
                                return new IPAddress(broadcastAddressBytes);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private bool IsValidIPAddress(string ipAddress)
        {
            IPAddress tempIPAddress;
            return IPAddress.TryParse(ipAddress, out tempIPAddress);
        }

        private void Lock_Click(object sender, EventArgs e)
        {

        }

        private void SendLockRequestToClient(string ipAddress)
        {
            // Tạo kết nối TCP tới máy Client
            TcpClient client = new TcpClient(ipAddress, 8888);

            // Gửi yêu cầu khóa tới máy Client
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes("LOCK_ACCESS");
            stream.Write(data, 0, data.Length);
            // Đóng kết nối
            //stream.Close();
            client.Close();
        }

        private async void tsUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                await updateAttanceToDB();
                await updateSessionComputer();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private async Task updateSessionComputer()
        {
            sessionComputers.Clear();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                // Lấy giá trị StudentID từ cột
                string studentIDValue = row.Cells[4].Value?.ToString() == "" ? students[0].Student.StudentID : row.Cells[4].Value?.ToString();

                // Tách StudentID nếu có dấu xuống dòng
                var studentIDs = studentIDValue.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var studentID in studentIDs)
                {
                    SessionComputer sessionComputer = new SessionComputer
                    {
                        ComputerName = row.Cells[0].Value?.ToString(),
                        HDD = row.Cells[1].Value?.ToString(),
                        CPU = row.Cells[2].Value?.ToString(),
                        RAM = row.Cells[3].Value?.ToString(),
                        StudentID = studentID.Trim(),
                        MouseConnected = row.Cells[6].Value?.ToString().ToLower() == "đã kết nối",
                        KeyboardConnected = row.Cells[7].Value?.ToString().ToLower() == "đã kết nối",
                        MonitorConnected = row.Cells[8].Value?.ToString().ToLower() == "đã kết nối",
                        ComputerID = int.Parse(row.Cells[9].Value?.ToString()),
                        SessionID = sessionID,
                        MismatchInfo = row.Cells[10].Value?.ToString()
                        // Add more properties as needed corresponding to the columns in your DataGridView
                    };

                    if (sessionComputer.StudentID != "")
                    {
                        if (selectedStudent != "")
                        {
                            sessionComputer.StudentID = selectedStudent;
                            sessionComputers.Add(sessionComputer);
                        }
                    }
                }
            }

            if (sessionComputers.Count != numbercomputer)
            {
                await _sessionComputerBLL.InsertSessionComputer(sessionID, sessionComputers);
            }
            else
            {
                MessageBox.Show("Chọn một sinh viên từ bảng điểm danh để kiểm tra máy!!");
            }
        }

        private async Task updateAttanceToDB()
        {
            List<Attendance> lstAttendances = new List<Attendance>();

            foreach (DataGridViewRow row in dgv_attendance.Rows)
            {
                string studentID = row.Cells["MSSV"].Value?.ToString();

                foreach (DataGridViewColumn col in dgv_attendance.Columns)
                {
                    // Kiểm tra xem cột hiện tại có tên là SessionID hay không
                    if (col.Index > 2)
                    {
                        string present = row.Cells[col.Index].Value?.ToString().ToLower();

                        Attendance attendance = new Attendance
                        {
                            StudentID = studentID,
                            SessionID = sessionID,
                            Present = present,
                        };

                        lstAttendances.Add(attendance);
                        break; // Thoát khỏi vòng lặp cột sau khi tìm thấy cột SessionID
                    }
                }
            }

            // Gọi BLL để thực hiện lưu danh sách Attendance vào cơ sở dữ liệu
            await _attendanceBLL.InsertAttendance(sessionID, lstAttendances);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            isFullInfoMode = !isFullInfoMode;

            List<List<string>> targetList = isFullInfoMode ? fullInfoList : summaryInfoList;
            foreach (List<string> entry in targetList)
            {
                AddOrUpdateRow(entry);
            }
        }
        private void LogElementFromList(List<List<string>> myList, int rowIndex, int colIndex)
        {
            if (rowIndex < 0 || rowIndex >= myList.Count)
            {
                Console.WriteLine("Index hàng nằm ngoài phạm vi của danh sách.");
                return;
            }

            List<string> row = myList[rowIndex];
            if (colIndex < 0 || colIndex >= row.Count)
            {
                Console.WriteLine("Index cột nằm ngoài phạm vi của hàng này.");
                return;
            }

            string element = row[colIndex];
            Console.WriteLine("Phần tử ở hàng {0}, cột {1} là: {2}", rowIndex, colIndex, element);
        }

        public object JsonConvert { get; private set; }

        private void dgv_client_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine("Chuot phai ne");
                // Xác định vị trí của chuột trên ListView
                Point clickPoint = dgv_client.PointToClient(new Point(e.X, e.Y));

                // Hiển thị ContextMenuStrip tại vị trí của chuột
                contextMenuStrip.Show(dgv_client, e.Location);
            }
        }

        private void dgv_attendance_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine("Chuot phai ne");
                // Xác định vị trí của chuột trên ListView
                Point clickPoint = dgv_attendance.PointToClient(new Point(e.X, e.Y));

                // Hiển thị ContextMenuStrip tại vị trí của chuột
                contextMenuStrip1.Show(dgv_client, e.Location);
            }
        }

        private void dgv_client_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SlideShowClick(object sender, EventArgs e)
        {
            List<Thread> clientThreads = new List<Thread>();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    if (row.Cells[5].Value != null && row.Cells[5].Value.ToString().Trim() != "")
                    {
                        string clientIP = row.Cells[5].Value.ToString();
                        Console.WriteLine(clientIP);

                        // Kiểm tra xem địa chỉ IP có hợp lệ không
                        if (IsValidIPAddress(clientIP))
                        {
                            // Kiểm tra nếu dòng này đang được chọn
                            string message = row.Selected ? "SlideShowToClient" : "SlideShow";
                            byte[] data = Encoding.UTF8.GetBytes(message);

                            // Tạo một luồng riêng biệt cho mỗi client
                            Thread clientThread = new Thread(() =>
                            {
                                try
                                {
                                    TcpClient client = new TcpClient(clientIP, 8888);

                                    // Gửi yêu cầu khóa tới máy Client
                                    NetworkStream stream = client.GetStream();
                                    stream.Write(data, 0, data.Length);
                                    // Đóng kết nối
                                    client.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                                }
                            });

                            // Bắt đầu luồng cho client hiện tại
                            clientThreads.Add(clientThread);
                            clientThread.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }
        }

        private void sendFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendForm sendForm = new SendForm();
            sendForm.FilesSelected += SendFileForm_FilePathSelected;
            sendForm.Show();
        }
        private void SendQuickLauch(string PathQC)
        {
            // Xử lý từng đường dẫn tệp trong danh sách

            List<Thread> clientThreads = new List<Thread>();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    string clientIP = row.Cells[5].Value.ToString();

                    // Kiểm tra xem địa chỉ IP có hợp lệ không
                    if (IsValidIPAddress(clientIP))
                    {
                        // Tạo một luồng riêng biệt cho mỗi client
                        Thread clientThread = new Thread(() =>
                        {
                            TcpClient client = null;
                            NetworkStream stream = null;
                            try
                            {
                                client = new TcpClient(clientIP, 8888);
                                stream = client.GetStream();

                                // Gửi tín hiệu thông báo
                                string fileName = System.IO.Path.GetFileName(PathQC);
                                string signal = $"QuickLaunch-{PathQC}";
                                byte[] signalBytes = Encoding.UTF8.GetBytes(signal);
                                stream.Write(signalBytes, 0, signalBytes.Length); // Gửi tín hiệu
                                stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức
                                Console.WriteLine("Đã gửi tín hiệu send");

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi khi gửi tệp: " + ex.Message);
                            }
                            finally
                            {
                                // Đảm bảo rằng kết nối được đóng đúng cách
                                if (stream != null) stream.Close();
                                if (client != null) client.Close();
                            }
                        });

                        // Bắt đầu luồng cho client hiện tại
                        clientThreads.Add(clientThread);
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }
        }
        private void SendFileForm_FilePathSelected(List<string> filePaths, string toPath)
        {
            foreach (string filePath in filePaths)
            {
                // Xử lý từng đường dẫn tệp trong danh sách

                List<Thread> clientThreads = new List<Thread>();

                foreach (DataGridViewRow row in dgv_client.Rows)
                {
                    try
                    {
                        string clientIP = row.Cells[5].Value.ToString();

                        // Kiểm tra xem địa chỉ IP có hợp lệ không
                        if (IsValidIPAddress(clientIP))
                        {
                            // Tạo một luồng riêng biệt cho mỗi client
                            Thread clientThread = new Thread(() =>
                            {
                                TcpClient client = null;
                                NetworkStream stream = null;
                                try
                                {
                                    client = new TcpClient(clientIP, 8888);
                                    stream = client.GetStream();

                                    // Gửi tín hiệu thông báo
                                    string fileName = System.IO.Path.GetFileName(filePath);

                                    string signal = $"SendFile-{fileName}-{toPath}";
                                    byte[] signalBytes = Encoding.UTF8.GetBytes(signal);
                                    stream.Write(signalBytes, 0, signalBytes.Length); // Gửi tín hiệu
                                    stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức
                                    Console.WriteLine("Đã gửi tín hiệu send");

                                    // Đợi client xác nhận đã nhận tín hiệu
                                    byte[] ackBytes = new byte[4];
                                    stream.Read(ackBytes, 0, 4);
                                    int ack = BitConverter.ToInt32(ackBytes, 0);
                                    if (ack != 1)
                                    {
                                        Console.WriteLine("Client không xác nhận tín hiệu.");
                                    }

                                    // Gửi nội dung tệp
                                    byte[] fileBytes = File.ReadAllBytes(filePath);
                                    stream.Write(BitConverter.GetBytes(fileBytes.Length), 0, 4); // Gửi độ dài nội dung tệp
                                    stream.Write(fileBytes, 0, fileBytes.Length); // Gửi nội dung tệp
                                    stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức

                                    Console.WriteLine("Tệp đã được gửi thành công.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Lỗi khi gửi tệp: " + ex.Message);
                                }
                                finally
                                {
                                    // Đảm bảo rằng kết nối được đóng đúng cách
                                    if (stream != null) stream.Close();
                                    if (client != null) client.Close();
                                }
                            });

                            // Bắt đầu luồng cho client hiện tại
                            clientThreads.Add(clientThread);
                            clientThread.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                    }
                }

                // Chờ tất cả các luồng kết thúc trước khi tiếp tục
                foreach (Thread t in clientThreads)
                {
                    t.Join();
                }
            }
        }
        private void OpenNewForm(TcpClient tcpclient)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { OpenNewForm(tcpclient); });
            }
            else
            {
                if (form1 == null)
                {
                    form1 = new SlideShowForm();
                    form1.Show();
                }
            }
        }
        private void reciveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReciveForm sendForm = new ReciveForm();
            sendForm.FilePathSelected += ReciveFileForm_FilePathSelected;
            sendForm.Show();
        }

        private void ReciveFileForm_FilePathSelected(string filePath, string folderPath, string folderToSavePath, bool check)
        {
            // Xử lý đường dẫn tệp nhận được từ FormSendFile
            Console.WriteLine("Đường dẫn tệp nhận được: " + filePath);
            Console.WriteLine("Đường dẫn tệp đến: " + folderPath);
            Console.WriteLine("Đường dẫn tệp đến: " + folderToSavePath);

            List<Thread> clientThreads = new List<Thread>();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    if (row.Cells[5].Value != null)
                    {
                        string clientIP = row.Cells[5].Value.ToString();

                        // Kiểm tra xem địa chỉ IP có hợp lệ không
                        if (IsValidIPAddress(clientIP))
                        {
                            // Tạo một luồng riêng biệt cho mỗi client
                            Thread clientThread = new Thread(() =>
                            {
                                TcpClient client = null;
                                NetworkStream stream = null;
                                try
                                {
                                    client = new TcpClient(clientIP, 8888);
                                    stream = client.GetStream();

                                    // Gửi tín hiệu thông báo
                                    string fileName = System.IO.Path.GetFileName(filePath);
                                    string signal = $"CollectFile-{fileName}-{folderPath}-{check}";
                                    byte[] signalBytes = Encoding.UTF8.GetBytes(signal);
                                    stream.Write(signalBytes, 0, signalBytes.Length); // Gửi tín hiệu
                                    stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức
                                    Console.WriteLine("Đã gửi tín hiệu send");

                                    using (BinaryReader reader = new BinaryReader(stream))
                                    {
                                        try
                                        {
                                            // Đọc độ dài của tên file zip
                                            int fileNameLength = reader.ReadInt32();
                                            // Đọc tên file zip
                                            string receivedFileName = Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));

                                            // Đọc độ dài của tệp zip
                                            int fileLength = reader.ReadInt32();
                                            // Đọc dữ liệu tệp zip
                                            byte[] fileBytes = reader.ReadBytes(fileLength);

                                            // Lưu file zip vào đường dẫn tạm thời
                                            string tempZipPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), receivedFileName);
                                            File.WriteAllBytes(tempZipPath, fileBytes);

                                            // Giải nén file zip
                                            ZipFile.ExtractToDirectory(tempZipPath, folderToSavePath);

                                            // Xóa file zip tạm thời
                                            File.Delete(tempZipPath);

                                            MessageBox.Show("Các tệp đã được nhận và lưu thành công tại: " + folderToSavePath);
                                        }
                                        catch (EndOfStreamException eosEx)
                                        {
                                            Console.WriteLine("Lỗi khi nhận tệp: " + eosEx.Message);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Lỗi khi nhận tệp: " + ex.Message);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Lỗi khi nhận tệp: " + ex.Message);
                                }
                                finally
                                {
                                    // Đảm bảo rằng kết nối được đóng đúng cách
                                    if (stream != null) stream.Close();
                                    if (client != null) client.Close();
                                }
                            });

                            // Bắt đầu luồng cho client hiện tại
                            clientThreads.Add(clientThread);
                            clientThread.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }
        }

        private void sendWork_ButtonClick(object sender, EventArgs e)
        {
            SendForm sendForm = new SendForm();
            sendForm.FilesSelected += SendFileForm_FilePathSelected;
            sendForm.ShowDialog();
        }


        private void ExportToExcel(string fileName)
        {
            try
            {
                // Thiết lập LicenseContext
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                FileInfo newFile = new FileInfo(fileName);
                if (newFile.Exists)
                {
                    newFile.Delete();  // Xóa tệp đã tồn tại    
                    newFile = new FileInfo(fileName);
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PhongMayInfo");

                    // Đặt tiêu đề cho các cột
                    for (int i = 0; i < dgv_client.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = dgv_client.Columns[i].HeaderText;
                    }

                    // Đổ dữ liệu từ DataGridView vào tệp Excel và kiểm tra sự khác biệt
                    for (int i = 0; i < dgv_client.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgv_client.Columns.Count; j++)
                        {
                            var cell = worksheet.Cells[i + 2, j + 1];
                            cell.Value = dgv_client.Rows[i].Cells[j].Value?.ToString() ?? "";

                            // Kiểm tra nếu ô có màu đỏ thì tô màu trong Excel
                            if (dgv_client.Rows[i].Cells[j].Style.ForeColor == Color.Red)
                            {
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(Color.Red);
                            }
                        }
                    }

                    package.Save();
                }

                Console.WriteLine("Exported data to Excel successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while exporting data to Excel: " + ex.Message);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            string thongTinMayTinh = "InfoClient-IPC: 192.168.1.40Tenmay: F71-01Chuot: Ðã k?t n?iBanphim: Ðã k?t n?iManhinh: Ðã k?t n?iOcung: Model: INTEL SSDPEKNU512GZInterface: SCSISize: 476 GBCPU: AMD Ryzen 71 4800H with Radeon Graphics         RAM: Capacity: 8 GB, Manufacturer: Micron Technology|MSSV: nguyen tan tai - 0306211189 + truong tang chi vinh - 0306211215 +";
            ReciveInfo(thongTinMayTinh);
        }

        private void Export_Click(object sender, EventArgs e)
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
                    ExportToExcel(folderPath + @"\F711-Info.xlsx");
                }
            }
        }

        private void svForm_Click(object sender, EventArgs e)
        {


        }



        private void tsCreateExam_Click(object sender, EventArgs e)
        {
            ExamForm = new SvExamForm(Tests, SendTest, StudentsAreReady, DidExamId);
            ExamForm.ShowDialog();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            dgv_client.Hide();
            lst_client.Hide();
            dgv_attendance.Show();
        }


        private void CheckAndCreateDirectoryAndFile(string directoryPath)
        {
            // Kiểm tra và tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        private void SendTest(string content, string key, int indexTest=-1)
        {
            IndexTestReady = indexTest!=-1?indexTest:IndexTestReady;
            if (key == "DoExam")
            {
                if (StudentsAreReady.Count < 0)
                {
                    MessageBox.Show("Chưa có sinh viên nào sẵn sàng!");
                    return ;
                }
            }

            List<Thread> clientThreads = new List<Thread>();

            // Tạo luồng để gửi dữ liệu đến từng client
            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    string clientIP = row.Cells[5].Value.ToString();

                    // Kiểm tra xem địa chỉ IP có hợp lệ không
                    if (IsValidIPAddress(clientIP))
                    {
                        // Tạo một luồng riêng biệt cho mỗi client
                        Thread clientThread = new Thread(() =>
                        {
                            TcpClient client = null;
                            NetworkStream stream = null;
                            try
                            {
                                // Tạo kết nối đến client
                                client = new TcpClient(clientIP, 8888);
                                client.SendBufferSize = 1024 * 1024; // Thiết lập kích thước buffer lớn (1 MB)
                                stream = client.GetStream();

                                // Chuẩn bị dữ liệu để gửi
                              //  string content = Tests[indexTest]?.GetTestString() ?? "";

                               

                                //content = "Key-Exam" + content;
                                content = key + content;

                                byte[] signalBytes = Encoding.UTF8.GetBytes(content);
                                int bufferSize = client.SendBufferSize;
                                int bytesSent = 0;

                                // Gửi dữ liệu theo từng phần nếu quá lớn
                                while (bytesSent < signalBytes.Length)
                                {
                                    int bytesToSend = Math.Min(bufferSize, signalBytes.Length - bytesSent);
                                    stream.Write(signalBytes, bytesSent, bytesToSend);
                                    bytesSent += bytesToSend;
                                }

                                stream.Flush();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi khi gửi thông tin đến {0}: {1}", clientIP, ex.Message);
                            }
                            finally
                            {
                                // Đảm bảo đóng kết nối sau khi gửi xong
                                stream?.Close();
                                client?.Close();
                            }
                        });

                        // Bắt đầu luồng cho client hiện tại
                        clientThreads.Add(clientThread);
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                    Console.WriteLine("Mất kết nối: " + ex.Message);
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }
            if(key== "Key-Exam")
            {
                MessageBox.Show("Đã phát đề");
            }
        }

        private void tsQuickLauch_Click(object sender, EventArgs e)
        {
            QuickLaunchForm sendForm = new QuickLaunchForm();
            sendForm.FilesSelected += SendQuickLauch;
            sendForm.Show();
        }

        private void tsUpdate_ButtonClick(object sender, EventArgs e)
        {

        }

        private async void tsUpdateInfoSession_Click(object sender, EventArgs e)
        {
            await updateSessionComputer();
        }

        private async void tsUpdateAttendance_Click(object sender, EventArgs e)
        {
            await updateAttanceToDB();
        }

        //private async void tsUpdateScore_Click(object sender, EventArgs e)
        //{
        //    List<Submission> submissions = new List<Submission>();

        //    foreach (DataGridViewRow row in dgv_attendance.Rows)
        //    {
        //        if (!row.IsNewRow)
        //        {
        //            Submission submission = new Submission();
        //            string value = row.Cells[0].Value.ToString();
        //            double cr = _answer.ProcessPoint(value, submission);
        //            if (cr == -1)
        //                continue;
        //            submission.StudentID = value;
        //            submission.Score = cr;
        //            submission.SessionID = sessionID;
        //            submissions.Add(submission);
        //            UpdateAttendanceDataGridView(value, sessionID, cr.ToString());
        //        }
        //    }
        //    await _answer.InsertAnswer(sessionID, submissions);
        //}   


        private async void tsUpdateBoth_Click(object sender, EventArgs e)
        {
            await updateSessionComputer();
            await updateAttanceToDB();
        }

        private void tsGroup_Click(object sender, EventArgs e)
        {
            GroupForm groupForm = new GroupForm(fullInfoList);

            groupForm.GroupCreated += (groupName, selectedComputers) =>
            {
                CreateGroup(groupName, selectedComputers);
            };

            groupForm.Show();
        }

        private void SendSlideShowSignalToClient(DataGridViewRow row)
        {
            try
            {
                // Lấy địa chỉ IP từ cột thứ 5 (Cells[5]) trong dòng được chọn
                string clientIP = row.Cells[5].Value?.ToString();

                // Kiểm tra tính hợp lệ của địa chỉ IP
                if (!string.IsNullOrEmpty(clientIP) && IsValidIPAddress(clientIP))
                {
                    // Chuẩn bị dữ liệu để gửi tín hiệu "SlideShowToClient"
                    string message = "SlideShowToClient";
                    byte[] data = Encoding.UTF8.GetBytes(message);

                    // Tạo một luồng riêng để gửi tín hiệu tới máy client
                    Thread clientThread = new Thread(() =>
                    {
                        try
                        {
                            // Tạo kết nối TCP tới máy client
                            TcpClient client = new TcpClient(clientIP, 8888);

                            // Gửi dữ liệu qua NetworkStream
                            NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length);

                            // Đóng kết nối
                            client.Close();
                        }
                        catch (Exception ex)
                        {
                            // Xử lý lỗi nếu mất kết nối
                            MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value?.ToString());
                        }
                    });

                    // Khởi động luồng để gửi tín hiệu tới máy client
                    clientThread.Start();
                }
                else
                {
                    MessageBox.Show("Địa chỉ IP không hợp lệ hoặc không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void contextMenu_SlideShowToClient_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn không
            if (dgv_client.SelectedRows.Count > 0)
            {
                // Gọi hàm để gửi tín hiệu SlideShowToClient cho dòng được chọn
                SendSlideShowSignalToClient(dgv_client.SelectedRows[0]);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để gửi tín hiệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private async void EndClassToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            await updateAttanceToDB();
            await updateSessionComputer();
            this.Close();
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            dgv_client.Show();
            dgv_attendance.Hide();
            lst_client.Hide();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            dgv_client.Hide();
            dgv_attendance.Hide();
            lst_client.Show();
        }

        private void tsUpdate_ButtonClick_1(object sender, EventArgs e)
        {

        }

        private void tsRandom_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem dgv_client có dòng nào hay không
            if (dgv_client.Rows.Count > 0)
            {
                // Khởi tạo đối tượng Random để tạo số ngẫu nhiên
                Random rand = new Random();

                // Lấy số dòng ngẫu nhiên từ 0 đến số dòng hiện tại (trừ 1)
                int randomIndex = rand.Next(0, dgv_client.Rows.Count);

                // Chọn dòng ngẫu nhiên
                dgv_client.ClearSelection(); // Hủy bỏ việc chọn các dòng trước đó
                dgv_client.Rows[randomIndex].Selected = true;

                // (Tuỳ chọn) Di chuyển đến dòng được chọn trong DataGridView
                dgv_client.FirstDisplayedScrollingRowIndex = randomIndex;
            }
            else
            {
                MessageBox.Show("Không có dữ liệu!");
            }
        }

        private void tsLock_Click(object sender, EventArgs e)
        {
            List<Thread> clientThreads = new List<Thread>();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    string clientIP = row.Cells[5].Value.ToString();

                    // Kiểm tra xem địa chỉ IP có hợp lệ không
                    if (IsValidIPAddress(clientIP))
                    {
                        // Tạo một luồng riêng biệt cho mỗi client
                        Thread clientThread = new Thread(() =>
                        {
                            TcpClient client = null;
                            NetworkStream stream = null;
                            try
                            {
                                client = new TcpClient(clientIP, 8888);
                                stream = client.GetStream();

                                // Gửi tín hiệu thông báo
                                string signal = $"LockScreen";
                                byte[] signalBytes = Encoding.UTF8.GetBytes(signal);
                                stream.Write(signalBytes, 0, signalBytes.Length); // Gửi tín hiệu
                                stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức
                                Console.WriteLine("Đã gửi tín hiệu send");

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi khi gửi tệp: " + ex.Message);
                            }
                            finally
                            {
                                // Đảm bảo rằng kết nối được đóng đúng cách
                                if (stream != null) stream.Close();
                                if (client != null) client.Close();
                            }
                        });

                        // Bắt đầu luồng cho client hiện tại
                        clientThreads.Add(clientThread);
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }
        }

     
    }
}