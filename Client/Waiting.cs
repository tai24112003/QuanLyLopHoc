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
using Newtonsoft.Json;
using DAL.Models;
using System.Text.RegularExpressions;


namespace testUdpTcp
{
    public partial class Waiting : Form
    {
        private readonly Action<string, string> GetMSSV;
        public Waiting(Action<string,string> getMSSV)
        {
            InitializeComponent();
            CenterTextBoxAndButton(txtMSSV, btnLogin);
            GetMSSV = getMSSV;

            SetPlaceholder(txt_name, "Nhập họ và tên...");
            SetPlaceholder(txtMSSV, "Nhập MSSV...");
        }

        private void CenterTextBoxAndButton(TextBox textBox, Button button)
        {
            // Căn giữa TextBox theo chiều ngang
            textBox.Left = (this.ClientSize.Width - textBox.Width) / 2;
            textBox.Top = (this.ClientSize.Height - textBox.Height - button.Height - 10) / 2; // Điều chỉnh 10 để thêm khoảng cách giữa TextBox và Button

            txt_name.Left= (this.ClientSize.Width - textBox.Width) / 2;
            txt_name.Top = textBox.Bottom + 25;

            // Căn giữa Button theo chiều ngang, đặt nó ngay dưới TextBox
            button.Left = (this.ClientSize.Width - button.Width) / 2;
            button.Top = txt_name.Bottom + 15; // Khoảng cách giữa TextBox và Button

            
        }

        private void CenterLabel(Label label, int top)
        {
            // Căn giữa chiều ngang
            label.Left = (this.ClientSize.Width - label.Width) / 2;

            // Căn giữa chiều dọc
            label.Top = (this.ClientSize.Height - label.Height) *top/ 4;
            label.Padding = new Padding(10, 10, 10, 10);

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMSSV.Text.Trim()))
            {
                MessageBox.Show("Mã số sinh viên không được để trống.");
                return;
            }
            string mssvPattern = @"^0\d{9}$"; // Bắt đầu bằng '0' và theo sau là 9 chữ số.
            if (!Regex.IsMatch(txtMSSV.Text.Trim(), mssvPattern))
            {
                MessageBox.Show("Mã số sinh viên phải có dạng '0XXXXXXXXX' (10 chữ số).");
                return;
            }

            RemovePlaceholder(txt_name, "Nhập họ và tên...");
            string fullName = txt_name.Text.Trim();
            if (!ValidateFullName(fullName, out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txt_name.Hide();
            txtMSSV.Hide();
            btnLogin.Hide();
            CenterLabel(label1, 2);
            GetMSSV?.Invoke(txtMSSV.Text.Trim(), txt_name.Text.Trim());

            this.Dispose();
        }

        private bool ValidateFullName(string fullName, out string errorMessage)
        {
            // Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(fullName))
            {
                errorMessage = "Họ và tên không được để trống.";
                return false;
            }

            // Kiểm tra độ dài
            if (fullName.Length > 50)
            {
                errorMessage = "Họ và tên không được vượt quá 50 ký tự.";
                return false;
            }

            // Kiểm tra ký tự hợp lệ (chỉ cho phép chữ cái và khoảng trắng)
            if (!Regex.IsMatch(fullName, @"^[\p{L}\s]+$"))
            {
                errorMessage = "Họ và tên chỉ được chứa chữ cái và khoảng trắng.";
                return false;
            }


            errorMessage = string.Empty;
            return true;
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Color.Gray; // Màu placeholder
            }
        }

        private void RemovePlaceholder(TextBox textBox, string placeholder)
        {
            if (textBox.Text == placeholder)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black; // Màu chữ bình thường
            }
        }

        private void txtMSSV_GotFocus(object sender, EventArgs e)
        {
            RemovePlaceholder(txtMSSV, "Nhập MSSV...");
        }

        private void txtMSSV_LostFocus(object sender, EventArgs e)
        {
            SetPlaceholder(txtMSSV, "Nhập MSSV...");
        }

        private void txt_name_GotFocus(object sender, EventArgs e)
        {
            RemovePlaceholder(txt_name, "Nhập họ và tên...");
        }

        private void txt_name_LostFocus(object sender, EventArgs e)
        {
            SetPlaceholder(txt_name, "Nhập họ và tên...");
        }
    }
}
