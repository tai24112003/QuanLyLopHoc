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

namespace testUdpTcp
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
            myIp = getIPServer();
            inf = GetDeviceInfo();
        }
        private UdpClient udpClient;
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
                

            }

            switch (receivedMessage)
            {
                case "LOCK_ACCESS": LockWeb(); Console.WriteLine("nhan dc tin hieu"); break;
                case "SlideShow":
                    Console.WriteLine("Nhan SlideShow");
                    Thread t = new Thread(() => Application.Run(new SlideShowForm()));
                    t.Start();
                    break;
            }
            tcpClient.Close();

            
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
                // Lọc ra các card mạng LAN
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    // Lấy danh sách địa chỉ IP của card mạng này
                    UnicastIPAddressInformationCollection ipInfo = networkInterface.GetIPProperties().UnicastAddresses;

                    foreach (UnicastIPAddressInformation info in ipInfo)
                    {
                        // Chỉ lấy địa chỉ IPv4
                        if (info.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ip = info.Address.ToString();
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
        if (!string.IsNullOrEmpty(mobj["Name"] as string))
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
            if(tmp != String.Empty)
            {
                mssvLst.Add(tmp);
            }
            if (inf[inf.Count -1] != tmp && tmp != String.Empty)
            {
                inf.Add($" {string.Join("-", mssvLst)}");
                mssvLst.Clear();
            }
                
            
            
            updateBox();
            textBox1.Text = String.Empty;
            
            sendInfToServer();
            if (sended) MessageBox.Show("Gửi thành công","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);
            else MessageBox.Show("Gửi thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void sendInfToServer()
        {
            
            sended = false;
            try
            {
                if (IpServer != String.Empty)
                {
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

        static void SendData(NetworkStream stream,string message)
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
            if (listener != null)
            listener.Stop();
        }
    }
}
