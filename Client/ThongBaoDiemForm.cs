using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class ThongBaoDiemForm : Form
    {
        public ThongBaoDiemForm(double Score)
        {
            InitializeComponent();
            label1.Text = Score.ToString();
            label1.Location = new Point((this.ClientSize.Width - label1.Width) / 2, (this.ClientSize.Height - label1.Height) / 2);
        }
    }
}
