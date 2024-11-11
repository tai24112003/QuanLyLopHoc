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


namespace testUdpTcp
{
    public partial class Waiting : Form
    {
        private readonly Action<string> GetMSSV;
        public Waiting(Action<string> getMSSV)
        {
            InitializeComponent();
            CenterTextBoxAndButton(txtMSSV, btnLogin);
          //  GetMSSV = getMSSV;
        }

        private void CenterTextBoxAndButton(TextBox textBox, Button button)
        {
            // Căn giữa TextBox theo chiều ngang
            textBox.Left = (this.ClientSize.Width - textBox.Width) / 2;
            textBox.Top = (this.ClientSize.Height - textBox.Height - button.Height - 10) / 2; // Điều chỉnh 10 để thêm khoảng cách giữa TextBox và Button

            // Căn giữa Button theo chiều ngang, đặt nó ngay dưới TextBox
            button.Left = (this.ClientSize.Width - button.Width) / 2;
            button.Top = textBox.Bottom + 10; // Khoảng cách giữa TextBox và Button
        }

        private void CenterLabel(Label label)
        {
            // Căn giữa chiều ngang
            label.Left = (this.ClientSize.Width - label.Width) / 2;

            // Căn giữa chiều dọc
            label.Top = (this.ClientSize.Height - label.Height) / 2;
            label.Padding = new Padding(10, 10, 10, 10);

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMSSV.Text.Trim())|| txtMSSV.Text.Trim().Length!=10)
            {
                return;
            }
            txtMSSV.Hide();
            btnLogin.Hide();
            CenterLabel(label1);
            GetMSSV?.Invoke(txtMSSV.Text.Trim());

            this.Close();
        }
    }
}
