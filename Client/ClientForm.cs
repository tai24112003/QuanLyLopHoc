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

namespace testUdpTcp
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
            //myIp = getIPServer();
            myIp = getIPServer();
            inf = GetDeviceInfo();

        }
        private UdpClient udpClient;
        SlideShowForm form1;
        private Thread udpReceiverThread;
        private string IpServer = "";
        private string myIp = "";
        private List<string> inf;
        private List<string> mssvLst = new List<string>();
        private bool sended = false;
        private string hostName;
        TcpListener listener;
        private Thread listenThread;


        private void Form1_Load(object sender, EventArgs e)
        {

            udpClient = new UdpClient(11312);
            udpReceiverThread = new Thread(new ThreadStart(ReceiveDataOnce));
            udpReceiverThread.Start();
            udpReceiverThread.Join();
            updateBox();

            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();

        }
        private void ListenForClients()
        {

            listener = new TcpListener(IPAddress.Parse(myIp), 8888);
            listener.Start();

            Console.WriteLine("Client is listening...");
            while (true)
            {
                try
                {

                    TcpClient client = listener.AcceptTcpClient();
                    // Bạn có thể xử lý kết nối client ở đây

                    HandleClient(client);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
            }
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

                // Nếu nhận đủ thông điệp
                if (receivedMessage.Contains("-"))
                {
                    break;
                }
            }

            if (receivedMessage.StartsWith("SendFile"))
            {
                // Parse the signal
                string[] parts = receivedMessage.Split('-');
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
            }
            else if (receivedMessage.StartsWith("CollectFile"))
            {
                // Parse the signal
                string[] parts = receivedMessage.Split('-');
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
            }
            else
            {
                switch (receivedMessage)
                {
                    case "LOCK_ACCESS": LockWeb(); Console.WriteLine("nhan dc tin hieu"); tcpClient.Close(); break;
                    case "SlideShow":
                        //SlideShowForm slideShowForm = new SlideShowForm();

                        // Gán địa chỉ IP của máy chủ từ ClientForm sang SlideShowForm
                        //slideShowForm.ServerIP = IpServer;
                        //slideShowForm.Listener = listener;
                        listener.Stop();
                        OpenNewForm(tcpClient);
                        // Hiển thị SlideShowForm
                        //slideShowForm.Show();
                        break;
                }

            }

            Console.WriteLine("Đóng kết nối");
            tcpClient.Close();
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

                // Mở tập tin hosts để ghi đè
                using (StreamWriter writer = new StreamWriter(hostsFilePath, false))
                {
                    // Ghi đè nội dung tập tin hosts với mỗi yêu cầu trang web được điều hướng đến địa chỉ loopback
                    writer.WriteLine($"{loopbackIP} localhost");
                    writer.WriteLine($"{loopbackIP} localhost.localdomain");
                    writer.WriteLine($"{loopbackIP} 0.0.0.0");
                    writer.WriteLine($"{loopbackIP} 255.255.255.255");
                }

                Console.WriteLine("Tập tin hosts đã được cập nhật thành công!");
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

            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Gọi Receive một lần để nhận dữ liệu
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
                IpServer = receivedMessage;
                // Xử lý dữ liệu nhận được
                //DisplayMessage(IpServer);
                sendInfToServer();
                //if (sended) MessageBox.Show("Gửi thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //else MessageBox.Show("Gửi thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi nhận dữ liệu: {ex.Message}");
            }
        }
        private void DisplayMessage(string message)
        {
            // Thực hiện hiển thị thông điệp trong TextBox hoặc nơi khác trên giao diện người dùng
            if (label1.InvokeRequired)
            {
                label1.Invoke(new Action(() => { label1.Text = message; }));
            }
            else
            {
                label1.Text = message;
            }
        }
        List<string> stringList = new List<string>();
        public static bool HasDevice(string strtype)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_" + strtype);

            foreach (ManagementObject mobj in searcher.Get())
            {
                // Kiểm tra xem trường "Name" có giá trị hay không để xác định thiết bị có sẵn và hoạt động
                if (mobj["Status"].ToString().Contains("OK"))
                {
                    return true; // Thiết bị có sẵn và hoạt động
                }
            }

            return false; // Không tìm thấy thiết bị hoặc không hoạt động
        }

        private List<string> GetDeviceInfo()
        {
            stringList.Add($"InfoClient-");

            // Lấy thông tin về tên máy
            string machineName = Environment.MachineName;
            stringList.Add($"IPC: {myIp}");
            stringList.Add($"Tenmay: {machineName}");

            // Kiểm tra kết nối chuột
            if (HasDevice("PointingDevice"))
            {
                stringList.Add("Chuot: Đã kết nối");
            }
            else
            {
                stringList.Add("Chuot: Không kết nối");
            }


            if (HasDevice("Keyboard"))
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
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    stringList.Add($"{drive.Name}, {drive.TotalSize / (1024 * 1024 * 1024)} GB\n");
                }

            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

            stringList.Add($"CPU: ");
            // Thực hiện truy vấn và lấy kết quả
            foreach (ManagementObject obj in searcher.Get())
            {
                // In ra một số thông tin CPU
                stringList.Add($"{obj["Name"]}\n");
            }

            stringList.Add($"RAM: ");
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher.Get())
            {
                stringList.Add($"Capacity: {obj["Capacity"]} bytes Speed: {obj["Speed"]} Manufacturer: {obj["Manufacturer"]}  Part Number: {obj["PartNumber"]}\n");
            }
            stringList.Add("MSSV: ");
            return stringList;
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void updateBox()
        {
            listBox1.Items.Clear();
            foreach (string infe in inf) listBox1.Items.Add(infe);
        }

        private void textBox1_MultilineChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (IpServer == String.Empty) return;
            string[] mssvs = textBox1.Text.Split(new[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string tmp = string.Join(" ", mssvs);
            if (tmp != String.Empty)
            {
                mssvLst.Add(tmp);
            }
            if (inf[inf.Count - 1] != tmp && tmp != String.Empty)
            {
                inf.Add($" {string.Join("-", mssvLst)}");
                mssvLst.Clear();
            }



            updateBox();
            textBox1.Text = String.Empty;

            sendInfToServer();
            if (sended) MessageBox.Show("Gửi thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show("Gửi thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        static void SendData(NetworkStream stream, string message)
        {

            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);

        }
        static void ReceiveData(NetworkStream stream)
        {


            //// Chuyển dữ liệu nhận được sang dạng chuỗi
            //string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Loi");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            udpClient.Close();
            listenThread.Abort();
            Cursor.Show();
            if (listener != null)
                listener.Stop();
        }
    }
}
