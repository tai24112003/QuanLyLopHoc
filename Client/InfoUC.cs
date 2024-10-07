using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    public partial class InfoUC : UserControl
    {
        public InfoUC()
        {
            InitializeComponent();

        }
        public Image Image
        {
            get { return pbIcon.Image; }
            set { pbIcon.Image = value; }
        }

        public string TextLabel
        {
            get { return lblContent.Text; }
            set { lblContent.Text = value; }
        }
    }
    
}
