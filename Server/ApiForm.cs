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
    public partial class ApiForm : Form
    {
        public string ApiUrl { get; private set; }
        public ApiForm()
        {
            InitializeComponent();
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ApiUrl = textBox1.Text; // Lấy URL API từ TextBox
            this.DialogResult = DialogResult.OK; // Xác nhận form
            this.Close();
        }
    }
}
