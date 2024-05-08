using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class SlideShowForm : Form
    {
        private string serverIP;

        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }
        private string myIp = "";
        private TcpClient client;
        private NetworkStream stream;
        private bool isLoaded = false;
        TcpListener listener;
        private Thread listenThread;
        public SlideShowForm()
        {
            InitializeComponent();
            myIp = getIPServer();
        }

        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            // Kết nối đến máy chủ và gửi tín hiệu load xong
            ConnectToServer();
            SendLoadDoneSignal();
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();

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

                // Kiểm tra xem liệu có phải là hiệu lệnh "PicSS-" không
                if (receivedMessage.StartsWith("PicSS-"))
                {
                    try
                    {
                        // Đọc dữ liệu hình ảnh từ máy chủ
                        byte[] imageData = new byte[1024];
                        int bytesReadImage = clientStream.Read(imageData, 0, imageData.Length);

                        // Xử lý hình ảnh nhận được và cập nhật giao diện của form
                        if (bytesReadImage > 0)
                        {
                            // Decode imageData và hiển thị hình ảnh lên form
                            pictureBox1.Image = DecodeImage(imageData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error receiving image: " + ex.Message);
                    }
                }
            }
        }
        private Image DecodeImage(byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                return Image.FromStream(ms);
            }
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient(serverIP, 8765);
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to server: " + ex.Message);
            }
        }

        private void SendLoadDoneSignal()
        {
            if (stream != null)
            {
                string message = "LOAD_DONE";
                byte[] messageData = Encoding.UTF8.GetBytes(message);
                stream.Write(messageData, 0, messageData.Length);
                isLoaded = true;
            }
        }



        // Hàm DecodeImage để giải mã dữ liệu hình ảnh nhận được từ máy chủ
        // Bạn cần tự triển khai hàm này hoặc sử dụng thư viện phù hợp để giải mã hình ảnh từ dữ liệu byte

        private void SlideShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Đóng kết nối khi đóng form
            if (client != null)
                client.Close();
        }
    }
}
