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
        private bool isFullInfoMode = true;
        private WinFormsTimer timer;
        private string hostName;
        private string Ip;
        private TcpListener tcpListener;
        private Thread listenThread;
        private NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        private List<List<string>> fullInfoList = new List<List<string>>();
        private List<List<string>> summaryInfoList = new List<List<string>>();
        private ImageList smallImageList;
        private ImageList largeImageList;
        public svForm()
        {
            InitializeComponent();
            Ip = getIPServer();
            InitializeContextMenu();
            
            smallImageList = new ImageList();
            largeImageList = new ImageList();
            // Thêm biểu tượng "user" vào ImageList
            largeImageList.ImageSize = new Size(100, 100);
            smallImageList.Images.Add("user", Properties.Resources.user);
            largeImageList.Images.Add("user", Properties.Resources.user);
            lv_client.SmallImageList=smallImageList;
            lv_client.LargeImageList=largeImageList;
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
            lv_client.MouseClick += lv_client_MouseClick;
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

        private ContextMenuStrip contextMenuStrip;

        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem viewDetailsItem = new ToolStripMenuItem("Chế độ xem Chi tiết");
            viewDetailsItem.Click += ViewDetailsItem_Click;
            contextMenuStrip.Items.Add(viewDetailsItem);

            ToolStripMenuItem viewListItem = new ToolStripMenuItem("Chế độ xem Danh sách");
            viewListItem.Click += ViewListItem_Click;
            contextMenuStrip.Items.Add(viewListItem);

            ToolStripMenuItem viewLargeIconItem = new ToolStripMenuItem("Chế độ xem Biểu tượng lớn");
            viewLargeIconItem.Click += ViewLargeIconItem_Click;
            contextMenuStrip.Items.Add(viewLargeIconItem);

            ToolStripMenuItem viewSmallIconItem = new ToolStripMenuItem("Chế độ xem Biểu tượng nhỏ");
            viewSmallIconItem.Click += ViewSmallIconItem_Click;
            contextMenuStrip.Items.Add(viewSmallIconItem);
        }

        private void ViewDetailsItem_Click(object sender, EventArgs e)
        {
            lv_client.View = View.Details;
        }

        private void ViewListItem_Click(object sender, EventArgs e)
        {
            lv_client.View = View.List;
        }

        private void ViewLargeIconItem_Click(object sender, EventArgs e)
        {
            lv_client.View = View.LargeIcon;
        }

        private void ViewSmallIconItem_Click(object sender, EventArgs e)
        {
            lv_client.View = View.SmallIcon;
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

            string[] tmp = receivedMessage.Split(new char[] { '-' }, 2);


            if (tmp[0] == "InfoClient")
            {
                ReciveInfo(tmp[1]);
            }

            
            // Đóng kết nối khi client đóng kết nối
            tcpClient.Close();
        }

        private void ReciveInfo(string info)
        {
            Console.WriteLine(info);
            string[] infC = info.Split(new string[] { "Tenmay: ", "MSSV: ", "Ocung: ", "CPU: ", "RAM: ", "IPC: ", "Chuot: ", "Banphim: ", "Manhinh: " }, StringSplitOptions.None);
           
            //Tạo danh sách mới
            List<string> newEntry = new List<string>();
            //Thêm thông tin vào danh sách mới
            newEntry.Add(infC[2]);  // Tên máy
            newEntry.Add(infC[6]);  // Ổ cứng
            newEntry.Add(infC[7]);  // CPU
            newEntry.Add(infC[8]);  // RAM
            newEntry.Add(infC[9]);  // MSSV
            newEntry.Add(infC[1]);  // IPC
            newEntry.Add(infC[3]);  // Chuột
            newEntry.Add(infC[4]);  // Bàn phím
            newEntry.Add(infC[5]);  // Màn hình

            // Thêm danh sách mới vào danh sách đầy đủ và tóm tắt
            fullInfoList.Add(newEntry);
            if (isFullInfoMode)
                AddOrUpdateRowToListView(newEntry);
            Console.WriteLine(newEntry[3]);
            List<string> newEntryCopy = new List<string>(newEntry);
            //Làm ngắn thông tin RAM chỉ chừa lại dung lượng 
            int capacityIndex = infC[8].IndexOf("Capacity:");
            int bytesIndex = infC[8].IndexOf("bytes", capacityIndex);
            string capacityStr = infC[8].Substring(capacityIndex + "Capacity:".Length, bytesIndex - capacityIndex - "Capacity:".Length).Trim();
            double capacityBytes = double.Parse(capacityStr);
            double capacityGB = capacityBytes / (1024 * 1024 * 1024);
            Console.WriteLine("length: " + fullInfoList.Count);

            newEntryCopy[3] = capacityGB + " GB";
            Console.WriteLine(newEntryCopy[3]);
            //Xử lí thông tin cần rút gọn tiếp tục ở đây
            //
            summaryInfoList.Add(newEntryCopy);

            // Hiển thị dữ liệu trên DataGridView
            if (!isFullInfoMode)
                AddOrUpdateRowToListView(newEntry);
            

        }
        private void AddOrUpdateRowToListView(List<string> entry)
        {
            if (lv_client.InvokeRequired)
            {
                lv_client.Invoke(new Action(() =>
                {
                    ListViewItem item = null;
                    foreach (ListViewItem listItem in lv_client.Items)
                    {
                        if (listItem.SubItems[0].Text == entry[0])
                        {
                            item = listItem;
                            break;
                        }
                    }
                    if (item != null)
                    {
                        // Cập nhật dữ liệu nếu dòng đã tồn tại
                        for (int i = 0; i < entry.Count; i++)
                        {
                            item.SubItems[i].Text = entry[i];
                        }
                    }
                    else
                    {
                        // Thêm dòng mới nếu không tồn tại
                        item = new ListViewItem(entry.ToArray());
                        item.ImageIndex = 0;
                        lv_client.Items.Add(item);
                    }
                }));
            }
            else
            {
                ListViewItem item = null;
                foreach (ListViewItem listItem in lv_client.Items)
                {
                    if (listItem.SubItems[0].Text == entry[0])
                    {
                        item = listItem;
                        break;
                    }
                }
                if (item != null)
                {
                    // Cập nhật dữ liệu nếu dòng đã tồn tại
                    for (int i = 0; i < entry.Count; i++)
                    {
                        item.SubItems[i].Text = entry[i];
                    }
                }
                else
                {
                    // Thêm dòng mới nếu không tồn tại
                    item = new ListViewItem(entry.ToArray());
                    lv_client.Items.Add(item);
                }
            }
        }

        private void AddRow(DataGridView dataGridView, List<string> entry)
        {
            dataGridView.Rows.Add(entry.ToArray());
        }

        private void UpdateRow(DataGridViewRow row, List<string> entry)
        {
            row.Cells["tenmay"].Value = entry[0];
            row.Cells["Ocung"].Value = entry[1];
            row.Cells["cpu"].Value = entry[2];
            row.Cells["ram"].Value = entry[3];
            row.Cells["mssv"].Value = entry[4];
            row.Cells["IPC"].Value = entry[5];
            row.Cells["chuot"].Value = entry[6];
            row.Cells["banphim"].Value = entry[7];
            row.Cells["manhinh"].Value = entry[8];
        }


        private void SlideShowClick(object sender, EventArgs e)
        {
            // Tạo và bắt đầu một luồng mới
            string message = "SlideShow";
            byte[] messageData = Encoding.UTF8.GetBytes(message);
            foreach (ListViewItem item in lv_client.Items)
            {
                // Lấy địa chỉ IP từ cột IPC
                Console.WriteLine(item.SubItems[5].Text);
                string clientIP = item.SubItems[5].Text;

                // Kiểm tra xem địa chỉ IP có hợp lệ không
                if (IsValidIPAddress(clientIP))
                {
                    // Tạo kết nối tới client
                    Console.WriteLine("IP HOP LE");
                    TcpClient client = new TcpClient(clientIP, 8888);
                    NetworkStream stream = client.GetStream();
                    stream.Write(messageData, 0, messageData.Length); // Gửi thông điệp
                }
            }
            //Thread screenshotThread = new Thread(new ThreadStart(CaptureAndSendScreenshot));
            //screenshotThread.Start();
        }

        private void CaptureAndSendScreenshot()
        {
            // Lặp qua từng mục trong ListView
            foreach (ListViewItem item in lv_client.Items)
            {
                // Lấy địa chỉ IP từ cột IPC
                string clientIP = item.SubItems["IPC"].Text;

                // Kiểm tra xem địa chỉ IP có hợp lệ không
                if (IsValidIPAddress(clientIP))
                {
                    // Tạo kết nối tới client
                    TcpClient client = new TcpClient(clientIP, 8888);

                    // Chụp màn hình
                    Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    Graphics graphics = Graphics.FromImage(screenshot as Image);
                    graphics.CopyFromScreen(0, 0, 0, 0, screenshot.Size);

                    // Chuyển đổi hình ảnh thành mảng byte
                    ImageConverter converter = new ImageConverter();
                    byte[] imageData = (byte[])converter.ConvertTo(screenshot, typeof(byte[]));

                    // Gửi dữ liệu màn hình tới client
                    NetworkStream stream = client.GetStream();
                    stream.Write(imageData, 0, imageData.Length);

                    // Đóng kết nối
                    client.Close();
                }
            }
        }

        private bool IsValidIPAddress(string ipAddress)
        {
            IPAddress tempIPAddress;
            return IPAddress.TryParse(ipAddress, out tempIPAddress);
        }


        private void Lock_Click(object sender, EventArgs e)
        {

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
            }
            catch (Exception e)
            {
                MessageBox.Show("Mất kết nối với máy khách", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            sendAllIPInLan();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            lv_client.Items.Clear();
            isFullInfoMode = !isFullInfoMode;
            LogElementFromList(fullInfoList, 0, 3);
            LogElementFromList(summaryInfoList, 0, 3);
            List<List<string>> targetList = isFullInfoMode ? fullInfoList : summaryInfoList;
            foreach (List<string> entry in targetList)
            {
                lv_client.Items.Add(new ListViewItem(entry.ToArray()));
            }
        }
        private void LogElementFromList(List<List<string>> myList, int rowIndex, int colIndex)
        {
            if (rowIndex < 0 || rowIndex >= myList.Count)
            {
                Console.WriteLine("Index hàng nằm ngoài phạm vi của danh sách.");
                return;
            }

            List<string> row = myList[rowIndex];
            if (colIndex < 0 || colIndex >= row.Count)
            {
                Console.WriteLine("Index cột nằm ngoài phạm vi của hàng này.");
                return;
            }

            string element = row[colIndex];
            Console.WriteLine("Phần tử ở hàng {0}, cột {1} là: {2}", rowIndex, colIndex, element);
        }

        private void lv_client_MouseClick(object sender, MouseEventArgs e)
        {

            Console.WriteLine("Nhan chuot");
            if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine("Chuot phai ne");
                // Xác định vị trí của chuột trên ListView
                Point clickPoint = lv_client.PointToClient(new Point(e.X, e.Y));

                // Hiển thị ContextMenuStrip tại vị trí của chuột
                contextMenuStrip.Show(lv_client, e.Location);
            }
        }

        private void lv_client_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
