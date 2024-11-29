using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                textBox1.Text = e.Item.Tag.ToString(); // Hiển thị đường dẫn của tệp trong TextBox
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    // Kiểm tra file đã tồn tại trong danh sách chưa
                    if (listView1.Items.Cast<ListViewItem>().Any(item => item.Tag.ToString() == filePath))
                    {
                        MessageBox.Show("File đã tồn tại trong danh sách.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        // Tạo icon từ đường dẫn tệp
                        Icon fileIcon = Icon.ExtractAssociatedIcon(filePath);
                        Bitmap fileBitmap = fileIcon?.ToBitmap() ?? new Bitmap(64, 64); // Dùng Bitmap rỗng nếu icon null

                        // Thêm icon vào ImageList và thêm mục mới vào ListView
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
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi thêm tệp: {ex.Message}",
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

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
                MessageBox.Show("Vui lòng chọn mục cần xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            string filePaths = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(filePaths))
            {
                FilesSelected?.Invoke(filePaths); // Kích hoạt sự kiện với đường dẫn tệp đã chọn
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tệp trước khi khởi động.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void QuickLaunchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            listView1.ItemSelectionChanged -= ListView1_ItemSelectionChanged; // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ
        }
    }
}
