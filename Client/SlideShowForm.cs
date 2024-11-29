using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class SlideShowForm : Form
    {
        public bool AllowClose { get; set; } = false;
        private UdpClient udpClient;
        TcpListener tcpListener;
        private Thread udpListenerThread;
        private Thread tcpListenerThread;
        private string serverIP;
        private bool isRunning;

        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }
        string myIp;
        public SlideShowForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền của form
            this.WindowState = FormWindowState.Maximized; // Phóng to form ra toàn màn hình
            //this.TopMost = true; // Đặt form ở trên cùng của tất cả các cửa sổ khác
            myIp = getIPServer();

            // Ẩn con trỏ chuột
            //Cursor.Hide();
            udpClient = new UdpClient(8889); // Khởi tạo UDP client và lắng nghe trên cổng 8889
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
        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            ConnectToServer();
            isRunning = true;
            udpListenerThread = new Thread(new ThreadStart(ListenForUdpClients));
            udpListenerThread.Start();
            tcpListenerThread = new Thread(new ThreadStart(ListenForTcpClients));
            tcpListenerThread.Start();
        }

        private void ConnectToServer()
        {
            try
            {
                TcpClient client = new TcpClient(ServerIP, 8765);

                // Gửi yêu cầu khóa tới máy Client
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes("LOAD_DONE");
                stream.Write(data, 0, data.Length);
                // Đóng kết nối
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to server: " + ex.Message);
            }
        }
        private void ListenForUdpClients()
        {
            Dictionary<int, List<byte[]>> imagesData = new Dictionary<int, List<byte[]>>();


            try
            {
                while (isRunning) // Kiểm tra điều kiện dừng
                {
                    // Tạo một điểm cuối IP bất kỳ để lắng nghe dữ liệu từ UDP client
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedData = udpClient.Receive(ref remoteEP);

                    // Trích xuất chuỗi tín hiệu (13 byte đầu tiên)
                    string signalStr = Encoding.UTF8.GetString(receivedData, 0, 13);
                    if (signalStr.EndsWith("+"))
                    {
                        int imageIndex = int.Parse(signalStr.Substring(0, 4));
                        int cutCount = int.Parse(signalStr.Substring(4, 4));
                        int cutIndex = int.Parse(signalStr.Substring(8, 4));

                        // Lấy dữ liệu ảnh, bỏ qua 13 byte đầu tiên (tín hiệu)
                        byte[] imageData = new byte[receivedData.Length - 13];
                        Array.Copy(receivedData, 13, imageData, 0, imageData.Length);

                        // Khởi tạo danh sách các phần của ảnh nếu chưa có
                        if (!imagesData.ContainsKey(imageIndex))
                        {
                            imagesData[imageIndex] = new List<byte[]>(new byte[cutCount][]);
                        }

                        // Lưu phần ảnh vào vị trí tương ứng
                        imagesData[imageIndex][cutIndex - 1] = imageData;

                        // In thông tin kiểm tra
                        Console.WriteLine($"Received part {cutIndex}/{cutCount} of image {imageIndex}");

                        // Kiểm tra xem tất cả các phần của ảnh đã được nhận đủ chưa
                        if (imagesData[imageIndex].All(part => part != null))
                        {
                            // Tạo ảnh từ dữ liệu đã ghép và hiển thị ảnh
                            List<byte> completeImageData = new List<byte>();
                            foreach (var part in imagesData[imageIndex])
                            {
                                completeImageData.AddRange(part);
                            }

                            using (MemoryStream ms = new MemoryStream(completeImageData.ToArray()))
                            {
                                Image image = Image.FromStream(ms);
                                Console.WriteLine($"Displaying image {imageIndex}"); // Thông báo hiển thị ảnh
                                UpdatePictureBox(image);
                            }

                            // Xóa dữ liệu của ảnh vừa nhận xong để chuẩn bị cho lần nhận ảnh tiếp theo
                            imagesData.Remove(imageIndex);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (isRunning) // Nếu luồng vẫn chạy, in ra lỗi
                {
                    Console.WriteLine("Lỗi khi nhận dữ liệu UDP: " + ex.Message);
                }
            }
        }

        private void SlideShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (udpClient != null)
            {
                isRunning = false; // Dừng luồng UDP listener
                udpListenerThread.Abort();
                udpListenerThread?.Join(); // Đợi luồng hoàn thành
                udpClient.Close();
                udpClient.Dispose();
                tcpListenerThread.Abort();
                tcpListenerThread?.Join();
                tcpListener.Stop();
                tcpListener = null;
            }

            if (!AllowClose)
            {
                // Nếu không được phép đóng, hủy sự kiện đóng form
                e.Cancel = true;
                MessageBox.Show("Không được tự đóng form");
            }
            else
            {
                // Hiển thị lại con trỏ chuột khi form bị đóng
                Cursor.Show();
            }
        }

        public void StopSlideshow()
        {
            // Thực hiện các thao tác dừng slideshow
            AllowClose = true; // Cho phép form được đóng
            this.Close(); // Đóng form
        }


        private void UpdatePictureBox(Image image)
        {
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new Action<Image>(UpdatePictureBox), image);
                return;
            }

            pictureBox1.Image = image;
        }

        private void ListenForTcpClients()
        {
            try
            {
                // Khởi tạo TCPListener để lắng nghe kết nối từ client trên cổng 8765
                tcpListener = new TcpListener(IPAddress.Parse(myIp), 8765);
                tcpListener.Start();
                Console.WriteLine("Listening for TCP connections on port 8765...");

                while (isRunning)
                {
                    // Chấp nhận kết nối từ client
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    NetworkStream stream = tcpClient.GetStream();

                    // Đọc dữ liệu từ stream
                    byte[] buffer = new byte[4096]; // Tạo buffer tạm để nhận dữ liệu
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // Nếu có dữ liệu nhận được, xử lý chúng
                        ProcessReceivedTcpData(buffer.Take(bytesRead).ToArray());
                    }

                    // Đóng kết nối sau khi nhận xong dữ liệu
                    tcpClient.Close();
                }

                // Dừng lắng nghe kết nối khi không còn chạy
                tcpListener.Stop();
            }
            catch (Exception ex)
            {
                if (isRunning)
                {
                    Console.WriteLine("Lỗi khi nhận dữ liệu TCP: " + ex.Message);
                }
            }
        }

        private void ProcessReceivedTcpData(byte[] receivedData)
        {
            // Chuyển dữ liệu byte thành ảnh và cập nhật giao diện
            try
            {
                using (MemoryStream ms = new MemoryStream(receivedData))
                {
                    Image image = Image.FromStream(ms);
                    Console.WriteLine("Received and displaying image via TCP.");
                    UpdatePictureBox(image);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xử lý ảnh TCP: " + ex.Message);
            }
        }

    }
}
