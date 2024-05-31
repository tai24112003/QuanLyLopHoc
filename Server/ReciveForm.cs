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

            string filePath = txtCollect.Text;
            string folderPath = txtFolder.Text;
            string folderRecivePath = txtFolderRecive.Text;
            bool check = cbDelete.Checked;
            FilePathSelected?.Invoke(filePath, folderPath,folderRecivePath, check); // Gọi sự kiện với đường dẫn tệp được chọn
            this.Close();
        }
    }
}
