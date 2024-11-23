using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class ReciveForm : Form
    {
        public event Action<string, string, string, bool> FilePathSelected;
        public ReciveForm()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Chọn thư mục lưu trữ";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    // Hiển thị đường dẫn thư mục đã chọn, ví dụ trên một label
                    txtFolderRecive.Text = folderPath;
                }
            }
        }

        private void btnCollect_Click(object sender, EventArgs e)
        {
            string filePath = txtCollect.Text.Trim();
            string folderPath = txtFolder.Text.Trim();
            string folderRecivePath = txtFolderRecive.Text.Trim();
            bool check = cbDelete.Checked;

            // Kiểm tra các trường hợp bị bỏ trống
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Vui lòng nhập tên tệp cần thu thập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Vui lòng nhập đường dẫn thư mục nguồn của tệp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(folderRecivePath))
            {
                MessageBox.Show("Vui lòng chọn thư mục đích để lưu trữ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Nếu tất cả hợp lệ, thực hiện sự kiện
            FilePathSelected?.Invoke(filePath, folderPath, folderRecivePath, check);
            this.Close();
        }

    }
}
