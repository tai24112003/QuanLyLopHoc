using System;
using System.Windows.Forms;

namespace Server
{
    public partial class ConfigForm : Form
    {
        public int FPS
        {
            get => (int)numFPS.Value; 
            set
            {
                if (value >= numFPS.Minimum && value <= numFPS.Maximum)
                    numFPS.Value = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(value), $"FPS phải nằm trong khoảng {numFPS.Minimum} - {numFPS.Maximum}.");
            }
        }

        public int Quality
        {
            get => (int)numQuality.Value; 
            set
            {
                if (value >= numQuality.Minimum && value <= numQuality.Maximum)
                    numQuality.Value = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(value), $"Quality phải nằm trong khoảng {numQuality.Minimum} - {numQuality.Maximum}.");
            }
        }

        public bool UseTCP
        {
            get => radTCP.Checked; 
            set
            {
                radTCP.Checked = value;
                radUDP.Checked = !value;
            }
        }
        public string Protocol => UseTCP ? "TCP" : "UDP";

        public ConfigForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra thêm nếu cần
            if (FPS < numFPS.Minimum || FPS > numFPS.Maximum)
            {
                MessageBox.Show($"FPS phải nằm trong khoảng {numFPS.Minimum} - {numFPS.Maximum}.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Quality < numQuality.Minimum || Quality > numQuality.Maximum)
            {
                MessageBox.Show($"Chất lượng ảnh phải nằm trong khoảng {numQuality.Minimum} - {numQuality.Maximum}.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Đóng form với kết quả OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Đóng form với kết quả Cancel
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

       
    }
}
