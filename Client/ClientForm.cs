using System;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using WindowsInput.Native;
using WindowsInput;
using DAL.Models;
using System.Diagnostics;
using System.Collections;

namespace testUdpTcp
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
            myIp = getIPServer();
            //myIp = getIPServer();
            //myIp = "192.168.72.228";
            inf = GetDeviceInfo();

            foreach (var ip in inf)
            {
                Console.Write(ip.ToString());
            }


        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        // Constants for mouse events
        private const uint MOUSEEVENTF_LEFTDOWN = 0x00000002;
        private const uint MOUSEEVENTF_LEFTUP = 0x00000004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x00000008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x00000010;
        private const uint MOUSEEVENTF_WHEEL = 0x00000800;
        private UdpClient udpClient;
        private ExamForm examFrm;
        SlideShowForm form1;
        LockScreenForm LockScreen;
        //private string IpServer = "192.168.72.249";
        private string IpServer = "127.0.0.1";

        private string myIp = "127.0.0.1";
        private List<string> inf;
        private List<string> mssvLst = new List<string>();
        private bool sended = false;
        private string hostName;
        TcpListener listener;
        string mssvDoTest { get; set; }
        private bool isRunningscreenshot = true;
        private bool isRunningscreenshot5s = true;
        private bool isRunningtcplisten = true;
        private bool isRunningudplisten = true;
        private Thread screenshotThread;
        private Thread screenshotThread5s;
        private Thread listenThread;
        private Thread udpReceiverThread;

        // Khai báo danh sách để lưu MSSV đã nhập
        List<string> mssvList = new List<string>();

        private Test Test { get; set; }
        Form WaitingFrom { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {

            udpClient = new UdpClient(11312);
            isRunningudplisten = true;
            udpReceiverThread = new Thread(ReceiveDataOnce);
            udpReceiverThread.Start();
            //isRunningudplisten = true;
            //udpReceiverThread = new Thread(ReceiveDataOnce);
            //udpReceiverThread.Start();

            // Bắt đầu chụp ảnh màn hình mỗi 5 giây
            isRunningscreenshot5s = true;
            screenshotThread5s = new Thread(CaptureAndSendScreenshots5s);
            screenshotThread5s.Start();

            // Bắt đầu lắng nghe kết nối TCP
            isRunningtcplisten = true;
            listenThread = new Thread(ListenForClients);
            listenThread.Start();


        }

        private void GetMSSV(string mssv)
        {
            this.mssvDoTest = mssv;
            sendData($"Ready-{mssvDoTest}");
            WaitingFrom = null; 
            examFrm = new ExamForm(mssvDoTest, Test, sendData, UpdateMSSV);
            examFrm.Show();
        }
        private void UpdateMSSV(string newMSSV)
        {
            this.mssvDoTest= newMSSV;
            WaitingFrom = null;
        }
        private void ShowWaitingForm(Action<string> bindingMssv)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.Hide();
                    WaitingFrom = new Waiting(bindingMssv);
                    WaitingFrom.Show();
                }));
            }
            else
            {
                this.Hide();
                WaitingFrom = new Waiting(bindingMssv);
                WaitingFrom.Show();
            }
        }
        private void ClosingExam(string mess)
        {
            Test.Quests.Clear();
            Test.IsExamining = false;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => {
                    WaitingFrom?.Dispose();
                    WaitingFrom = null;
                    this.Show();
                    examFrm?.Focus();
                    examFrm?.QuestDone(mess);
                    examFrm = null;
                }));
            }
            else
            {
                WaitingFrom?.Dispose();
                WaitingFrom = null;
                this.Show();
                examFrm?.Focus();
                examFrm?.QuestDone(mess);
                examFrm = null;
            }
        }

        private async Task TestConnection()
        {
            int maxRetries = 10;
            int retryDelay = 1000;
            bool isConnected = true;

            try {
                Console.WriteLine("Start connection checking");

                while (Test.IsExamining)
                {
                    for (int i = 0; i < maxRetries; i++)
                    {
                        if (await IsServerReachable())
                        {
                            isConnected = true;
                            Console.WriteLine("Connection  still exists");
                            break; // Thoát nếu kết nối thành công
                        }

                        // Kết nối không thành công, chờ trước khi thử lại
                        isConnected = false;
                        Console.WriteLine("The connection to the Server has been lost, attempting to reconnect");
                        await Task.Delay(retryDelay);
                    }

                    // Nếu sau maxRetries vẫn không kết nối được
                    if (!isConnected)
                    {
                        ClosingExam("Mất kết nối đến máy chủ. Ứng dụng sẽ đóng.");
                        return;
                    }

                    await Task.Delay(10000); //kiểm tra sau mỗi 10s
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the connection test: {ex.Message}");
            }
            Console.WriteLine("Connection has been stopped");

        }
        private async Task<bool> IsServerReachable()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(IpServer, 1000); // 1 giây timeout
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        private void ListenForClients()
        {
            listener = new TcpListener(IPAddress.Parse(myIp), 8888);
            listener.Start();
            Console.WriteLine("TCP listener đang lắng nghe...");

            while (isRunningtcplisten)
            {
                try
                {
                    if (!listener.Pending())
                    {
                        Thread.Sleep(100); // Giảm tải CPU khi không có kết nối
                        continue;
                    }

                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client kết nối!");

                    // Xử lý kết nối client
                    HandleClient(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi lắng nghe TCP: {ex.Message}");
                    break;
                }
            }

            listener.Stop();
            Console.WriteLine("Đã dừng TCP listener.");
        }

        private string ConvertWildcardToRegexPattern(string wildcardPattern)
        {
            // Thay thế '*' thành '.*' trong mẫu regex
            string regexPattern = wildcardPattern.Replace("*", ".*");

            // Nếu mẫu không chứa ký tự '*', thêm '^' ở đầu và '$' ở cuối để đảm bảo so khớp chính xác
            if (!regexPattern.Contains(".*"))
            {
                regexPattern = "^" + regexPattern + "$";
            }

            return regexPattern;
        }
        
        private void HandleClient(TcpClient tcpClient)
        {
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] messageBuffer = new byte[1024];
            int bytesRead;
            string receivedMessage = "";

            // Đọc dữ liệu từ client
            while ((bytesRead = clientStream.Read(messageBuffer, 0, messageBuffer.Length)) > 0)
            {
                receivedMessage += Encoding.UTF8.GetString(messageBuffer, 0, bytesRead);
                Console.WriteLine(receivedMessage);

                // Nếu nhận đủ thông điệp
                if (receivedMessage.Contains("-") && !receivedMessage.StartsWith("Key-Examt-"))
                {
                    break;
                }
            }
            // Parse the signal
            Console.WriteLine("Full message");
            Console.WriteLine(receivedMessage);
            if (receivedMessage.StartsWith("SendFile"))
            {
                // Parse the signal
                string[] parts = receivedMessage.Split(new string[] { "FileName-","ToPath-" }, StringSplitOptions.None);
                if (parts.Length >= 3)
                {
                    string fileName = parts[1];
                    string destinationPath = parts[2];

                    // Tạo thư mục đích nếu chưa tồn tại
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    // Gửi xác nhận
                    clientStream.Write(BitConverter.GetBytes(1), 0, 4);
                    clientStream.Flush();

                    // Nhận nội dung tệp
                    byte[] fileLengthBytes = new byte[4];
                    clientStream.Read(fileLengthBytes, 0, 4);
                    int fileLength = BitConverter.ToInt32(fileLengthBytes, 0);

                    byte[] fileBytes = new byte[fileLength];
                    int totalBytesRead = 0;
                    while (totalBytesRead < fileLength)
                    {
                        bytesRead = clientStream.Read(fileBytes, totalBytesRead, fileLength - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        totalBytesRead += bytesRead;
                    }

                    // Lưu tệp vào đường dẫn đích
                    string fullFilePath = Path.Combine(destinationPath, fileName);
                    File.WriteAllBytes(fullFilePath, fileBytes);

                    Console.WriteLine("Tệp đã được nhận và lưu thành công: " + fullFilePath);
                }
                tcpClient.Close();

            }
            else if (receivedMessage.StartsWith("QuickLaunch"))
            {
                // Parse the signal
                string[] parts = receivedMessage.Split(new char[] { '-' }, 2);
                if (parts.Length >= 2)
                {
                    string fileName = parts[1];
                    Process.Start(fileName);
                }
                tcpClient.Close();

            }
            else if (receivedMessage.StartsWith("MouseLeft"))
            {
                SimulateLeftClick();
            }
            else if (receivedMessage.StartsWith("MouseRight"))
            {
                SimulateRightClick();
            }
            else if (receivedMessage.StartsWith("MouseWheel"))
            {
                string[] tmp = receivedMessage.Split('_');
                Console.WriteLine(tmp[1]);
                SimulateScroll(int.Parse(tmp[1]));
            }
            else if (receivedMessage.StartsWith("Mouse"))
            {
                string[] parts = receivedMessage.Split(new char[] { '-' }, 2);
                string[] coords = parts[1].Split(',');
                int mouseX = int.Parse(coords[0]);
                int mouseY = int.Parse(coords[1]);
                // Handle the mouse coordinates (e.g., update cursor position)
                UpdateMousePosition(mouseX, mouseY);
            }
            else if (receivedMessage.StartsWith("Keyboard"))
            {
                string[] parts = receivedMessage.Split(new[] { "Keyboard-" }, StringSplitOptions.None);
                Console.WriteLine(parts[1]);
                UpdateKeyboard(parts[1]);
            }
            else if (receivedMessage.StartsWith("CollectFile"))
            {
                string[] parts = receivedMessage.Split(new string[] { "FileName-","FolderPath-" ,"Check-" }, StringSplitOptions.None);
                if (parts.Length >= 4)
                {
                    string fileNamePattern = ConvertWildcardToRegexPattern(parts[1]);
                    string destinationPath = parts[2];
                    string check = parts[3];

                    try
                    {
                        var matchingFiles = Directory.GetFiles(destinationPath)
                            .Where(path => Regex.Match(Path.GetFileName(path), fileNamePattern).Success)
                            .ToList();

                        if (matchingFiles.Any())
                        {
                            string tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

                            using (ZipArchive zip = ZipFile.Open(tempZipPath, ZipArchiveMode.Create))
                            {
                                foreach (string filePath in matchingFiles)
                                {
                                    zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                                }
                            }

                            byte[] zipBytes = File.ReadAllBytes(tempZipPath);
                            byte[] zipNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(tempZipPath));

                            // Gửi độ dài của tên file zip
                            clientStream.Write(BitConverter.GetBytes(zipNameBytes.Length), 0, 4);
                            clientStream.Write(zipNameBytes, 0, zipNameBytes.Length);

                            // Gửi độ dài nội dung file zip
                            clientStream.Write(BitConverter.GetBytes(zipBytes.Length), 0, 4);
                            clientStream.Write(zipBytes, 0, zipBytes.Length);

                            clientStream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức 

                            File.Delete(tempZipPath); // Xóa file zip tạm thời
                            Console.WriteLine(check);
                            if (check == "True")
                            {
                                foreach (string filePath in matchingFiles)
                                {
                                    File.Delete(filePath);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi khi tìm kiếm tệp: " + ex.Message);
                    }
                }
                tcpClient.Close();
            }
            else if (receivedMessage.StartsWith("Key-Exam"))
            {
             
                string[] parts = receivedMessage.Split(new[] { "Key-Exam" }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    Test = new Test(parts[0]);
                    Console.WriteLine(Test);
                    if (!string.IsNullOrEmpty(mssvDoTest))
                    {
                        sendData($"Ready-{mssvDoTest}");
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() => {
                                this.Hide();
                                examFrm = new ExamForm(mssvDoTest, Test, sendData, UpdateMSSV);
                                examFrm.Show();
                                examFrm.Focus();
                            }));
                        }
                        else
                        {
                            this.Hide();
                            examFrm = new ExamForm(mssvDoTest, Test, sendData, UpdateMSSV);
                            examFrm.Show();
                            examFrm.Focus();
                        }
                        return;
                    }
                    if (WaitingFrom != null)
                    {
                        return;
                    }
                    ShowWaitingForm(GetMSSV);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi nhận đề: {ex.Message}");
                }finally { tcpClient.Close(); }

            } 
            else if (receivedMessage.StartsWith("TopStudent"))
            {
                try
                {
                    if (string.IsNullOrEmpty(mssvDoTest))
                    {
                        if (WaitingFrom != null)
                            return;
                        
                        ShowWaitingForm(UpdateMSSV);
                        return;
                    }
                    string[] parts = receivedMessage.Split(new[] { "TopStudent" }, StringSplitOptions.RemoveEmptyEntries);

                    List<string> topString = parts[0].Split(new[] { "sts@" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {

                            examFrm?.ShowTop(topString);
                        }));
                    }
                    else
                    {
                        examFrm?.ShowTop(topString);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi nhận score top: " + ex.Message);
                }
                finally { tcpClient.Close(); }

            }
            else if (receivedMessage.StartsWith("QuestCome")) {
                try
                {
                    string[] mess = receivedMessage.Split(new[] { "QuestCome" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] pts = mess[0].Split(new[] { "test@@" }, StringSplitOptions.RemoveEmptyEntries);
                    if (Test == null)
                    {
                        Test = new Test(pts[0]);
                    }

                    if (string.IsNullOrEmpty(mssvDoTest))
                    {
                        if (WaitingFrom != null)
                        {
                            return;
                        };
                        ShowWaitingForm(UpdateMSSV);
                        return;
                    }

                    Quest quest = new Quest(pts[1]);
                    Test.Quests.Add(quest);
                    if (examFrm == null)
                    {
                        examFrm = new ExamForm(mssvDoTest, Test, sendData, UpdateMSSV, true);
                        Test.IsExamining = true;
                        _ = TestConnection();
                    }

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            examFrm?.Show();
                            examFrm?.Focus();
                            examFrm?.StartDoExam(true);
                            examFrm?.NotiQuestCome(quest.Index);
                        }));
                    }
                    else
                    {
                        examFrm?.Show();
                        examFrm?.Focus();
                        examFrm?.StartDoExam(true);
                        examFrm?.NotiQuestCome(quest.Index);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi nhận câu hỏi: " + ex.Message);
                }
                finally { tcpClient.Close(); }
            }
            else
            {
                switch (receivedMessage)
                {
                    case "LOCK_ACCESS": LockWeb(); Console.WriteLine("nhan dc tin hieu"); tcpClient.Close(); break;
                    case "LockScreen": OpenNewFormLockScreen(); break;
                    case "SlideShow":
                        //SlideShowForm slideShowForm = new SlideShowForm();

                        // Gán địa chỉ IP của máy chủ từ ClientForm sang SlideShowForm
                        //slideShowForm.ServerIP = IpServer;
                        //slideShowForm.Listener = listener;
                        //listener.Stop();
                        OpenNewForm();
                        // Hiển thị SlideShowForm
                        //slideShowForm.Show();
                        break;
                    case "SlideShowToClient":
                        sendData("ReadyToCapture");
                        isRunningscreenshot = true;

                        screenshotThread = new Thread(new ThreadStart(CaptureAndSendScreenshotsContinuously));
                        screenshotThread.Start();

                        break;
                    case "CloseSlideShow":

                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                if (form1 != null)
                                {
                                    form1.StopSlideshow();
                                    form1=null;
                                };
                            });
                        }
                        else
                        {

                            if (form1 != null)
                            {
                                form1.StopSlideshow();
                                form1=null;

                            }
                        }
                        break;
                    case "CloseLockScreen":

                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                if (LockScreen != null)
                                {
                                    LockScreen.UnLockScreen();
                                    LockScreen=null;
                                };
                            });
                        }
                        else
                        {

                            if (LockScreen != null)
                            {
                                LockScreen.UnLockScreen();
                                LockScreen=null;
                            };
                        }
                        break;
                    case "DoExam":
                        Console.WriteLine("Làm bài");
                        if (string.IsNullOrEmpty(mssvDoTest))
                        {
                            if (WaitingFrom != null) break;

                            ShowWaitingForm(UpdateMSSV);
                            break;
                        }
                        if (examFrm == null)
                        {
                            examFrm = new ExamForm(mssvDoTest,Test, sendData,UpdateMSSV);
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() => {
                                    examFrm.Show();
                                    examFrm.Focus();
                                }));
                            }
                            else
                            {
                                examFrm.Show();
                                examFrm.Focus();
                            }
                        }

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(()=>{
                                this.Hide();
                                examFrm?.StartDoExam();
                                Test.IsExamining = true;
                                _=TestConnection();

                            }));
                        }
                        else
                        {
                            this.Hide();
                            examFrm?.StartDoExam();
                            Test.IsExamining = true;
                            _ = TestConnection();
                        }

                        break;
                    case "CancelTest":
                        string messCancelTest = "Bài kiểm tra đã bị giáo viên hủy";
                        ClosingExam(messCancelTest);
                        break;
                    case "TestDone": 
                        string messTestDone = "Bạn đã thi xong";
                        ClosingExam(messTestDone);
                        break;
                }
                tcpClient.Close();
            }
        }

        
        private void UpdateKeyboard(string key)
        {
            // Ensure that updating the mouse position is done on the UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateKeyboard), new object[] { key });
                return;
            }
            if (string.IsNullOrEmpty(key)) ;
            //VirtualKeyCode vKey  = KeyCodeMapper.MapStringToVirtualKeyCode(key);
            InputSimulator sim = new InputSimulator();
            //sim.Keyboard.KeyPress(vKey);
        }



        private void UpdateMousePosition(int x, int y)
        {
            // Ensure that updating the mouse position is done on the UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, int>(UpdateMousePosition), new object[] { x, y });
                return;
            }
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;
            // Get the size of the form (which is fullscreen) and the screen

            // Map the received coordinates to the form's size (this assumes the coordinates are proportional)
            // You might need to scale the coordinates if they're in a different resolution or range
            int scaledX = (int)(x * screenWidth / 1600.0); // Assume 1920x1080 as the reference resolution
            int scaledY = (int)(y * screenHeight / 900.0);

            // Set the cursor position to the mapped coordinates
            Cursor.Position = new Point(scaledX, scaledY);
        }

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
        private void CaptureAndSendScreenshotsContinuously()
        {
            UdpClient udpClient = new UdpClient(); // Tạo UDP client
            //udpClient.Client.SendBufferSize = 70000;
            int udpBufferSize = udpClient.Client.SendBufferSize;

            // Lấy địa chỉ broadcast của mạng local
            IPAddress broadcastAddress = GetBroadcastAddress();
            int i = 1;
            int fps = 120;
            while (isRunningscreenshot)
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
                    Thread.Sleep(1000 / fps); // Chờ 1/60 giây (mili giây) trước khi gửi hình tiếp theo
                }
                catch (ThreadInterruptedException ex)
                {
                    // Xử lý ngoại lệ ở đây
                    Console.WriteLine("Thread interrupted: " + ex.Message);
                }
            }
        }

        public void SendImage(string ipAddress, int port, string machineName, Bitmap image)
        {
            try
            {
                // Tạo kết nối tới server
                TcpClient client = new TcpClient(ipAddress, port);
                NetworkStream stream = client.GetStream();

                // Chuyển ảnh thành mảng byte
                using (MemoryStream ms = new MemoryStream())
                {
                    // Giảm chất lượng JPEG để nén ảnh
                    var jpegEncoder = ImageCodecInfo.GetImageDecoders().First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                    var encoderParams = new EncoderParameters(1) { Param = { [0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L) } };
                    image.Save(ms, jpegEncoder, encoderParams);
                    byte[] imageBytes = ms.ToArray();

                    // Tạo key với format Picture5s-Tên máy
                    string key = $"Picture5s-{machineName}Anh-";
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                    // Gửi key trước
                    stream.Write(keyBytes, 0, keyBytes.Length);

                    // Sau đó gửi ảnh
                    stream.Write(imageBytes, 0, imageBytes.Length);
                }

                // Đóng kết nối
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gửi ảnh: " + ex.Message);
            }
        }

        private void CaptureAndSendScreenshots5s()
        {
            while (isRunningscreenshot5s)
            {
                try
                {
                    Console.WriteLine("Chụp ảnh màn hình...");

                    int screenLeft = SystemInformation.VirtualScreen.Left;
                    int screenTop = SystemInformation.VirtualScreen.Top;
                    int screenWidth = SystemInformation.VirtualScreen.Width;
                    int screenHeight = SystemInformation.VirtualScreen.Height;

                    using (Bitmap screenshot = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(screenshot))
                        {
                            graphics.CopyFromScreen(screenLeft, screenTop, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy);
                            DrawCursorOnScreenshot(graphics);
                            SendImage(IpServer, 8765, Environment.MachineName, screenshot);
                        }
                    }

                    Thread.Sleep(5000); // Đợi 5 giây
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi chụp hoặc gửi ảnh: {ex.Message}");
                }
            }

            Console.WriteLine("Đã dừng chụp ảnh 5 giây/lần.");
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
            } while (imageData.Length > bufferSize && quality > 100);

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
                //Console.WriteLine(imageData.Length + " " + cutCount + " " + i + " " + quality);
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
            cursor.Draw(graphics, new Rectangle(cursorPosition, cursor.Size));
        }
        private void OpenNewFormLockScreen()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { OpenNewFormLockScreen(); });
            }
            else
            {
                if (LockScreen == null)
                {
                    LockScreen = new LockScreenForm();
                    LockScreen.Show();
                }
            }
        }
        private void OpenNewForm()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { OpenNewForm(); });
            }
            else
            {
                if (form1 == null)
                {
                    form1 = new SlideShowForm();
                    form1.ServerIP = IpServer;
                    form1.Show();
                }
            }
        }

        

        private void LockWeb()
        {
            try
            {
                // Đường dẫn tới tập tin hosts
                string hostsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\drivers\etc\hosts";

                // Kiểm tra xem tập tin hosts có tồn tại không
                if (!File.Exists(hostsFilePath))
                {
                    Console.WriteLine("Tập tin hosts không tồn tại.");
                    return;
                }

                // Địa chỉ IP loopback
                string loopbackIP = "127.0.0.1";

                // Đọc nội dung tập tin hosts hiện tại
                string[] hostsContent = File.ReadAllLines(hostsFilePath);

                // Kiểm tra xem các dòng cần thiết đã có chưa, tránh ghi đè
                bool isLineExist = hostsContent.Any(line => line.Contains(loopbackIP));

                if (!isLineExist)
                {
                    // Thêm các dòng vào cuối tập tin hosts
                    using (StreamWriter writer = new StreamWriter(hostsFilePath, true))
                    {
                        writer.WriteLine($"{loopbackIP} localhost");
                        writer.WriteLine($"{loopbackIP} localhost.localdomain");
                        writer.WriteLine($"{loopbackIP} 0.0.0.0");
                        writer.WriteLine($"{loopbackIP} 255.255.255.255");
                    }

                    Console.WriteLine("Tập tin hosts đã được cập nhật thành công!");
                }
                else
                {
                    Console.WriteLine("Các dòng đã tồn tại trong tập tin hosts.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Đã xảy ra lỗi khi cập nhật tập tin hosts: " + ex.Message);
            }
        }

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




        private void ReceiveDataOnce()
        {
            while (isRunningudplisten)
            {
                try
                {
                    Console.WriteLine("Đang nghe UDP...");
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    inf = GetDeviceInfo();
                    inf.Add($" {string.Join("-", mssvLst)}");
                      
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                    string receivedMessage = Encoding.UTF8.GetString(receivedBytes);

                    // Tách chuỗi để lấy IP và xử lý trạng thái
                    string ip;

                    if (receivedMessage.Contains("-End"))
                    {
                        ip = receivedMessage.Replace("-End", "").Trim();
                        isRunningudplisten = false; // Dừng lắng nghe
                    }
                    else
                    {
                        ip = receivedMessage.Trim();
                    }

                    IpServer = ip;
                    Console.WriteLine($"Dữ liệu nhận được: {IpServer}");

                    // Gửi thông tin tới server
                    sendInfToServer();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Lỗi khi nhận dữ liệu UDP: {ex.Message}");
                    Thread.Sleep(1000); // Tạm dừng 1 giây trước khi thử lại
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                }
            }

            Console.WriteLine("Đã dừng UDP listener.");
        }



        List<string> stringList = new List<string>();
        public static bool HasUsbDevice(string deviceType)
        {
            // Xác định PNPClass tương ứng với loại thiết bị
            string pnpClass = deviceType.Equals("Mouse", StringComparison.OrdinalIgnoreCase) ? "Mouse" :
                              deviceType.Equals("Keyboard", StringComparison.OrdinalIgnoreCase) ? "Keyboard" : null;

            if (pnpClass == null)
            {
                throw new ArgumentException("Invalid device type. Only 'Mouse' and 'Keyboard' are supported.");
            }

            // Truy vấn tất cả các thiết bị USB
            ManagementObjectSearcher usbSearcher = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT * FROM Win32_USBControllerDevice"
            );

            foreach (ManagementObject usbDevice in usbSearcher.Get())
            {
                // Lấy DeviceID của thiết bị USB
                string dependent = usbDevice["Dependent"].ToString();
                string deviceId = dependent.Split('=')[1].Trim('\"');

                // Truy vấn thông tin chi tiết của thiết bị dựa trên DeviceID và PNPClass tương ứng
                ManagementObjectSearcher deviceSearcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    $"SELECT * FROM Win32_PnPEntity WHERE DeviceID = '{deviceId}' AND PNPClass = '{pnpClass}'"
                );

                foreach (ManagementObject device in deviceSearcher.Get())
                {
                    if (device["Status"] != null && device["Status"].ToString().Contains("OK"))
                    {
                        return true; // Thiết bị USB có sẵn và hoạt động
                    }
                }
            }

            return false; // Không tìm thấy thiết bị USB hoặc không hoạt động
        }

        private List<string> GetDeviceInfo()
        {
            stringList.Add($"InfoClient-");

            // Lấy thông tin về tên máy
            string machineName = Environment.MachineName;
            // string machineName = "F71-01";
            stringList.Add($"IPC: {myIp}");
            stringList.Add($"Tenmay: {machineName}");
            lblNameComputer.Text = machineName;

            // Kiểm tra kết nối chuột
            if (HasUsbDevice("Mouse"))
            {
                stringList.Add("Chuot: Đã kết nối");
            }
            else
            {
                stringList.Add("Chuot: Không kết nối");
            }


            if (HasUsbDevice("Keyboard"))
            {
                stringList.Add("Banphim: Đã kết nối");
            }
            else
            {
                stringList.Add("Banphim: Không kết nối");
            }

            Screen[] screens = Screen.AllScreens;
            if (screens.Length == 1 && screens[0].Primary)
            {
                stringList.Add("Manhinh: Đã kết nối");
            }
            else
            {
                stringList.Add("Manhinh: Không kết nối");
            }
            stringList.Add($"Ocung: ");

            // Lấy thông tin về ổ cứng
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                string HDInfo = "";
                // Lấy model của ổ cứng
                stringList.Add("Model: " + wmi_HD["Model"]);
                HDInfo += "Model: " + wmi_HD["Model"];
                // Lấy giao diện (interface) của ổ cứng
                stringList.Add("Interface: " + wmi_HD["InterfaceType"]);
                HDInfo += " - Interface: " + wmi_HD["InterfaceType"];

                // Lấy kích thước của ổ cứng (size)
                ulong size = (ulong)wmi_HD["Size"];
                stringList.Add("Size: " + (size / (1024 * 1024 * 1024)) + " GB");
                HDInfo += " - Size: " + (size / (1024 * 1024 * 1024)) + " GB";
                InfoUC infoUC = new InfoUC();
                infoUC.Image = Properties.Resources.harddisk;
                infoUC.TextLabel = HDInfo;
                InForGroup.Controls.Add(infoUC);

            }

            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

            stringList.Add($"CPU: ");
            // Thực hiện truy vấn và lấy kết quả
            foreach (ManagementObject obj in searcher.Get())
            {
                // In ra một số thông tin CPU
                stringList.Add($"{obj["Name"]}");
                InfoUC infoUC = new InfoUC();
                infoUC.Image = Properties.Resources.cpu;
                infoUC.TextLabel = $"{obj["Name"]}";
                InForGroup.Controls.Add(infoUC);
            }

            stringList.Add($"RAM: ");
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher.Get())
            {
                ulong capacityBytes = (ulong)obj["Capacity"];
                double capacityGB = Math.Round(capacityBytes / (1024.0 * 1024 * 1024), 2); // Chuyển sang GB và làm tròn đến 2 chữ số thập phân

                // Lấy thông tin nhà sản xuất
                string manufacturer = obj["Manufacturer"] != null ? obj["Manufacturer"].ToString() : "Unknown";

                // Thêm thông tin vào danh sách
                stringList.Add($"Capacity: {capacityGB} GB, Manufacturer: {manufacturer}|");
                InfoUC infoUC = new InfoUC();
                infoUC.Image = Properties.Resources.memory;
                infoUC.TextLabel = $"Capacity: {capacityGB} GB - Manufacturer: {manufacturer}";
                InForGroup.Controls.Add(infoUC);
            }
            stringList.Add("MSSV: ");
            return stringList;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            // Kiểm tra MSSV có phải là 10 ký tự số
            string mssvPattern = @"^\d{10}$"; // MSSV phải có đúng 10 ký tự số
            Regex mssvRegex = new Regex(mssvPattern);

            // Kiểm tra Họ và Tên theo Regex tiếng Việt
            string namePattern = @"^[\p{L}\s]+$"; // Chỉ cho phép chữ cái và khoảng trắng
            Regex nameRegex = new Regex(namePattern);

            // MSSV đã tồn tại trong danh sách không cho phép nhập lại
            if (mssvList.Contains(txtMSSV.Text))
            {
                MessageBox.Show("MSSV đã tồn tại, vui lòng nhập MSSV khác", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra điều kiện nhập MSSV và họ tên
            if (!mssvRegex.IsMatch(txtMSSV.Text) ||
                !nameRegex.IsMatch(txtFullName.Text.Trim()))
            {
                MessageBox.Show("Nhập thiếu thông tin hoặc sai MSSV, họ và tên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Thêm MSSV mới vào danh sách
            mssvList.Add(txtMSSV.Text);

            string mssv = txtMSSV.Text;
            string fullName = txtFullName.Text.Trim();  // Lấy và cắt khoảng trắng thừa

            if (IpServer == String.Empty) return;

            // Kết hợp MSSV và Họ tên thành một chuỗi
            string tmp = $"{fullName} - {mssv} +";

            // Thêm vào danh sách nếu không rỗng
            if (!string.IsNullOrEmpty(tmp))
            {
                mssvLst.Add(tmp);
            }

            // Kiểm tra và thêm vào danh sách 'inf'
            if (inf[inf.Count - 1] != tmp && !string.IsNullOrEmpty(tmp))
            {
                inf.Add($" {string.Join("-", mssvLst)}");
            }

            // Gửi thông tin lên server
            sendInfToServer();

            foreach (var i in inf)
            {
                Console.Write(i);
            }


            if (sended)
            {
                InfoUC infoUC = new InfoUC();
                infoUC.Image = Properties.Resources.male_student;
                infoUC.TextLabel = $"{fullName} - {mssv}";  // Hiển thị họ tên và MSSV
                InForGroup.Controls.Add(infoUC);

                // Xóa nội dung các TextBox sau khi xử lý
                txtMSSV.Text = String.Empty;
                txtFullName.Text = String.Empty;

                MessageBox.Show("Gửi thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Gửi thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }



        private void sendInfToServer()
        {

            sended = false;
            try
            {
                if (IpServer != String.Empty)
                {
                    Console.WriteLine(IpServer);
                    // Tạo đối tượng TcpClient để kết nối đến server
                    TcpClient client = new TcpClient(IpServer, 8765);
                    //Console.WriteLine("Send: "+string.Join("", inf.ToArray()));
                    //// Lấy luồng mạng từ TcpClient
                    NetworkStream stream = client.GetStream();
                    SendData(stream, string.Join("", inf.ToArray()));
                    byte[] buffer = new byte[1024];
                    sended = true;// Định kích thước buffer tùy ý
                    client.Close();

                }

            }
            catch (Exception q)
            {
                Console.WriteLine(q);
            }
        }

        private void sendData(string data)
        {

            sended = false;
            try
            {
                if (IpServer != String.Empty)
                {
                    Console.WriteLine(IpServer);
                    // Tạo đối tượng TcpClient để kết nối đến server
                    TcpClient client = new TcpClient(IpServer, 8765);
                    //Console.WriteLine("Send: "+string.Join("", inf.ToArray()));
                    //// Lấy luồng mạng từ TcpClient
                    NetworkStream stream = client.GetStream();
                    SendData(stream, data);
                    byte[] buffer = new byte[1024];
                    sended = true;// Định kích thước buffer tùy ý
                    client.Close();

                }

            }
            catch (Exception q)
            {
                Console.WriteLine(q);
            }
        }

        static void SendData(NetworkStream stream, string message)
        {

            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);

        }




        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dừng UDP listener
            isRunningudplisten = false;
            udpReceiverThread?.Join(); // Chờ thread kết thúc

            // Dừng chụp ảnh màn hình mỗi 5 giây
            isRunningscreenshot5s = false;
            screenshotThread5s?.Join();

            // Dừng lắng nghe TCP client
            isRunningtcplisten = false;
            listenThread?.Join();

            // Dừng chụp ảnh màn hình liên tục
            isRunningscreenshot = false;
            screenshotThread?.Join();

            // Cleanup nếu cần thiết
            listener?.Stop();
            udpClient?.Close();
        }

        private void SimulateLeftClick()
        {
            var cursorPosition = Cursor.Position;
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)cursorPosition.X, (uint)cursorPosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)cursorPosition.X, (uint)cursorPosition.Y, 0, 0);
        }

        private void SimulateRightClick()
        {
            var cursorPosition = Cursor.Position;
            mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)cursorPosition.X, (uint)cursorPosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, (uint)cursorPosition.X, (uint)cursorPosition.Y, 0, 0);
        }

        private void SimulateScroll(int delta)
        {
            // Simulate scrolling by the specified delta amount
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)delta, 0);
        }
    }
}
