using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class LockScreenForm : Form
    {
        public bool AllowClose { get; set; } = false;

        public LockScreenForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền của form
            this.WindowState = FormWindowState.Maximized; // Phóng to form ra toàn màn hình
            //this.TopMost = true; // Đặt form ở trên cùng của tất cả các cửa sổ khác

            // Ẩn con trỏ chuột
            //Cursor.Hide();
        }
        public void UnLockScreen()
        {
            // Thực hiện các thao tác dừng slideshow
            AllowClose = true; // Cho phép form được đóng
            this.Close(); // Đóng form
        }

        private void LockScreenForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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
    }
}
