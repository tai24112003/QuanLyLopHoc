using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Server
{
    public partial class QuickLaunchForm : Form
    {
        public event Action<string> FilesSelected;

        public QuickLaunchForm()
        {
            InitializeComponent();
            InitializeListView();
        }

        private void InitializeListView()
        {
            listView1.LargeImageList = new ImageList(); // Tạo ImageList cho LargeIcon
            listView1.LargeImageList.ImageSize = new Size(64, 64); // Kích thước ảnh
            listView1.View = View.LargeIcon; // Đặt chế độ hiển thị là LargeIcon
            listView1.MultiSelect = false; // Chỉ cho phép chọn một mục cùng lúc

            // Sự kiện khi nhấn chọn một mục trong ListView
            listView1.ItemSelectionChanged += ListView1_ItemSelectionChanged;
        }

        // Xử lý sự kiện khi chọn một item trong ListView
        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                textBox1.Text = e.Item.Tag.ToString(); // Hiển thị đường dẫn của tệp trong TextBox
            }
        }

        // Xử lý sự kiện nhấn nút +
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName; // Lấy đường dẫn tệp đã chọn

                    // Tạo icon từ đường dẫn tệp
                    Icon fileIcon = Icon.ExtractAssociatedIcon(filePath);
                    Bitmap fileBitmap = fileIcon.ToBitmap();

                    // Thêm icon vào ImageList và thêm item mới vào ListView
                    string fileName = Path.GetFileName(filePath);
                    listView1.LargeImageList.Images.Add(fileName, fileBitmap);

                    ListViewItem item = new ListViewItem
                    {
                        Text = fileName, // Hiển thị tên tệp
                        ImageKey = fileName, // Sử dụng ImageKey để liên kết với hình ảnh
                        Tag = filePath // Lưu đường dẫn trong Tag của item
                    };
                    listView1.Items.Add(item);
                }
            }
        }

        // Xử lý sự kiện nhấn nút -
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                listView1.Items.Remove(selectedItem); // Xóa mục đã chọn
                textBox1.Clear(); // Xóa TextBox
            }
            else
            {
                MessageBox.Show("Vui lòng chọn mục cần xóa.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filePaths = textBox1.Text.Trim();
            if (filePaths.Length != 0)
            {
                FilesSelected?.Invoke(filePaths); // Trigger event with selected file paths
            }
        }
    }
}
