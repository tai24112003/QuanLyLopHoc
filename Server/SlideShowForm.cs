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


        private void SlideShowForm_MouseMove(object sender, MouseEventArgs e)
        {

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
            pictureBox1.MouseMove += SlideShowForm_MouseMove;
            // Ẩn con trỏ chuột
            //Cursor.Hide();
            udpClient = new UdpClient(8889); // Khởi tạo UDP client và lắng nghe trên cổng 8889
        }

        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            ConnectToClient();
            udpListenerThread = new Thread(new ThreadStart(ListenForUdpClients));
            udpListenerThread.Start();
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }

        private void ConnectToClient()
        {
            SendSignal("LOAD_DONE");

        }


        private void SendSignal(string signal)
        {
            try
            {
                TcpClient client = new TcpClient(ClientIP, 8888);

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
                        //Console.WriteLine($"Received part {cutIndex}/{cutCount} of image {imageIndex}");

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
                                //Console.WriteLine($"Displaying image {imageIndex}"); // Thông báo hiển thị ảnh
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

            // Hiển thị lại con trỏ chuột khi form bị đóng
            Cursor.Show();

        }
        public void StopSlideshow()
        {
            // Thực hiện các thao tác dừng slideshow
            AllowClose = true; // Cho phép form được đóng
            this.Close(); // Đóng form
        }



        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) SendSignal("MouseLeft");
            else if (e.Button == MouseButtons.Right) SendSignal("MouseRight");

        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            Console.WriteLine(e.Delta.ToString());
            SendSignal("MouseWheel_" + e.Delta.ToString());

        }



        private void SlideShowForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control || e.Alt || e.Shift)
            {
                // Phím kết hợp
                string modifierKeys = "";

                if (e.Control)
                {
                    modifierKeys += "Ctrl + ";
                }
                if (e.Alt)
                {
                    modifierKeys += "Alt + ";
                }
                if (e.Shift)
                {
                    modifierKeys += "Shift + ";
                }

                string combinedKey = modifierKeys + e.KeyCode.ToString();
                Console.WriteLine("Phím kết hợp: " + combinedKey);

                // Gửi tổ hợp phím qua mạng
                SendSignal(combinedKey);
            }
            else
            {
                // Phím đơn
                string singleKey = e.KeyCode.ToString();
                Console.WriteLine("Phím đơn: " + singleKey);

                // Gửi phím đơn qua mạng
                SendSignal(singleKey);
            }
        }





    }
}
