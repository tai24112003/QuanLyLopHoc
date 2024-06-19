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
    public partial class CreateExam : Form
    {
        private ContextMenuStrip contextMenuStrip;
        private string placeholderSearch = "Tìm kiếm";
        public CreateExam()
        {
            InitializeComponent();
            InitializeContextMenu();
            rdExam.Checked = true;
            txtSearch.Text = placeholderSearch;
            txtSearch.ForeColor = SystemColors.GrayText;
        }

        private void rdExam_CheckedChanged(object sender, EventArgs e)
        {
            if (rdExam.Checked)
            {
                lstExam.Visible = true;
                lstQuestion.Visible = false;
            }
        }

        private void rdQuestion_CheckedChanged(object sender, EventArgs e)
        {
            if (rdQuestion.Checked)
            {
                lstExam.Visible = false;
                lstQuestion.Visible = true;
            }
        }
        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem viewDetailsItem = new ToolStripMenuItem("Sửa");
            //viewDetailsItem.Click += ViewDetailsItem_Click;
            contextMenuStrip.Items.Add(viewDetailsItem);

            ToolStripMenuItem viewListItem = new ToolStripMenuItem("Thêm câu hỏi mới");
            //viewListItem.Click += ViewListItem_Click;
            contextMenuStrip.Items.Add(viewListItem);
        }

        private void lstExam_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void lstQuestion_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine("Chuot phai ne");
                // Xác định vị trí của chuột trên ListView
                Point clickPoint = lstQuestion.PointToClient(new Point(e.X, e.Y));

                // Hiển thị ContextMenuStrip tại vị trí của chuột
                contextMenuStrip.Show(lstQuestion, e.Location);
            }
        }

        private void CreateExam_Load(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            resetSearch();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == placeholderSearch)
            {
                //placeholderText = "";
                txtSearch.Text = string.Empty;
                txtSearch.ForeColor = SystemColors.WindowText;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtNumberQuestion.Clear();
            txtQuestion.Clear();
            cbbDifficult.SelectedIndex = -1;
            cbbSubject.SelectedIndex = -1;
            for (int i = 0; i < clbChapter.Items.Count; i++) clbChapter.SetItemChecked(i, false);
            txtSearch.Clear();
            resetSearch();
            txtTime.Clear();
        }

        private void resetSearch()
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text) || txtSearch.Text == placeholderSearch)
            {
                txtSearch.Text = placeholderSearch;
                txtSearch.ForeColor = SystemColors.GrayText;
            }
        }
    }
}
