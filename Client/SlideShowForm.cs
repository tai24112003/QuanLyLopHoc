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
        private string protocal;
        private string myIp;
        private bool isRunning;

        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }
        public string Protocal
        {
            get { return protocal; }
            set { protocal = value; }
        }
        public string myIP
        {
            get { return myIp; }
            set { myIp = value; }
        }
        public SlideShowForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền của form
            this.WindowState = FormWindowState.Maximized; // Phóng to form ra toàn màn hình
            //this.TopMost = true; // Đặt form ở trên cùng của tất cả các cửa sổ khác

            // Ẩn con trỏ chuột
            //Cursor.Hide();
            udpClient = new UdpClient(8889); // Khởi tạo UDP client và lắng nghe trên cổng 8889
        }
       
        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            ConnectToServer();
            isRunning = true;
            if (protocal == "UDP")
            {
                udpListenerThread = new Thread(new ThreadStart(ListenForUdpClients));
                udpListenerThread.Start();
            }
            else
            {
                tcpListenerThread = new Thread(new ThreadStart(ListenForTcpClients));
                tcpListenerThread.Start();
            }
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
            if (!AllowClose)
            {
                // Ngăn người dùng đóng form nếu không được phép
                e.Cancel = true;
                MessageBox.Show("Không được tự đóng form");
                return;
            }

            // Đặt cờ dừng
            isRunning = false;

            // Dừng luồng UDP nếu đang sử dụng giao thức UDP
            if (udpListenerThread != null && udpListenerThread.IsAlive)
            {
                udpClient?.Close(); // Đóng UdpClient
                udpListenerThread.Join(); // Chờ luồng hoàn tất
            }

            // Dừng luồng TCP nếu đang sử dụng giao thức TCP
            if (tcpListenerThread != null && tcpListenerThread.IsAlive)
            {
                tcpListener?.Stop(); // Dừng TcpListener
                tcpListenerThread.Join(); // Chờ luồng hoàn tất
            }

            // Hiển thị lại con trỏ chuột khi form bị đóng
            Cursor.Show();
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
                // Khởi tạo TCPListener để lắng nghe kết nối từ client trên cổng 8998
                tcpListener = new TcpListener(IPAddress.Parse(myIp), 8998);
                tcpListener.Start();
                Console.WriteLine("Listening for TCP connections on port 8998...");

                while (isRunning)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    Console.WriteLine("Đã kết nối với một client.");

                    Thread clientThread = new Thread(() =>
                    {
                        HandleClient(tcpClient); // Xử lý client trong một thread riêng
                    });
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                if (isRunning)
                {
                    Console.WriteLine("Lỗi khi nhận dữ liệu TCP: " + ex.Message);
                }
            }
        }
        private void HandleClient(TcpClient tcpClient)
        {
            try
            {
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    while (true)
                    {
                        // Bước 1: Đọc 4 byte đầu tiên để lấy độ dài dữ liệu
                        byte[] lengthBuffer = new byte[4];
                        int bytesRead = 0;

                        // Đọc đủ 4 byte để lấy độ dài dữ liệu
                        while (bytesRead < 4)
                        {
                            int read = stream.Read(lengthBuffer, bytesRead, 4 - bytesRead);
                            if (read == 0)
                            {
                                Console.WriteLine("Kết nối bị đóng.");
                                return;
                            }
                            bytesRead += read;
                        }

                        int imageDataLength = BitConverter.ToInt32(lengthBuffer, 0);
                        if (imageDataLength <= 0)
                        {
                            Console.WriteLine("Độ dài dữ liệu không hợp lệ. Đóng kết nối.");
                            break;
                        }

                        // Bước 2: Đọc dữ liệu ảnh
                        byte[] imageData = new byte[imageDataLength];
                        bytesRead = 0;

                        while (bytesRead < imageDataLength)
                        {
                            int chunkSize = stream.Read(imageData, bytesRead, imageDataLength - bytesRead);
                            if (chunkSize == 0) // Kết nối bị đóng trước khi nhận đủ dữ liệu
                            {
                                Console.WriteLine("Kết nối bị đóng bất ngờ.");
                                return;
                            }
                            bytesRead += chunkSize;
                        }
                        Console.WriteLine($"Đã nhận đủ {bytesRead}/{imageDataLength} byte.");

                        if (bytesRead == imageDataLength)
                        {
                            // Bước 3: Xử lý ảnh và hiển thị
                            ProcessReceivedTcpData(imageData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi trong quá trình xử lý client: " + ex.Message);
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private void ProcessReceivedTcpData(byte[] receivedData)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(receivedData))
                {
                    Image image = Image.FromStream(ms);
                    Console.WriteLine("Nhận và hiển thị ảnh qua TCP.");
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
