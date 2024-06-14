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
    public partial class CreateQuestion : Form
    {
        private string placeholderText = "Kết quả của câu trả lời";
        private string placeholderSearch = "Tìm câu hỏi";
        public CreateQuestion()
        {
            InitializeComponent();
            txtResult.Text = placeholderText;
            txtResult.ForeColor = SystemColors.GrayText;
            txtSearch.Text = placeholderSearch;
            txtSearch.ForeColor = SystemColors.GrayText;
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            if(txtResult.Text.Contains("Đ") || txtResult.Text.Contains("S"))
            {
                rdTF.Checked=true;
            }
            else
            {
                rdSX.Checked = true;
            }
        }

        private void txtResult_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtResult.Text) || txtResult.Text == placeholderText)
            {
                txtResult.Text = placeholderText;
                txtResult.ForeColor = SystemColors.GrayText;
            }
        }

        private void txtResult_Enter(object sender, EventArgs e)
        {
            // Xóa placeholder text khi TextBox được chọn
            if (txtResult.Text == placeholderText)
            {
                //placeholderText = "";
                txtResult.Text = string.Empty;
                txtResult.ForeColor = SystemColors.WindowText;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text) || txtSearch.Text == placeholderSearch)
            {
                txtSearch.Text = placeholderSearch;
                txtSearch.ForeColor = SystemColors.GrayText;
            }
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
