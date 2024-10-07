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

namespace Server
{
    public partial class SlideShowForm : Form
    {
        public bool AllowClose { get; set; } = false;
        private UdpClient udpClient;
        private Thread udpListenerThread;
        private string ClientIP;
        public string clientIP
        {
            get { return ClientIP; }
            set { ClientIP = value; }
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

        private void SlideShowForm_MouseMove(object sender, MouseEventArgs e)
        {
            IPAddress broadcastAddress = GetBroadcastAddress();

            // Lấy tọa độ chuột
            Point mousePosition = e.Location;

            // Chuẩn bị thông điệp chứa tọa độ chuột
            string message = $"Mouse-{mousePosition.X},{mousePosition.Y}";
            SendSignal(message);
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
            ConnectToClient();
            udpListenerThread = new Thread(new ThreadStart(ListenForUdpClients));
            udpListenerThread.Start();
        }

        private void ConnectToClient()
        {
            SendSignal("LOAD_DONE");

        }


        private void SendSignal(string signal)
        {
            try
            {
                TcpClient client = new TcpClient(ClientIP, 8765);

                // Gửi yêu cầu khóa tới máy Client
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(signal);
                stream.Write(data, 0, data.Length);
                // Đóng kết nối
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to Client: " + ex.Message);
            }
        }
        private void ListenForUdpClients()
        {
            Dictionary<int, List<byte[]>> imagesData = new Dictionary<int, List<byte[]>>();

            while (true)
            {
                try
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
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi nhận dữ liệu UDP: " + ex.Message);
                }
            }
        }

        // Phương thức cập nhật PictureBox với ảnh mới
        private void UpdatePictureBox(Image image)
        {
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new Action<Image>(UpdatePictureBox), image);
                return;
            }

            pictureBox1.Image = image;
        }

        private void SlideShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
                
            }
            if (!AllowClose)
            {
                // Nếu không được phép đóng, hủy sự kiện đóng form
                e.Cancel = true;
                MessageBox.Show("Form cannot be closed from here.");
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
    }
}
