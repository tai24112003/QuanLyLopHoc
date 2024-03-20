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

namespace testUdpTcp
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }
        private UdpClient udpClient;
        private Thread udpReceiverThread;
        private string IpServer = "";
        private List<string> inf;
        private List<string> mssvLst = new List<string>();
        private bool sended = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            udpClient = new UdpClient(11312);
            udpReceiverThread = new Thread(new ThreadStart(ReceiveDataOnce));
            udpReceiverThread.Start();
            inf = GetDeviceInfo();
            updateBox();
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
        private List<string> GetDeviceInfo()
        {
            List<string> stringList = new List<string>();
            // Lấy thông tin về tên máy
            string machineName = Environment.MachineName;
            stringList.Add($"Tenmay: {machineName}Ocung: ");
           

            // Lấy thông tin về ổ cứng
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    stringList.Add($"{drive.Name}, Dungluong: {drive.TotalSize / (1024 * 1024)} MB");
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
                    
                    // Lấy luồng mạng từ TcpClient
                    NetworkStream stream = client.GetStream();
                    SendData(stream, string.Join("", inf.ToArray()));
                    byte[] buffer = new byte[1024];
                    sended = true;// Định kích thước buffer tùy ý
                    client.Close();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Lỗi");
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
        }
    }
}
