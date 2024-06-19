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
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using OfficeOpenXml;
using System.IO.Compression;
using OfficeOpenXml.Style;

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
        private Thread screenshotThread;
        private NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        private List<List<string>> fullInfoList = new List<List<string>>();
        private List<List<string>> summaryInfoList = new List<List<string>>();
        private ImageList smallImageList;
        private ImageList largeImageList; private volatile bool isRunning = true;
        private List<Dictionary<string, string>> standardInfoList = new List<Dictionary<string, string>>();
        private List<Dictionary<string, string>> privateStandardInfoList = new List<Dictionary<string, string>>();
        public svForm()
        {
            InitializeComponent();
            Ip = getIPServer();
            //InitializeContextMenu();
            InitializeStandard();
            InitializeFullInfoList(30, "F711");
            Console.WriteLine("IP:" + Ip);
            LoadFullInfoListIntoDataGridView(fullInfoList);
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
            //dgv_client.MouseClick += dgv_client_MouseClick;
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


        private void LoadFullInfoListIntoDataGridView(List<List<string>> fullInfoList)
        {
            dgv_client.Rows.Clear(); // Xóa các hàng cũ trước khi thêm mới
            foreach (List<string> infoList in fullInfoList)
            {
                dgv_client.Rows.Add(infoList.ToArray()); // Thêm từng dòng vào DataGridView
            }
        }




        //Function 
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


        private ContextMenuStrip contextMenuStrip;

        //private void InitializeContextMenu()
        //{
        //    contextMenuStrip = new ContextMenuStrip();

        //    ToolStripMenuItem viewDetailsItem = new ToolStripMenuItem("Chế độ xem Chi tiết");
        //    viewDetailsItem.Click += ViewDetailsItem_Click;
        //    contextMenuStrip.Items.Add(viewDetailsItem);

        //    ToolStripMenuItem viewListItem = new ToolStripMenuItem("Chế độ xem Danh sách");
        //    viewListItem.Click += ViewListItem_Click;
        //    contextMenuStrip.Items.Add(viewListItem);

        //    ToolStripMenuItem viewLargeIconItem = new ToolStripMenuItem("Chế độ xem Biểu tượng lớn");
        //    viewLargeIconItem.Click += ViewLargeIconItem_Click;
        //    contextMenuStrip.Items.Add(viewLargeIconItem);

        //    ToolStripMenuItem viewSmallIconItem = new ToolStripMenuItem("Chế độ xem Biểu tượng nhỏ");
        //    viewSmallIconItem.Click += ViewSmallIconItem_Click;
        //    contextMenuStrip.Items.Add(viewSmallIconItem);
        //}

      

        private void sendAllIPInLan()
        {


            // Gửi tin nhắn UDP đến từng địa chỉ IP trong mạng


            IPAddress broadcastAddress = GetBroadcastAddress();


            SendUDPMessage(broadcastAddress, 11312, Ip);

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
                    Console.WriteLine("TCP: " + ex);
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
            else if (tmp[0] == "LOAD_DONE")
            {
                isRunning = true;

                screenshotThread = new Thread(new ThreadStart(CaptureAndSendScreenshotsContinuously));
                screenshotThread.Start();
            }


            // Đóng kết nối khi client đóng kết nối
            //tcpClient.Close();
        }
        public void InitializeStandard()
        {
            string keysString = "Tên máy,Ổ cứng,CPU,RAM,MSSV,IPC,Chuột,Bàn phím,Màn hình";
            // Chuỗi chứa các value cho standardInfoList
            string privateStandardValuesString = "F711-11,500GB,Intel(R) Core(TM) i5-10500 CPU @ 3.10GHz, 16 gb, ,192.168.1.1,đang kết nối,đang kết nối,đang kết nối";
            // Chuỗi chứa các value cho privateStandardInfoList
            string standardValuesString = "F711-02,1TB,Intel i7,16GB\n16GB,654321,192.168.1.2,đang kết nối,đang kết nối,đang kết nối";

            // Tách các key và value từ chuỗi
            var keys = keysString.Split(',');
            var standardValues = standardValuesString.Split(',');
            var privateStandardValues = privateStandardValuesString.Split(',');

            // Khởi tạo standardInfoList
            var standardInfo = new Dictionary<string, string>();
            for (int i = 0; i < keys.Length; i++)
            {
                standardInfo[keys[i]] = standardValues[i];
            }
            // Thay thế ký tự xuống dòng trong chuỗi "16GB\n16GB" bằng ký tự xuống dòng thực sự
            standardInfo["RAM"] = standardInfo["RAM"].Replace("\\n", Environment.NewLine);
            standardInfoList.Add(standardInfo);

            // Khởi tạo privateStandardInfoList
            var privateStandardInfo = new Dictionary<string, string>();
            for (int i = 0; i < keys.Length; i++)
            {
                privateStandardInfo[keys[i]] = privateStandardValues[i];
            }
            privateStandardInfoList.Add(privateStandardInfo);
        }

        public void InitializeFullInfoList(int machineCount, string roomName)
        {
            for (int i = 1; i <= machineCount; i++)
            {
                string machineName = $"{roomName}-{i:D2}";
                var newEntry = new List<string>();

                //Kiểm tra nếu máy tồn tại trong privateStandardInfoList
                var privateInfo = privateStandardInfoList.FirstOrDefault(info => info.ContainsKey("Tên máy") && info["Tên máy"] == machineName);
                if (privateInfo != null)
                {
                    newEntry.Add(privateInfo["Tên máy"]);
                    newEntry.Add(privateInfo["Ổ cứng"]);
                    newEntry.Add(privateInfo["CPU"]);
                    newEntry.Add(privateInfo["RAM"]);
                    newEntry.Add(privateInfo["MSSV"]);
                    newEntry.Add(privateInfo["IPC"]);
                    newEntry.Add(privateInfo["Chuột"]);
                    newEntry.Add(privateInfo["Bàn phím"]);
                    newEntry.Add(privateInfo["Màn hình"]);
                }
                else
                {
                    // Tự lấy thông tin chuẩn và khởi tạo máy nếu máy không tồn tại trong privateStandardInfoList
                    var standardInfo = standardInfoList[0];
                    newEntry.Add(machineName); // "Tên máy"
                    newEntry.Add(standardInfo["Ổ cứng"]);
                    newEntry.Add(standardInfo["CPU"]);
                    newEntry.Add(standardInfo["RAM"]);
                    newEntry.Add(standardInfo["MSSV"]);
                    newEntry.Add(standardInfo["IPC"]);
                    newEntry.Add(standardInfo["Chuột"]);
                    newEntry.Add(standardInfo["Bàn phím"]);
                    newEntry.Add(standardInfo["Màn hình"]);
                }

                fullInfoList.Add(newEntry);
            }
        }

        public Dictionary<string, string> CompareInfo(Dictionary<string, string> newInfo)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, string> matchedInfo = null;

            // Check privateStandardInfoList first
            foreach (var info in privateStandardInfoList)
            {
                if (info.ContainsKey("Tên máy") && newInfo.ContainsKey("Tên máy") && info["Tên máy"] == newInfo["Tên máy"])
                {
                    matchedInfo = info;
                    break;
                }
            }

            // If not found, check standardInfoList
            if (matchedInfo == null)
            {
                foreach (var info in standardInfoList)
                {
                    if (info.ContainsKey("Tên máy") && newInfo.ContainsKey("Tên máy") && info["Tên máy"] == newInfo["Tên máy"])
                    {
                        matchedInfo = info;
                        break;
                    }
                }
            }

            // Compare and update result
            foreach (var key in newInfo.Keys)
            {
                if (key == "Chuột" || key == "Bàn phím" || key == "Màn hình")
                {
                    result[key] = newInfo[key] == "Đã kết nối" ? newInfo[key] + ":1" : newInfo[key] + ":0";
                }
                else
                {
                    if(key!= "Tên máy"&&key!= "MSSV"&&key!="IPC")
                    if (matchedInfo != null && matchedInfo.ContainsKey(key) && matchedInfo[key] == newInfo[key])
                    {
                        result[key] = newInfo[key] + ":1";
                    }
                    else
                    {
                        result[key] = newInfo[key] + ":0";
                    }
                }
            }

            return result;
        }


        public void ReciveInfo(string info)
        {
            Console.WriteLine(info);
            string[] infC = info.Split(new string[] { "Tenmay: ", "MSSV: ", "Ocung: ", "CPU: ", "RAM: ", "IPC: ", "Chuot: ", "Banphim: ", "Manhinh: " }, StringSplitOptions.None);

            List<string> newEntry = new List<string>
    {
        infC[2],  // Tên máy
        infC[6],  // Ổ cứng
        infC[7],  // CPU
        infC[8],  // RAM
        infC[9],  // MSSV
        infC[1],  // IPC
        infC[3],  // Chuột
        infC[4],  // Bàn phím
        infC[5]   // Màn hình
    };

            // Tạo dictionary mới từ danh sách thông tin nhận được
            var newInfoDict = new Dictionary<string, string>
    {
        { "Tên máy", newEntry[0] },
        { "Ổ cứng", newEntry[1] },
        { "CPU", newEntry[2] },
        { "RAM", newEntry[3] },
        { "MSSV", newEntry[4] },
        { "IPC", newEntry[5] },
        { "Chuột", newEntry[6] },
        { "Bàn phím", newEntry[7] },
        { "Màn hình", newEntry[8] }
    };

            // So sánh và cập nhật thông tin
            var comparedInfo = CompareInfo(newInfoDict);

            // Cập nhật danh sách đầy đủ và tóm tắt
            UpdateInfoList(newEntry, comparedInfo);

            

            // Tạo bản sao của danh sách mới để rút gọn thông tin
            List<string> newEntryCopy = new List<string>(newEntry);

            // Xử lý thông tin RAM để tách dung lượng của từng RAM và kết hợp chúng
            string ramInfo = infC[8];
            string combinedRamCapacities = CombineRamCapacities(ramInfo);
            Console.WriteLine(combinedRamCapacities);
            // Cập nhật thông tin RAM đã kết hợp trong danh sách tóm tắt
            newEntryCopy[3] = combinedRamCapacities;
            Console.WriteLine("Ram sau khi tom tat: " + combinedRamCapacities);
            // Kiểm tra và cập nhật thông tin trong danh sách tóm tắt
            UpdateSummaryInfoList(newEntryCopy);

            // Hiển thị dữ liệu trên ListView và so sánh
            AddOrUpdateRowToDataGridView(newEntry);

            Console.WriteLine("length: " + fullInfoList.Count);
        }

        private void UpdateInfoList(List<string> newEntry, Dictionary<string, string> comparedInfo)
        {
            // Cập nhật danh sách tóm tắt
            foreach (var key in comparedInfo.Keys)
            {
                newEntry[GetIndexForKey(key)] = comparedInfo[key];
            }
            // Kiểm tra xem tên máy đã tồn tại trong danh sách hay chưa
            bool entryExists = false;
            for (int i = 0; i < fullInfoList.Count; i++)
            {
                if (fullInfoList[i][0] == newEntry[0]) // So sánh tên máy
                {
                    // Cập nhật thông tin cho danh sách đầy đủ
                    fullInfoList[i] = newEntry;
                    entryExists = true;

                    // Nếu đang ở chế độ xem đầy đủ, cập nhật hiển thị
                    if (isFullInfoMode)
                    {
                        AddOrUpdateRowToDataGridView(newEntry);
                    }
                    break;
                }
            }

            // Nếu tên máy không tồn tại, thêm danh sách mới vào danh sách đầy đủ
            if (!entryExists)
            {
                fullInfoList.Add(newEntry);
                if (isFullInfoMode)
                {
                    AddOrUpdateRowToDataGridView(newEntry);
                }
            }

           
        }

        private int GetIndexForKey(string key)
        {
            switch (key)
            {
                case "Tên máy": return 0;
                case "Ổ cứng": return 1;
                case "CPU": return 2;
                case "RAM": return 3;
                case "MSSV": return 4;
                case "IPC": return 5;
                case "Chuột": return 6;
                case "Bàn phím": return 7;
                case "Màn hình": return 8;
                default: return -1;
            }
        }

        private void UpdateSummaryInfoList(List<string> newEntryCopy)
        {
            bool entryExists = false;
            for (int i = 0; i < summaryInfoList.Count; i++)
            {
                if (summaryInfoList[i][0] == newEntryCopy[0]) // So sánh tên máy
                {
                    // Cập nhật thông tin cho danh sách tóm tắt
                    summaryInfoList[i] = newEntryCopy;
                    entryExists = true;
                    break;
                }
            }

            // Nếu tên máy không tồn tại trong danh sách tóm tắt, thêm danh sách mới
            if (!entryExists)
            {
                summaryInfoList.Add(newEntryCopy);
            }
        }

        private string CombineRamCapacities(string ramInfo)
        {
            List<string> ramCapacities = new List<string>();

            // Tách các thông tin về RAM
            string[] ramModules = ramInfo.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ramModule in ramModules)
            {
                int bytesIndex = ramModule.IndexOf("bytes");
                int capaIndex = ramModule.IndexOf(":");
                if (bytesIndex > 0)
                {
                    string capacityStr = ramModule.Substring(capaIndex + 1, bytesIndex - capaIndex - 1).Trim();
                    if (double.TryParse(capacityStr, out double capacityBytes))
                    {
                        double capacityGB = capacityBytes / (1024 * 1024 * 1024);
                        ramCapacities.Add($"{capacityGB} GB");
                    }
                }
            }

            // Kết hợp các dung lượng RAM vào một chuỗi duy nhất, sử dụng Environment.NewLine để xuống dòng
            return string.Join(Environment.NewLine, ramCapacities);
        }

        private void AddOrUpdateRowToDataGridView(List<string> entry)
        {
            if (dgv_client.InvokeRequired)
            {
                dgv_client.Invoke(new Action(() =>
                {
                    AddOrUpdateRow(entry);
                }));
            }
            else
            {
                AddOrUpdateRow(entry);
            }
        }


        private Dictionary<(int rowIndex, int colIndex), Color> cellColors = new Dictionary<(int rowIndex, int colIndex), Color>();

        private void AddOrUpdateRow(List<string> entry)
        {
            DataGridViewRow row = null;
            int rowIndex = -1;

            // Tìm hàng cần cập nhật hoặc thêm mới
            foreach (DataGridViewRow dgvRow in dgv_client.Rows)
            {
                if (entry[0].Contains(dgvRow.Cells[0].Value.ToString()))
                {
                    row = dgvRow;
                    rowIndex = dgvRow.Index;
                    break;
                }
            }

            if (row != null)
            {
                // Cập nhật dữ liệu nếu dòng đã tồn tại
                for (int i = 0; i < entry.Count; i++)
                {
                    int indexOf = entry[i].LastIndexOf(":");
                    indexOf = indexOf == -1 ? entry[i].Length : indexOf;
                    string newValue = entry[i].Substring(0, indexOf);
                    string tmp = entry[i].Substring(indexOf);

                    // Đối với thông tin RAM, sử dụng Environment.NewLine để xuống dòng
                    if (i == 3) // Ở đây giả sử cột RAM là cột thứ 3 (index là 2)
                    {
                        row.Cells[i].Value = newValue.Replace("\n", Environment.NewLine);
                    }
                    else
                    {
                        row.Cells[i].Value = newValue;
                    }

                    // Cập nhật màu sắc và lưu trạng thái màu
                    if (tmp.Contains("0"))
                    {
                        row.Cells[i].Style.ForeColor = Color.Red;
                        cellColors[(rowIndex, i)] = Color.Red;
                    }
                    else
                    {
                        row.Cells[i].Style.ForeColor = Color.Black;
                        cellColors[(rowIndex, i)] = Color.Black;
                    }
                }
            }
            else
            {
                // Thêm dòng mới nếu không tồn tại
                rowIndex = dgv_client.Rows.Add(entry.ToArray());
                DataGridViewRow newRow = dgv_client.Rows[rowIndex];

                // Kiểm tra và tô màu các ô của dòng mới
                for (int i = 0; i < entry.Count; i++)
                {
                    int indexOf = entry[i].LastIndexOf(":");
                    indexOf = indexOf == -1 ? entry[i].Length : indexOf;
                    string newValue = entry[i].Substring(0, indexOf);
                    string tmp = entry[i].Substring(indexOf);

                    // Đối với thông tin RAM, sử dụng Environment.NewLine để xuống dòng
                    if (i == 3) // Ở đây giả sử cột RAM là cột thứ 3 (index là 2)
                    {
                        newRow.Cells[i].Value = newValue.Replace("\n", Environment.NewLine);
                    }
                    else
                    {
                        newRow.Cells[i].Value = newValue;
                    }

                    // Cập nhật màu sắc và lưu trạng thái màu
                    if (tmp.Contains("0"))
                    {
                        newRow.Cells[i].Style.ForeColor = Color.Red;
                        cellColors[(rowIndex, i)] = Color.Red;
                    }
                    else
                    {
                        newRow.Cells[i].Style.ForeColor = Color.Black;
                        cellColors[(rowIndex, i)] = Color.Black;
                    }
                }
            }

            dgv_client.Refresh();
        }


        private void StopSlideShow_Click(object sender, EventArgs e)
        {
            string message = "CloseSlideShow";
            byte[] data = Encoding.UTF8.GetBytes(message);

            List<Thread> clientThreads = new List<Thread>();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    string clientIP = row.Cells[5].Value.ToString();

                    // Kiểm tra xem địa chỉ IP có hợp lệ không
                    if (IsValidIPAddress(clientIP))
                    {
                        // Tạo một luồng riêng biệt cho mỗi client
                        Thread clientThread = new Thread(() =>
                        {
                            try
                            {
                                TcpClient client = new TcpClient(clientIP, 8888);

                                // Gửi yêu cầu khóa tới máy Client
                                NetworkStream stream = client.GetStream();
                                stream.Write(data, 0, data.Length);
                                // Đóng kết nối
                                client.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                            }
                        });

                        // Bắt đầu luồng cho client hiện tại
                        clientThreads.Add(clientThread);
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }

            isRunning = false;
        }





        private void CaptureAndSendScreenshotsContinuously()
        {
            UdpClient udpClient = new UdpClient(); // Tạo UDP client
            //udpClient.Client.SendBufferSize = 70000;
            int udpBufferSize = udpClient.Client.SendBufferSize;

            // Lấy địa chỉ broadcast của mạng local
            IPAddress broadcastAddress = GetBroadcastAddress();
            int i = 1;
            int fps = 120;
            while (isRunning)
            {
                try
                {
                    // Lấy kích thước của vùng hiển thị thực sự của màn hình
                    int screenLeft = SystemInformation.VirtualScreen.Left;
                    int screenTop = SystemInformation.VirtualScreen.Top;
                    int screenWidth = SystemInformation.VirtualScreen.Width;
                    int screenHeight = SystemInformation.VirtualScreen.Height;

                    using (Bitmap screenshot = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(screenshot))
                        {
                            graphics.CopyFromScreen(screenLeft, screenTop, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy);

                            // Draw the cursor
                            DrawCursorOnScreenshot(graphics);

                            // Compress and send the screenshot
                            using (MemoryStream stream = new MemoryStream())
                            {
                                CompressAndSendImage(i, screenshot, stream, udpBufferSize, udpClient, broadcastAddress);
                            }
                        }
                    }
                    if (i > fps - 1) i = 1;
                    i++;
                    // Chờ khoảng thời gian trước khi chụp và gửi tiếp
                    Thread.Sleep(1000 / fps); // Chờ 1/60 giây (mili giây) trước khi gửi hình tiếp theo
                }
                catch (ThreadInterruptedException ex)
                {
                    // Xử lý ngoại lệ ở đây
                    Console.WriteLine("Thread interrupted: " + ex.Message);
                }
            }
        }

        private void CompressAndSendImage(int i, Bitmap image, MemoryStream stream, int bufferSize, UdpClient udpClient, IPAddress broadcastAddress)
        {
            long quality = 100L; // Bắt đầu với chất lượng 90%

            byte[] imageData;
            do
            {
                stream.SetLength(0); // Đặt lại stream cho mỗi lần nén
                SaveJpeg(stream, image, quality);
                imageData = stream.ToArray();
                quality -= 10; // Giảm chất lượng đi 10% cho lần tiếp theo nếu cần
            } while (imageData.Length > bufferSize && quality > 10);

            int bytesSent = 0;
            int index = 0;
            int cutCount = (imageData.Length / (bufferSize - 100)) + 1;
            int cutCounter = 1;
            while (bytesSent < imageData.Length)
            {
                string str = $"{i:D4}{cutCount:D4}{cutCounter:D4}+";
                byte[] signal = Encoding.UTF8.GetBytes(str);
                int signalLength = signal.Length;

                int remainingBytes = imageData.Length - bytesSent;
                int bytesToSend = Math.Min(bufferSize - signalLength - 100, remainingBytes); // Đảm bảo có chỗ cho tín hiệu

                // Tạo một mảng byte mới để chứa tín hiệu và dữ liệu cần gửi
                byte[] dataToSend = new byte[signalLength + bytesToSend];
                Console.WriteLine(imageData.Length + " " + cutCount + " " + i + " " + quality);
                // Sao chép tín hiệu và dữ liệu vào mảng mới
                Array.Copy(signal, 0, dataToSend, 0, signalLength);
                Array.Copy(imageData, index, dataToSend, signalLength, bytesToSend);

                // Gửi dữ liệu bao gồm tín hiệu
                udpClient.Send(dataToSend, dataToSend.Length, new IPEndPoint(broadcastAddress, 8889)); // Gửi dữ liệu qua UDP

                // Cập nhật số byte đã gửi và vị trí index
                bytesSent += bytesToSend;
                index += bytesToSend;
                cutCounter++;
            }
        }

        private void SaveJpeg(Stream stream, Bitmap image, long quality)
        {
            var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            var jpegCodec = GetEncoder(ImageFormat.Jpeg);
            if (jpegCodec == null)
                return;

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            image.Save(stream, jpegCodec, encoderParams);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void DrawCursorOnScreenshot(Graphics graphics)
        {
            // Get the cursor position
            Point cursorPosition = Cursor.Position;

            // Adjust for the screen bounds
            cursorPosition.X -= SystemInformation.VirtualScreen.Left;
            cursorPosition.Y -= SystemInformation.VirtualScreen.Top;

            // Draw the cursor on the screenshot
            Cursor cursor = Cursors.Default;
            cursor.Draw(graphics, new Rectangle(cursorPosition, cursor.Size));
        }

        // Hàm để lấy địa chỉ broadcast của mạng local
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
            // Tạo kết nối TCP tới máy Client
            TcpClient client = new TcpClient(ipAddress, 8888);

            // Gửi yêu cầu khóa tới máy Client
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes("LOCK_ACCESS");
            stream.Write(data, 0, data.Length);
            MessageBox.Show("Sendlockweb ne");
            // Đóng kết nối
            //stream.Close();
            client.Close();
        }
        private void refresh_Click(object sender, EventArgs e)
        {
            sendAllIPInLan();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            dgv_client.Rows.Clear();
            isFullInfoMode = !isFullInfoMode;

            List<List<string>> targetList = isFullInfoMode ? fullInfoList : summaryInfoList;
            foreach (List<string> entry in targetList)
            {
                int rowIndex = dgv_client.Rows.Add(entry.ToArray());
                DataGridViewRow newRow = dgv_client.Rows[rowIndex];

                // Khôi phục màu sắc từ cellColors
                for (int i = 0; i < entry.Count; i++)
                {
                    if (cellColors.TryGetValue((rowIndex, i), out Color color))
                    {
                        newRow.Cells[i].Style.ForeColor = color;
                    }
                }
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

        //private void dgv_client_MouseClick(object sender, MouseEventArgs e)
        //{

        //    Console.WriteLine("Nhan chuot");
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        Console.WriteLine("Chuot phai ne");
        //        // Xác định vị trí của chuột trên ListView
        //        Point clickPoint = dgv_client.PointToClient(new Point(e.X, e.Y));

        //        // Hiển thị ContextMenuStrip tại vị trí của chuột
        //        contextMenuStrip.Show(dgv_client, e.Location);
        //    }
        //}

        private void dgv_client_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SlideShowClick(object sender, EventArgs e)
        {
            string message = "SlideShow";
            byte[] data = Encoding.UTF8.GetBytes(message);

            List<Thread> clientThreads = new List<Thread>();

            foreach (DataGridViewRow row in dgv_client.Rows)
            {
                try
                {
                    if (row.Cells[5].Value != null)
                    {
                        string clientIP = row.Cells[5].Value.ToString();
                        Console.WriteLine(clientIP);

                        // Kiểm tra xem địa chỉ IP có hợp lệ không
                        if (IsValidIPAddress(clientIP))
                        {
                            // Tạo một luồng riêng biệt cho mỗi client
                            Thread clientThread = new Thread(() =>
                            {
                                try
                                {
                                    TcpClient client = new TcpClient(clientIP, 8888);

                                    // Gửi yêu cầu khóa tới máy Client
                                    NetworkStream stream = client.GetStream();
                                    stream.Write(data, 0, data.Length);
                                    // Đóng kết nối
                                    client.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                                }
                            });

                            // Bắt đầu luồng cho client hiện tại
                            clientThreads.Add(clientThread);
                            clientThread.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                }
            }

            // Chờ tất cả các luồng kết thúc trước khi tiếp tục
            foreach (Thread t in clientThreads)
            {
                t.Join();
            }
        }


        private void sendFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendForm sendForm = new SendForm();
            sendForm.FilesSelected += SendFileForm_FilePathSelected;
            sendForm.Show();
        }

        private void SendFileForm_FilePathSelected(List<string> filePaths, string toPath)
        {
            foreach (string filePath in filePaths)
            {
                // Xử lý từng đường dẫn tệp trong danh sách

                List<Thread> clientThreads = new List<Thread>();

                foreach (DataGridViewRow row in dgv_client.Rows)
                {
                    try
                    {
                        string clientIP = row.Cells[5].Value.ToString();

                        // Kiểm tra xem địa chỉ IP có hợp lệ không
                        if (IsValidIPAddress(clientIP))
                        {
                            // Tạo một luồng riêng biệt cho mỗi client
                            Thread clientThread = new Thread(() =>
                            {
                                TcpClient client = null;
                                NetworkStream stream = null;
                                try
                                {
                                    client = new TcpClient(clientIP, 8888);
                                    stream = client.GetStream();

                                    // Gửi tín hiệu thông báo
                                    string fileName = Path.GetFileName(filePath);
                                    string signal = $"SendFile-{fileName}-{toPath}";
                                    byte[] signalBytes = Encoding.UTF8.GetBytes(signal);
                                    stream.Write(signalBytes, 0, signalBytes.Length); // Gửi tín hiệu
                                    stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức
                                    Console.WriteLine("Đã gửi tín hiệu send");

                                    // Đợi client xác nhận đã nhận tín hiệu
                                    byte[] ackBytes = new byte[4];
                                    stream.Read(ackBytes, 0, 4);
                                    int ack = BitConverter.ToInt32(ackBytes, 0);
                                    if (ack != 1)
                                    {
                                        throw new Exception("Client không xác nhận tín hiệu.");
                                    }

                                    // Gửi nội dung tệp
                                    byte[] fileBytes = File.ReadAllBytes(filePath);
                                    stream.Write(BitConverter.GetBytes(fileBytes.Length), 0, 4); // Gửi độ dài nội dung tệp
                                    stream.Write(fileBytes, 0, fileBytes.Length); // Gửi nội dung tệp
                                    stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức

                                    Console.WriteLine("Tệp đã được gửi thành công.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Lỗi khi gửi tệp: " + ex.Message);
                                }
                                finally
                                {
                                    // Đảm bảo rằng kết nối được đóng đúng cách
                                    if (stream != null) stream.Close();
                                    if (client != null) client.Close();
                                }
                            });

                            // Bắt đầu luồng cho client hiện tại
                            clientThreads.Add(clientThread);
                            clientThread.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
                    }
                }

                // Chờ tất cả các luồng kết thúc trước khi tiếp tục
                foreach (Thread t in clientThreads)
                {
                    t.Join();
                }
            }
        }

        private void reciveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReciveForm sendForm = new ReciveForm();
            sendForm.FilePathSelected += ReciveFileForm_FilePathSelected;
            sendForm.Show();
        }
       
private void ReciveFileForm_FilePathSelected(string filePath, string folderPath, string folderToSavePath, bool check)
{
    // Xử lý đường dẫn tệp nhận được từ FormSendFile
    Console.WriteLine("Đường dẫn tệp nhận được: " + filePath);
    Console.WriteLine("Đường dẫn tệp đến: " + folderPath);
    Console.WriteLine("Đường dẫn tệp đến: " + folderToSavePath);

    List<Thread> clientThreads = new List<Thread>();

    foreach (DataGridViewRow row in dgv_client.Rows)
    {
        try
        {
            if (row.Cells[5].Value != null)
            {
                string clientIP = row.Cells[5].Value.ToString();

                // Kiểm tra xem địa chỉ IP có hợp lệ không
                if (IsValidIPAddress(clientIP))
                {
                    // Tạo một luồng riêng biệt cho mỗi client
                    Thread clientThread = new Thread(() =>
                    {
                        TcpClient client = null;
                        NetworkStream stream = null;
                        try
                        {
                            client = new TcpClient(clientIP, 8888);
                            stream = client.GetStream();

                            // Gửi tín hiệu thông báo
                            string fileName = Path.GetFileName(filePath);
                            string signal = $"CollectFile-{fileName}-{folderPath}-{check}";
                            byte[] signalBytes = Encoding.UTF8.GetBytes(signal);
                            stream.Write(signalBytes, 0, signalBytes.Length); // Gửi tín hiệu
                            stream.Flush(); // Đảm bảo dữ liệu được gửi đi ngay lập tức
                            Console.WriteLine("Đã gửi tín hiệu send");

                            using (BinaryReader reader = new BinaryReader(stream))
                            {
                                try
                                {
                                    // Đọc độ dài của tên file zip
                                    int fileNameLength = reader.ReadInt32();
                                    // Đọc tên file zip
                                    string receivedFileName = Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));

                                    // Đọc độ dài của tệp zip
                                    int fileLength = reader.ReadInt32();
                                    // Đọc dữ liệu tệp zip
                                    byte[] fileBytes = reader.ReadBytes(fileLength);

                                    // Lưu file zip vào đường dẫn tạm thời
                                    string tempZipPath = Path.Combine(Path.GetTempPath(), receivedFileName);
                                    File.WriteAllBytes(tempZipPath, fileBytes);

                                    // Giải nén file zip
                                    ZipFile.ExtractToDirectory(tempZipPath, folderToSavePath);

                                    // Xóa file zip tạm thời
                                    File.Delete(tempZipPath);

                                    MessageBox.Show("Các tệp đã được nhận và lưu thành công tại: " + folderToSavePath);
                                }
                                catch (EndOfStreamException eosEx)
                                {
                                    Console.WriteLine("Lỗi khi nhận tệp: " + eosEx.Message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Lỗi khi nhận tệp: " + ex.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi khi nhận tệp: " + ex.Message);
                        }
                        finally
                        {
                            // Đảm bảo rằng kết nối được đóng đúng cách
                            if (stream != null) stream.Close();
                            if (client != null) client.Close();
                        }
                    });

                    // Bắt đầu luồng cho client hiện tại
                    clientThreads.Add(clientThread);
                    clientThread.Start();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Mất kết nối với: " + row.Cells[0].Value.ToString());
        }
    }

    // Chờ tất cả các luồng kết thúc trước khi tiếp tục
    foreach (Thread t in clientThreads)
    {
        t.Join();
    }
}

        private void sendWork_ButtonClick(object sender, EventArgs e)
        {

        }


        private void ExportToExcel(string fileName)
        {
            try
            {
                // Thiết lập LicenseContext
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                FileInfo newFile = new FileInfo(fileName);
                if (newFile.Exists)
                {
                    newFile.Delete();  // Xóa tệp đã tồn tại
                    newFile = new FileInfo(fileName);
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PhongMayInfo");

                    // Đặt tiêu đề cho các cột
                    for (int i = 1; i <= dgv_client.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i].Value = dgv_client.Columns[i - 1].HeaderText;
                    }

                    // Đổ dữ liệu từ DataGridView vào tệp Excel và kiểm tra sự khác biệt
                    for (int i = 0; i < dgv_client.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgv_client.Rows[i].Cells.Count; j++)
                        {
                            var cell = worksheet.Cells[i + 2, j + 1];
                            cell.Value = dgv_client.Rows[i].Cells[j].Value.ToString();

                            // Kiểm tra nếu ô có màu đỏ thì tô màu trong Excel
                            if (dgv_client.Rows[i].Cells[j].Style.ForeColor == Color.Red)
                            {
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(Color.Red);
                            }
                        }
                    }

                    package.Save();
                }

                Console.WriteLine("Exported data to Excel successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while exporting data to Excel: " + ex.Message);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            string thongTinMayTinh = "InfoClient-IPC: 192.168.0.104Tenmay: F711-11Chuot: Đã kết nốiBanphim: Đã kết nốiManhinh: Đã kết nốiOcung: C:\\, 465 GBCPU: 13th Gen Intel(R) Core(TM) i5-13400FRAM: Capacity: 17179869184 bytes Speed: 3200 Manufacturer: Golden Empire  Part Number: CL16-20-20 D4-3200  |Capacity: 17179869184 bytes Speed: 3200 Manufacturer: Golden Empire  Part Number: CL16-20-20 D4-3200  |MSSV: 1\n2";
            ReciveInfo(thongTinMayTinh);
        }

        private void Export_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Chọn thư mục lưu trữ";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    Console.WriteLine($"{folderPath}");
                    // Hiển thị đường dẫn thư mục đã chọn, ví dụ trên một label
                    ExportToExcel(folderPath + @"\F711-Info.xlsx");
                }
            }
        }

        private void svForm_Click(object sender, EventArgs e)
        {

        }
    }
}