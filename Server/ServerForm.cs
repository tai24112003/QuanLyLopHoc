using System;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsTimer = System.Windows.Forms.Timer;


namespace Server
{
    public partial class svForm : Form
    {
        private WinFormsTimer timer;
        private string hostName;
        private string Ip;
        private TcpListener tcpListener;
        private Thread listenThread;
        private NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        public svForm()
        {
            InitializeComponent();
            Ip = getIPServer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sendAllIPInLan();
            timer = new WinFormsTimer();
            timer.Interval = 5000;
            // Gán sự kiện xảy ra khi Timer đã chạy đủ thời gian
            timer.Tick += OnTimerTick;

            // Bắt đầu Timer
            timer.Start();

            IPAddress ip = IPAddress.Parse(Ip);
            int tcpPort = 8765;

            // Tạo đối tượng TcpListener để lắng nghe kết nối từ client
            tcpListener = new TcpListener(ip, tcpPort);
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
        }
        private void serverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpListener.Stop();
        }






        //Function 
        private string getIPServer()
        {
            // Lấy tên máy tính hiện tại
            hostName = Dns.GetHostName();
            string ip = string.Empty;
            // Lấy địa chỉ IP của máy tính hiện tại
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);


            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ip = address.ToString();
            }
            return ip;
        }

        private void sendAllIPInLan()
        {
            if (Ip.Length == 0) return;
            // IPAddress[] allIPsInNetwork = GetAddresses();
            String[] IpC = Ip.Split('.');

            // Gửi tin nhắn UDP đến từng địa chỉ IP trong mạng
            for (int i = 2; i < 255; i++)
            {
                string ipAddressString = $"{IpC[0]}.{IpC[1]}.{IpC[2]}.{i}";

                // Tạo đối tượng IPAddress từ chuỗi
                IPAddress ipAddress;
                if (IPAddress.TryParse(ipAddressString, out ipAddress))
                {
                    //Console.WriteLine($"Địa chỉ IP hợp lệ: {ipAddress}");
                }
                else
                {
                    Console.WriteLine("Chuỗi không phải là một địa chỉ IP hợp lệ.");
                }
                SendUDPMessage(ipAddress, 11312, Ip);
            }
        }
        private void SendUDPMessage(IPAddress ipAddress, int port, String mes)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);

                string messageToSend = mes;
                byte[] data = Encoding.UTF8.GetBytes(messageToSend);

                udpClient.Send(data, data.Length, remoteEndPoint);

                //Console.WriteLine($"Sent message to {ipAddress}");
                udpClient.Close();
            }

        }
        private void OnTimerTick(object sender, EventArgs e)
        {

            // Gửi tin nhắn UDP đến từng địa chỉ IP trong mạng
            sendAllIPInLan();
            // Dừng Timer sau khi đã thực hiện công việc
            //timer.Stop();
        }
        private void ListenForClients()
        {
            tcpListener.Start();

            Console.WriteLine("Server is listening for clients...");

            while (true)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

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
            string[] infC = receivedMessage.Split(new string[] { "Tenmay: ", "MSSV: ", "Ocung: ","CPU: ","RAM: ","IPC: " }, StringSplitOptions.None);
             
          

            if (infC.Length >= 6)
            {
                AddOrUpdateRowToDataGridView(infC[2], infC[3], infC[4], infC[5], infC[6], infC[1]);
            }
            //else
            //{
            //   // AddOrUpdateRowToDataGridView( infC[2], infC[3], infC[4],infC[5], "", infC[1]);
            // }
                // Đóng kết nối khi client đóng kết nối
                tcpClient.Close();
            
        }

        

        private void AddOrUpdateRowToDataGridView(string Tenmay, string Ocung,string cpu, string ram, string MSSV, string IPC)
        {

            if (dgv_lst_client.InvokeRequired)
            {
                dgv_lst_client.Invoke(new Action(() =>
                {
                    foreach(DataGridViewRow row in dgv_lst_client.Rows)
                    {
                        if (row.Cells["tenmay"].Value != null)
                        {
                            string cellValue = row.Cells["tenmay"].Value.ToString();
                            if (cellValue == Tenmay)
                            {
                                row.Cells["tenmay"].Value = Tenmay;
                                row.Cells["Ocung"].Value = Ocung;
                                row.Cells["cpu"].Value = cpu;
                                row.Cells["ram"].Value = ram;
                                row.Cells["mssv"].Value = MSSV;
                                row.Cells["IPC"].Value = IPC;
                                return;
                            }
                        }
                    }
                    dgv_lst_client.Rows.Add(Tenmay, Ocung,cpu,ram, MSSV, IPC);
                }));
            }
            else
            {
                foreach (DataGridViewRow row in dgv_lst_client.Rows)
                {
                    string cellValue = row.Cells["tenmay"].Value.ToString();
                    if (cellValue == Tenmay)
                    {
                        row.Cells["tenmay"].Value = Tenmay;
                        row.Cells["Ocung"].Value = Ocung;
                        row.Cells["cpu"].Value = cpu;
                        row.Cells["ram"].Value = ram;
                        row.Cells["mssv"].Value = MSSV;
                        row.Cells["IPC"].Value = IPC;

                        return;
                    }
                }
                dgv_lst_client.Rows.Add(Tenmay, Ocung,cpu,ram, MSSV, IPC);
            }
            // Thêm một dòng mới vào DataGridView

        }

        private void Lock_Click(object sender, EventArgs e)
        {
            // Lấy thông tin máy Client đã chọn từ DataGridView
            string selectedIPAddress = dgv_lst_client.SelectedRows[0].Cells["IPC"].Value.ToString();

            // Gửi yêu cầu khóa tới máy Client
            SendLockRequestToClient(selectedIPAddress);

        
        }
        private void SendLockRequestToClient(string ipAddress)
        {
            try
            {
                // Tạo kết nối TCP tới máy Client
                TcpClient client = new TcpClient(ipAddress, 8888);

                // Gửi yêu cầu khóa tới máy Client
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes("LOCK_ACCESS");
                stream.Write(data, 0, data.Length);
                // Đóng kết nối
                //stream.Close();
                client.Close();
            }catch(Exception e)
            {
                MessageBox.Show("Mất kết nối với máy khách","Cảnh báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            sendAllIPInLan();
        }
    }
}
