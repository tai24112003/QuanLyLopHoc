using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Server
{
    public partial class GroupForm : Form
    {
        public event Action<string, List<List<string>>> GroupCreated; // Thay đổi kiểu dữ liệu thành List<List<string>>

        public GroupForm(List<List<string>> fullInfoList)
        {
            InitializeComponent();
            InitializeComputers(fullInfoList);
        }

        private List<List<string>> fullInfoList = new List<List<string>>();

        // Khởi tạo danh sách máy tính từ fullInfoList
        public void InitializeComputers(List<List<string>> computerList)
        {
            fullInfoList = computerList;
            listBoxAvailableComputers.Items.Clear();

            foreach (var info in fullInfoList)
            {
                // Thêm tên máy tính (thường là thông tin thứ hai trong danh sách)
                listBoxAvailableComputers.Items.Add(info[0]); // Giả sử tên máy là phần tử thứ 1
            }
        }

        private void btnAddSelected_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(listBoxAvailableComputers, listBoxSelectedComputers);
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            MoveAllItems(listBoxAvailableComputers, listBoxSelectedComputers);
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(listBoxSelectedComputers, listBoxAvailableComputers);
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            MoveAllItems(listBoxSelectedComputers, listBoxAvailableComputers);
        }

        private void btnCreateGroup_Click(object sender, EventArgs e)
        {
            if (listBoxSelectedComputers.Items.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một máy tính để tạo nhóm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Nhập tên nhóm từ người dùng
            string groupName = txtNameGroup.Text;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                MessageBox.Show("Vui lòng nhập tên nhóm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            // Lấy danh sách các máy đã chọn
            var selectedComputers = new List<List<string>>();

            foreach (var item in listBoxSelectedComputers.Items)
            {
                // Tìm thông tin máy tính tương ứng với tên máy trong danh sách fullInfoList
                var computerInfo = fullInfoList.FirstOrDefault(info => info[0] == item.ToString());
                if (computerInfo != null)
                {
                    selectedComputers.Add(computerInfo);
                }
            }

            // Gọi sự kiện GroupCreated để gửi dữ liệu về MainForm
            GroupCreated?.Invoke(groupName, selectedComputers);

            this.Close(); // Đóng form sau khi tạo nhóm
        }

        private void MoveSelectedItems(ListBox source, ListBox destination)
        {
            var selectedItems = source.SelectedItems.Cast<object>().ToList();

            foreach (var item in selectedItems)
            {
                destination.Items.Add(item);
                source.Items.Remove(item);
            }
        }

        private void MoveAllItems(ListBox source, ListBox destination)
        {
            foreach (var item in source.Items)
            {
                destination.Items.Add(item);
            }
            source.Items.Clear();
        }
    }
}
