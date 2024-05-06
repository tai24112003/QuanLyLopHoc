using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
        private TcpClient client;
        private NetworkStream stream;
        private bool isLoaded = false;
        public SlideShowForm()
        {
            InitializeComponent();
        }

        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            // Kết nối đến máy chủ và gửi tín hiệu load xong
            ConnectToServer();
            SendLoadDoneSignal();
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

        private void ReceiveImage()
        {
            while (true)
            {
                try
                {
                    // Đọc dữ liệu hình ảnh từ máy chủ
                    byte[] imageData = new byte[1024];
                    int bytesRead = stream.Read(imageData, 0, imageData.Length);

                    // Xử lý hình ảnh nhận được và cập nhật giao diện của form
                    if (bytesRead > 0)
                    {
                        // Decode imageData và hiển thị hình ảnh lên form
                        // Ví dụ: pictureBox.Image = DecodeImage(imageData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error receiving image: " + ex.Message);
                }
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
