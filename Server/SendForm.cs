using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class SendForm : Form
    {
        public event Action<List<string>, string> FilesSelected;

        public SendForm()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "d:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true; // Allow selecting multiple files

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] filePaths = openFileDialog.FileNames;
                    // Display selected file paths
                    txtFilePath.Text = string.Join(Environment.NewLine, filePaths);
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            // Lấy danh sách file từ TextBox
            string[] filePaths = txtFilePath.Text
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Lấy đường dẫn đích từ TextBox
            string toPath = txtToPath.Text.Trim();

            // Validate danh sách file
            if (filePaths.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một tệp để gửi.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (string filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Tệp không tồn tại: {filePath}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Validate đường dẫn đích
            if (string.IsNullOrWhiteSpace(toPath))
            {
                MessageBox.Show("Vui lòng nhập đường dẫn đích.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Nếu mọi thứ hợp lệ, kích hoạt sự kiện và đóng form
            FilesSelected?.Invoke(filePaths.ToList(), toPath);
            this.Close();
        }


        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
